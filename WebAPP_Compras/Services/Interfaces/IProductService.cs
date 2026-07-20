using WebAPP_Compras.Models.DTOs.Products;

namespace WebAPP_Compras.Services.Interfaces;

public interface IProductService
{
    Task<IReadOnlyCollection<ProductResponse>> GetAllAsync(
        int? storeId,
        bool includeInactive,
        CancellationToken cancellationToken = default);

    Task<ProductResponse> GetByIdAsync(
        int id,
        CancellationToken cancellationToken = default);

    Task<ProductResponse> CreateAsync(
        CreateProductRequest request,
        CancellationToken cancellationToken = default);

    Task<ProductResponse> UpdateAsync(
        int id,
        UpdateProductRequest request,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(
        int id,
        CancellationToken cancellationToken = default);
}