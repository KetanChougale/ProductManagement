using ProductManagement.Application.DTOs;

namespace ProductManagement.Application.Interfaces;

public interface IProductService
{
    Task<List<ProductResponse>> GetAllAsync();

    Task<ProductResponse> CreateAsync(ProductCreateRequest request);

    Task<ProductResponse?> UpdateAsync(
        int id,
        ProductUpdateRequest request);

    Task<bool> DeleteAsync(int id);
}
