using System.Net;
using System.Net.Http.Json;
using ProductManagement.Tests.Infrastructure;
using ProductManagement.Infrastructure.Data;
using ProductManagement.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace ProductManagement.Tests;

public class ProductControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ProductControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetAll_ShouldReturn200Ok()
    {
        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetAll_ShouldReturnSuccessResponse()
    {
        // Act
        var response = await _client.GetAsync("/api/products");

        // Assert
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Products retrieved successfully", content);
    }

    [Fact]
    public async Task Create_ShouldReturn201Created()
    {
        // Arrange
        var request = new
        {
            name = "Integration Test Laptop",
            price = 75000,
            stockQuantity = 10
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedProduct()
    {
        // Arrange
        var request = new
        {
            name = "Integration Test Monitor",
            price = 15000,
            stockQuantity = 15
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", request);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();

        Assert.Contains("Integration Test Monitor", content);
    }

    [Fact]
    public async Task Create_ShouldReturn400_WhenRequestIsInvalid()
    {
        // Arrange
        var request = new
        {
            name = "",
            price = -100,
            stockQuantity = -5
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/products", request);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Update_ShouldReturn404_WhenProductDoesNotExist()
    {
        // Arrange
        var request = new
        {
            name = "Does Not Exist",
            price = 1000,
            stockQuantity = 5
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/products/999999", request);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Delete_ShouldReturn404_WhenProductDoesNotExist()
    {
        // Act
        var response = await _client.DeleteAsync("/api/products/999999");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
