using ECommerce.Application.Services;
using ECommerce.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize] // 需要身份验证
    public class AdminController : BaseController
    {
        private readonly IProductService _productService;
        private readonly ILogger<AdminController> _logger;

        public AdminController(IProductService productService, ILogger<AdminController> logger)
        {
            _productService = productService;
            _logger = logger;
        }

        /// <summary>
        /// 检查当前用户是否为管理员
        /// </summary>
        private bool IsAdmin()
        {
            var roleClaim = User.FindFirst(ClaimTypes.Role);
            return roleClaim?.Value == "Admin";
        }

        /// <summary>
        /// 检查管理员权限，如果不是管理员则返回403
        /// </summary>
        private ActionResult? CheckAdminPermission()
        {
            if (!IsAdmin())
            {
                _logger.LogWarning("Non-admin user {UserId} attempted to access admin endpoint", CurrentUserId);
                return Forbid("Access denied. Admin privileges required.");
            }
            return null;
        }

        /// <summary>
        /// 获取所有产品（管理员视图）
        /// </summary>
        [HttpGet("products")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAllProducts()
        {
            var permissionCheck = CheckAdminPermission();
            if (permissionCheck != null) return permissionCheck;

            try
            {
                var products = await _productService.GetAllProductsAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all products for admin");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 创建新产品
        /// </summary>
        [HttpPost("products")]
        public async Task<ActionResult<ProductDto>> CreateProduct(CreateProductDto createProductDto)
        {
            var permissionCheck = CheckAdminPermission();
            if (permissionCheck != null) return permissionCheck;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                var product = await _productService.CreateProductAsync(createProductDto);
                _logger.LogInformation("Admin {AdminId} created product: {ProductId} - {ProductName}", 
                    CurrentUserId, product.Id, product.Name);
                return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product in admin panel");
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 获取单个产品详情
        /// </summary>
        [HttpGet("products/{id}")]
        public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
        {
            var permissionCheck = CheckAdminPermission();
            if (permissionCheck != null) return permissionCheck;

            try
            {
                var product = await _productService.GetProductByIdAsync(id);
                if (product == null)
                    return NotFound();

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting product {ProductId} for admin", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 更新产品信息
        /// </summary>
        [HttpPut("products/{id}")]
        public async Task<ActionResult<ProductDto>> UpdateProduct(Guid id, UpdateProductDto updateProductDto)
        {
            var permissionCheck = CheckAdminPermission();
            if (permissionCheck != null) return permissionCheck;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            
            try
            {
                var product = await _productService.UpdateProductAsync(id, updateProductDto);
                _logger.LogInformation("Admin {AdminId} updated product: {ProductId} - {ProductName}", 
                    CurrentUserId, product.Id, product.Name);
                return Ok(product);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product {ProductId} in admin panel", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 删除产品
        /// </summary>
        [HttpDelete("products/{id}")]
        public async Task<ActionResult> DeleteProduct(Guid id)
        {
            var permissionCheck = CheckAdminPermission();
            if (permissionCheck != null) return permissionCheck;

            try
            {
                var result = await _productService.DeleteProductAsync(id);
                if (!result)
                    return NotFound();

                _logger.LogInformation("Admin {AdminId} deleted product: {ProductId}", CurrentUserId, id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product {ProductId} in admin panel", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 批量更新产品状态
        /// </summary>
        [HttpPut("products/batch-status")]
        public async Task<ActionResult> BatchUpdateProductStatus([FromBody] BatchUpdateProductStatusDto batchUpdateDto)
        {
            var permissionCheck = CheckAdminPermission();
            if (permissionCheck != null) return permissionCheck;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var successCount = 0;
                var errors = new List<string>();

                foreach (var productId in batchUpdateDto.ProductIds)
                {
                    try
                    {
                        var updateDto = new UpdateProductDto
                        {
                            IsActive = batchUpdateDto.IsActive
                        };
                        
                        // 获取现有产品信息
                        var existingProduct = await _productService.GetProductByIdAsync(productId);
                        if (existingProduct != null)
                        {
                            updateDto.Name = existingProduct.Name;
                            updateDto.Description = existingProduct.Description;
                            updateDto.Price = existingProduct.Price;
                            updateDto.Stock = existingProduct.Stock;
                            updateDto.Category = existingProduct.Category;
                            updateDto.ImageUrl = existingProduct.ImageUrl;
                            
                            await _productService.UpdateProductAsync(productId, updateDto);
                            successCount++;
                        }
                        else
                        {
                            errors.Add($"Product {productId} not found");
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Error updating product {productId}: {ex.Message}");
                    }
                }

                _logger.LogInformation("Admin {AdminId} batch updated {SuccessCount} products status to {IsActive}", 
                    CurrentUserId, successCount, batchUpdateDto.IsActive);

                return Ok(new { 
                    SuccessCount = successCount, 
                    ErrorCount = errors.Count,
                    Errors = errors 
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in batch update product status");
                return StatusCode(500, "Internal server error");
            }
        }
    }

    /// <summary>
    /// 批量更新产品状态的数据传输对象
    /// </summary>
    public class BatchUpdateProductStatusDto
    {
        public List<Guid> ProductIds { get; set; } = new List<Guid>();
        public bool IsActive { get; set; }
    }
}
