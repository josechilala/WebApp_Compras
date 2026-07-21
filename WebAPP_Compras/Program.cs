using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using WebAPP_Compras.Data;
using WebAPP_Compras.Models.Entities;
using WebAPP_Compras.Models.Settings;
using WebAPP_Compras.Repositories;
using WebAPP_Compras.Repositories.Interfaces;
using WebAPP_Compras.Services;
using WebAPP_Compras.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

ConfigureSwagger(builder.Services);

ConfigureDatabase(
    builder.Services,
    builder.Configuration);

ConfigureJwtSettings(
    builder.Services,
    builder.Configuration);

ConfigureAuthentication(
    builder.Services,
    builder.Configuration);

ConfigureRepositories(builder.Services);

ConfigureServices(builder.Services);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "WebAPP Compras API v1");

        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

static void ConfigureDatabase(
    IServiceCollection services,
    IConfiguration configuration)
{
    string connectionString =
        configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException(
            "A connection string 'DefaultConnection' não foi configurada.");

    services.AddDbContext<ApplicationDbContext>(options =>
    {
        options.UseSqlServer(connectionString);
    });
}

static void ConfigureJwtSettings(
    IServiceCollection services,
    IConfiguration configuration)
{
    services
        .AddOptions<JwtSettings>()
        .Bind(
            configuration.GetSection(
                JwtSettings.SectionName))
        .ValidateDataAnnotations()
        .Validate(
            settings =>
                !string.IsNullOrWhiteSpace(settings.Key) &&
                Encoding.UTF8.GetByteCount(settings.Key) >= 32,
            "A chave JWT deve possuir pelo menos 32 bytes.")
        .ValidateOnStart();
}

static void ConfigureAuthentication(
    IServiceCollection services,
    IConfiguration configuration)
{
    JwtSettings jwtSettings =
        configuration
            .GetSection(JwtSettings.SectionName)
            .Get<JwtSettings>()
        ?? throw new InvalidOperationException(
            "As configurações JWT não foram encontradas.");

    if (string.IsNullOrWhiteSpace(jwtSettings.Key))
    {
        throw new InvalidOperationException(
            "A chave JWT não foi configurada. " +
            "Configure Jwt:Key no User Secrets.");
    }

    var signingKey = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(jwtSettings.Key));

    services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme =
                JwtBearerDefaults.AuthenticationScheme;

            options.DefaultChallengeScheme =
                JwtBearerDefaults.AuthenticationScheme;

            options.DefaultScheme =
                JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.RequireHttpsMetadata = true;

            options.SaveToken = true;

            options.TokenValidationParameters =
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings.Issuer,

                    ValidateAudience = true,
                    ValidAudience = jwtSettings.Audience,

                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = signingKey,

                    ValidateLifetime = true,

                    RequireExpirationTime = true,

                    ClockSkew = TimeSpan.Zero
                };
        });

    services.AddAuthorization();
}

static void ConfigureSwagger(
    IServiceCollection services)
{
    services.AddSwaggerGen(options =>
    {
        const string securitySchemeName = "Bearer";

        options.SwaggerDoc(
            "v1",
            new OpenApiInfo
            {
                Title = "WebAPP Compras API",
                Version = "v1",
                Description =
                    "API para gerenciamento de mercados, " +
                    "produtos, usuários e pedidos."
            });

        options.AddSecurityDefinition(
            securitySchemeName,
            new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description =
                    "Informe somente o token JWT, sem aspas."
            });

        options.AddSecurityRequirement(document =>
            new OpenApiSecurityRequirement
            {
                [
                    new OpenApiSecuritySchemeReference(
                        securitySchemeName,
                        document)
                ] = []
            });
    });
}

static void ConfigureRepositories(
    IServiceCollection services)
{
    services.AddScoped<
        IUserRepository,
        UserRepository>();

    services.AddScoped<
        IStoreRepository,
        StoreRepository>();

    services.AddScoped<
        IProductRepository,
        ProductRepository>();

    services.AddScoped<
        IAddressRepository,
        AddressRepository>();

    services.AddScoped<
        IDeliveryScheduleRepository,
        DeliveryScheduleRepository>();

    services.AddScoped<
        IOrderRepository,
        OrderRepository>();
}

static void ConfigureServices(
    IServiceCollection services)
{
    services.AddHttpContextAccessor();

    services.AddScoped<
        IAuthService,
        AuthService>();

    services.AddScoped<
        IStoreService,
        StoreService>();

    services.AddScoped<
        IProductService,
        ProductService>();

    services.AddScoped<
        IAddressService,
        AddressService>();

    services.AddScoped<
        IDeliveryScheduleService,
        DeliveryScheduleService>();

    services.AddScoped<
        IOrderService,
        OrderService>();

    services.AddScoped<
        IJwtService,
        JwtService>();

    services.AddScoped<
        ICurrentUserService,
        CurrentUserService>();

    services.AddScoped<PasswordHasher<User>>();
}