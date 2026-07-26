using Microsoft.Extensions.Logging;
using ProductManagement.Application.DTOs;
using ProductManagement.Application.Interfaces;
using ProductManagement.Domain.Entities;

namespace ProductManagement.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ProductService> _logger;

    public ProductService(
        IProductRepository productRepository,
        ILogger<ProductService> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    public async Task<List<ProductResponse>> GetAllAsync()
    {
        _logger.LogInformation("Fetching all products.");

        var products = await _productRepository.GetAllAsync();

        _logger.LogInformation("Retrieved {ProductCount} products.", products.Count);

        return products.Select(product => new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            StockQuantity = product.StockQuantity,
            IsActive = product.IsActive,
            CreatedAt = product.CreatedAt
        }).ToList();
    }

    public async Task<ProductResponse> CreateAsync(
        ProductCreateRequest request)
    {
        _logger.LogInformation("Creating product with name {ProductName}.", request.Name);

        var product = new Product
        {
            Name = request.Name,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createdProduct =
            await _productRepository.CreateAsync(product);

        _logger.LogInformation("Product created successfully with ID {ProductId}.", createdProduct.Id);

        return new ProductResponse
        {
            Id = createdProduct.Id,
            Name = createdProduct.Name,
            Price = createdProduct.Price,
            StockQuantity = createdProduct.StockQuantity,
            IsActive = createdProduct.IsActive,
            CreatedAt = createdProduct.CreatedAt
        };
    }

    public async Task<ProductResponse?> UpdateAsync(
        int id,
        ProductUpdateRequest request)
    {
        _logger.LogInformation("Updating product with ID {ProductId}.", id);

        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
        {
            _logger.LogWarning("Product with ID {ProductId} was not found.", id);
            return null;
        }

        product.Name = request.Name;
        product.Price = request.Price;
        product.StockQuantity = request.StockQuantity;

        var updatedProduct =
            await _productRepository.UpdateAsync(product);

        _logger.LogInformation("Product with ID {ProductId} updated successfully.", id);

        return new ProductResponse
        {
            Id = updatedProduct.Id,
            Name = updatedProduct.Name,
            Price = updatedProduct.Price,
            StockQuantity = updatedProduct.StockQuantity,
            IsActive = updatedProduct.IsActive,
            CreatedAt = updatedProduct.CreatedAt
        };
    }

    public async Task<bool> DeleteAsync(int id)
    {
        _logger.LogInformation("Deleting product with ID {ProductId}.", id);

        var deleted = await _productRepository.DeleteAsync(id);

        if (!deleted)
        {
            _logger.LogWarning("Product with ID {ProductId} was not found for deletion.", id);
            return false;
        }

        _logger.LogInformation("Product with ID {ProductId} deleted successfully.", id);

        return true;
    }
}
