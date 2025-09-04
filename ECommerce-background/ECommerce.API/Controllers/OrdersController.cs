using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class OrdersController : BaseController
    {
        private readonly IOrderService _orderService;
        private readonly IRabbitMqDelayPublisher _delayPublisher;

        public OrdersController(IOrderService orderService, IRabbitMqDelayPublisher delayPublisher)
        {
            _orderService = orderService;
            _delayPublisher = delayPublisher;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            if (CurrentUserId == null)
                return BadRequest("Invalid user");

            var orders = await _orderService.GetUserOrdersAsync(CurrentUserId.Value);
            return Ok(orders);
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderDto>> GetOrder(Guid id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            if (order == null)
                return NotFound();

            if (!IsAdmin && order.UserId != CurrentUserId)
                return Forbid();

            return Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult<OrderDto>> CreateOrder(CreateOrderDto createOrderDto)
        {
            // 模型验证
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (CurrentUserId == null)
                return BadRequest("Invalid user");

            var order = await _orderService.CreateOrderAsync(CurrentUserId.Value, createOrderDto);

            // 发送延迟消息
            await _delayPublisher.PublishOrderDelayMessageAsync(order.Id, TimeSpan.FromMinutes(30));

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        [HttpPost("payment")]
        public async Task<ActionResult> ProcessPayment(PaymentDto paymentDto)
        {
            // 模型验证
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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
            // 模型验证
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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