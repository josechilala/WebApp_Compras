using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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

//
// BANCO DE DADOS
//

string connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "A ConnectionString 'DefaultConnection' não foi configurada.");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

//
// CONFIGURAÇÕES JWT
//

builder.Services
    .AddOptions<JwtSettings>()
    .BindConfiguration(JwtSettings.SectionName)
    .ValidateDataAnnotations()
    .Validate(
        settings =>
            !string.IsNullOrWhiteSpace(settings.Key) &&
            settings.Key.Length >= 32,
        "A chave JWT deve possuir pelo menos 32 caracteres.")
    .ValidateOnStart();

JwtSettings jwtSettings =
    builder.Configuration
        .GetSection(JwtSettings.SectionName)
        .Get<JwtSettings>()
    ?? throw new InvalidOperationException(
        "A seção Jwt não foi configurada.");

if (string.IsNullOrWhiteSpace(jwtSettings.Key))
{
    throw new InvalidOperationException(
        "A chave JWT não foi configurada. " +
        "Adicione Jwt:Key aos Segredos do Usuário.");
}

if (jwtSettings.Key.Length < 32)
{
    throw new InvalidOperationException(
        "A chave JWT deve possuir pelo menos 32 caracteres.");
}

//
// AUTENTICAÇÃO JWT
//

builder.Services
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
        options.SaveToken = false;
        options.IncludeErrorDetails =
            builder.Environment.IsDevelopment();

        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtSettings.Issuer,

                ValidateAudience = true,
                ValidAudience = jwtSettings.Audience,

                ValidateIssuerSigningKey = true,
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Key)),

                ValidateLifetime = true,
                RequireExpirationTime = true,
                RequireSignedTokens = true,

                ClockSkew = TimeSpan.Zero,

                NameClaimType = ClaimTypes.Name,
                RoleClaimType = ClaimTypes.Role
            };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    Console.WriteLine(
                        $"Falha JWT: " +
                        $"{context.Exception.GetType().Name}");

                    Console.WriteLine(
                        context.Exception.Message);
                }

                return Task.CompletedTask;
            },

            OnTokenValidated = context =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    Console.WriteLine(
                        "Token JWT validado com sucesso.");
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

//
// MVC E CONTROLLERS
//

builder.Services.AddControllersWithViews();

//
// SWAGGER
//

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    const string securitySchemeName = "Bearer";

    options.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "WebAPP Compras API",
            Version = "v1",
            Description =
                "API para compras em mercados e entregas agendadas."
        });

    options.AddSecurityDefinition(
        securitySchemeName,
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description =
                "Cole somente o token JWT retornado pelo login. " +
                "Não informe a palavra Bearer."
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

//
// INJEÇÃO DE DEPENDÊNCIAS
//

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IStoreRepository, StoreRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IJwtService, JwtService>();

builder.Services.AddScoped<PasswordHasher<User>>();

//
// CONSTRUÇÃO DA APLICAÇÃO
//

var app = builder.Build();

//
// PIPELINE HTTP
//

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint(
            "/swagger/v1/swagger.json",
            "WebAPP Compras API v1");

        options.DisplayRequestDuration();
        options.EnableTryItOutByDefault();
    });
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllers();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();