using Microsoft.AspNetCore.Mvc;
using ProductManagement.Application.DTOs;
using ProductManagement.Application.Interfaces;

namespace ProductManagement.API.Controllers;

[ApiController]
[Route("api/products")]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<List<ProductResponse>>>> GetAll()
    {
        var products = await _productService.GetAllAsync();

        var response = ApiResponse<List<ProductResponse>>.SuccessResponse(
            products,
            "Products retrieved successfully.");

        return Ok(response);
    }

    [HttpPost]
    public async Task<ActionResult<ApiResponse<ProductResponse>>> Create(
        ProductCreateRequest request)
    {
        var createdProduct =
            await _productService.CreateAsync(request);

        var response = ApiResponse<ProductResponse>.SuccessResponse(
            createdProduct,
            "Product created successfully.");

        return CreatedAtAction(
            nameof(GetAll),
            new { id = createdProduct.Id },
            response);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ApiResponse<ProductResponse>>> Update(
        int id,
        ProductUpdateRequest request)
    {
        var updatedProduct =
            await _productService.UpdateAsync(id, request);

        if (updatedProduct is null)
        {
            var errorResponse =
                ApiResponse<ProductResponse>.FailureResponse(
                    $"Product with ID {id} was not found.");

            return NotFound(errorResponse);
        }

        var response = ApiResponse<ProductResponse>.SuccessResponse(
            updatedProduct,
            "Product updated successfully.");

        return Ok(response);
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
    {
        var deleted = await _productService.DeleteAsync(id);

        if (!deleted)
        {
            var errorResponse =
                ApiResponse<object>.FailureResponse(
                    $"Product with ID {id} was not found.");

            return NotFound(errorResponse);
        }

        var response =
            ApiResponse<object>.SuccessResponse(
                null,
                "Product deleted successfully.");

        return Ok(response);
    }
}
