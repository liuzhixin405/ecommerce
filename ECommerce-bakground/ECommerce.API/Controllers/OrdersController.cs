using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                    return BadRequest("Invalid user");

                var orders = await _orderService.GetUserOrdersAsync(userId);
                return Ok(orders);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrdersAsync();
                return Ok(orders);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(Guid id)
        {
            try
            {
                var order = await _orderService.GetOrderByIdAsync(id);
                if (order == null)
                    return NotFound();

                // Check if user owns this order or is admin
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                var isAdmin = User.IsInRole("Admin");
                
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                    return BadRequest("Invalid user");

                if (!isAdmin && order.UserId != userId)
                    return Forbid();

                return Ok(order);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto createOrderDto)
        {
            try
            {
                var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier);
                if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
                    return BadRequest("Invalid user");

                var order = await _orderService.CreateOrderAsync(userId, createOrderDto);
                return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("payment")]
        public async Task<ActionResult> ProcessPayment(PaymentDto paymentDto)
        {
            try
            {
                var result = await _orderService.ProcessPaymentAsync(paymentDto);
                if (!result)
                    return BadRequest("Payment processing failed");

                return Ok(new { message = "Payment processed successfully" });
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}/cancel")]
        public async Task<ActionResult> CancelOrder(Guid id)
        {
            try
            {
                var result = await _orderService.CancelOrderAsync(id);
                if (!result)
                    return BadRequest("Order cancellation failed");

                return Ok(new { message = "Order cancelled successfully" });
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}/ship")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> ShipOrder(Guid id, [FromBody] ShipOrderRequest request)
        {
            try
            {
                var result = await _orderService.ShipOrderAsync(id, request.TrackingNumber);
                if (!result)
                    return BadRequest("Order shipping failed");

                return Ok(new { message = "Order shipped successfully" });
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}/deliver")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeliverOrder(Guid id)
        {
            try
            {
                var result = await _orderService.DeliverOrderAsync(id);
                if (!result)
                    return BadRequest("Order delivery failed");

                return Ok(new { message = "Order delivered successfully" });
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<OrderDto>> UpdateOrderStatus(Guid id, UpdateOrderStatusDto updateOrderStatusDto)
        {
            try
            {
                var order = await _orderService.UpdateOrderStatusAsync(id, updateOrderStatusDto);
                return Ok(order);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPost("cancel-expired")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> CancelExpiredOrders()
        {
            try
            {
                var result = await _orderService.CancelExpiredOrdersAsync();
                return Ok(new { message = "Expired orders cancelled successfully" });
            }
            catch (Exception)
            {
                return StatusCode(500, "Internal server error");
            }
        }
    }

    public class ShipOrderRequest
    {
        public string TrackingNumber { get; set; } = string.Empty;
    }
}