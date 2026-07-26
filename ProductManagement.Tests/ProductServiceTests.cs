using Moq;
using Xunit;
using ProductManagement.Application.Interfaces;
using ProductManagement.Application.Services;
using ProductManagement.Domain.Entities;
using ProductManagement.Application.DTOs;
using Microsoft.Extensions.Logging;

namespace ProductManagement.Tests;

public class ProductServiceTests
{
    private readonly Mock<IProductRepository> _repositoryMock;
    private readonly Mock<ILogger<ProductService>> _loggerMock;
    private readonly ProductService _productService;

    public ProductServiceTests()
    {
        _repositoryMock = new Mock<IProductRepository>();
        _loggerMock = new Mock<ILogger<ProductService>>();

        _productService = new ProductService(
            _repositoryMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new Product { Id = 1, Name = "Laptop", Price = 75000m, StockQuantity = 10, IsActive = true, CreatedAt = System.DateTime.UtcNow },
            new Product { Id = 2, Name = "Mouse", Price = 1500m, StockQuantity = 20, IsActive = true, CreatedAt = System.DateTime.UtcNow }
        };

        _repositoryMock
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(products);

        // Act
        var result = await _productService.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
    }

    [Fact]
    public async Task CreateAsync_ShouldCreateProduct()
    {
        // Arrange
        var request = new ProductCreateRequest
        {
            Name = "Keyboard",
            Price = 2500m,
            StockQuantity = 15
        };

        var createdProduct = new Product
        {
            Id = 3,
            Name = request.Name,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            IsActive = true,
            CreatedAt = System.DateTime.UtcNow
        };

        _repositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Product>()))
            .ReturnsAsync(createdProduct);

        // Act
        var result = await _productService.CreateAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Id);
        Assert.Equal("Keyboard", result.Name);
        Assert.Equal(2500m, result.Price);

        _repositoryMock.Verify(x => x.CreateAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldUpdateExistingProduct()
    {
        // Arrange
        var existingProduct = new Product
        {
            Id = 1,
            Name = "Laptop",
            Price = 75000m,
            StockQuantity = 10,
            IsActive = true,
            CreatedAt = System.DateTime.UtcNow
        };

        var request = new ProductUpdateRequest
        {
            Name = "Gaming Laptop",
            Price = 85000m,
            StockQuantity = 8
        };

        var updatedProduct = new Product
        {
            Id = existingProduct.Id,
            Name = request.Name,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            IsActive = existingProduct.IsActive,
            CreatedAt = existingProduct.CreatedAt
        };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(1))
            .ReturnsAsync(existingProduct);

        _repositoryMock
            .Setup(x => x.UpdateAsync(It.IsAny<Product>()))
            .ReturnsAsync(updatedProduct);

        // Act
        var result = await _productService.UpdateAsync(1, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Gaming Laptop", result!.Name);
        Assert.Equal(85000m, result.Price);
        Assert.Equal(8, result.StockQuantity);

        _repositoryMock.Verify(x => x.GetByIdAsync(1), Times.Once);
        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Product>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_ShouldReturnNull_WhenProductDoesNotExist()
    {
        // Arrange
        var request = new ProductUpdateRequest
        {
            Name = "Gaming Laptop",
            Price = 85000m,
            StockQuantity = 8
        };

        _repositoryMock
            .Setup(x => x.GetByIdAsync(999))
            .ReturnsAsync((Product?)null);

        // Act
        var result = await _productService.UpdateAsync(999, request);

        // Assert
        Assert.Null(result);

        _repositoryMock.Verify(x => x.GetByIdAsync(999), Times.Once);
        _repositoryMock.Verify(x => x.UpdateAsync(It.IsAny<Product>()), Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnTrue_WhenProductExists()
    {
        // Arrange
        _repositoryMock
            .Setup(x => x.DeleteAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _productService.DeleteAsync(1);

        // Assert
        Assert.True(result);

        _repositoryMock.Verify(x => x.DeleteAsync(1), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_ShouldReturnFalse_WhenProductDoesNotExist()
    {
        // Arrange
        _repositoryMock
            .Setup(x => x.DeleteAsync(999))
            .ReturnsAsync(false);

        // Act
        var result = await _productService.DeleteAsync(999);

        // Assert
        Assert.False(result);

        _repositoryMock.Verify(x => x.DeleteAsync(999), Times.Once);
    }
}
