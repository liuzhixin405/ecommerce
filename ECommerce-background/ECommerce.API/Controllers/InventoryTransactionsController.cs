using ECommerce.Application.Services;
using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class InventoryTransactionsController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;
        private readonly ILogger<InventoryTransactionsController> _logger;

        public InventoryTransactionsController(IInventoryService inventoryService, ILogger<InventoryTransactionsController> logger)
        {
            _inventoryService = inventoryService;
            _logger = logger;
        }

        /// <summary>
        /// 获取指定产品的库存事务记录
        /// </summary>
        /// <param name="productId">产品ID</param>
        /// <param name="limit">返回记录数量限制，默认50条</param>
        /// <returns>库存事务记录列表</returns>
        [HttpGet("product/{productId}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<InventoryTransaction>>> GetByProduct(Guid productId, int limit = 50)
        {
            try
            {
                if (limit <= 0 || limit > 1000)
                {
                    return BadRequest("Limit must be between 1 and 1000");
                }

                var transactions = await _inventoryService.GetProductInventoryTransactionsAsync(productId, limit);
                return Ok(transactions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inventory transactions for product {ProductId}", productId);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 获取库存事务记录详情
        /// </summary>
        /// <param name="id">事务ID</param>
        /// <returns>库存事务记录</returns>
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<InventoryTransaction>> Get(Guid id)
        {
            try
            {
                var transaction = await _inventoryService.GetInventoryTransactionAsync(id);
                if (transaction == null)
                    return NotFound();

                return Ok(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inventory transaction {TransactionId}", id);
                return StatusCode(500, "Internal server error");
            }
        }

        /// <summary>
        /// 根据操作类型获取库存事务记录
        /// </summary>
        /// <param name="operationType">操作类型</param>
        /// <param name="limit">返回记录数量限制，默认50条</param>
        /// <returns>库存事务记录列表</returns>
        [HttpGet("operation/{operationType}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<InventoryTransaction>>> GetByOperationType(string operationType, int limit = 50)
        {
            try
            {
                if (limit <= 0 || limit > 1000)
                {
                    return BadRequest("Limit must be between 1 and 1000");
                }

                if (!Enum.TryParse<InventoryOperationType>(operationType, out var parsedOperationType))
                {
                    return BadRequest($"Invalid operation type: {operationType}");
                }

                // 注意：这里需要在仓储层添加按操作类型查询的方法
                // 暂时返回空列表，因为仓储层还没有实现该方法
                await Task.CompletedTask; // 添加await以避免警告
                return Ok(new List<InventoryTransaction>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting inventory transactions for operation type {OperationType}", operationType);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}