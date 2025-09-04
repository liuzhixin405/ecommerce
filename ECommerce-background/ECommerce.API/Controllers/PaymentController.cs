using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// 处理支付
        /// </summary>
        [HttpPost("process")]
        public async Task<ActionResult<PaymentResult>> ProcessPayment([FromBody] PaymentRequest paymentRequest)
        {
            try
            {
                _logger.LogInformation("Processing payment for order: {OrderId}", paymentRequest.OrderId);

                var result = await _paymentService.ProcessPaymentAsync(paymentRequest);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(new { message = result.Message, details = result });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment processing failed for order: {OrderId}", paymentRequest.OrderId);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// 验证支付
        /// </summary>
        [HttpGet("validate/{paymentId}")]
        public async Task<ActionResult<PaymentValidationResult>> ValidatePayment(string paymentId)
        {
            try
            {
                _logger.LogInformation("Validating payment: {PaymentId}", paymentId);

                var result = await _paymentService.ValidatePaymentAsync(paymentId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Payment validation failed for: {PaymentId}", paymentId);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// 处理退款
        /// </summary>
        [HttpPost("refund")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<RefundResult>> ProcessRefund([FromBody] RefundRequest refundRequest)
        {
            try
            {
                _logger.LogInformation("Processing refund for order: {OrderId}", refundRequest.OrderId);

                var result = await _paymentService.ProcessRefundAsync(refundRequest);

                if (result.Success)
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(new { message = result.Message, details = result });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Refund processing failed for order: {OrderId}", refundRequest.OrderId);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// 获取支付状态
        /// </summary>
        [HttpGet("status/{orderId}")]
        public async Task<ActionResult<PaymentStatus>> GetPaymentStatus(Guid orderId)
        {
            try
            {
                _logger.LogInformation("Getting payment status for order: {OrderId}", orderId);

                var result = await _paymentService.GetPaymentStatusAsync(orderId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get payment status for order: {OrderId}", orderId);
                return StatusCode(500, new { message = "Internal server error", error = ex.Message });
            }
        }

        /// <summary>
        /// 获取支持的支付方式
        /// </summary>
        [HttpGet("methods")]
        [AllowAnonymous]
        public ActionResult<IEnumerable<object>> GetPaymentMethods()
        {
            var paymentMethods = Enum.GetValues<PaymentMethod>()
                .Select(pm => new
                {
                    Value = pm.ToString(),
                    Name = pm switch
                    {
                        PaymentMethod.CreditCard => "信用卡",
                        PaymentMethod.DebitCard => "借记卡",
                        PaymentMethod.PayPal => "PayPal",
                        PaymentMethod.Alipay => "支付宝",
                        PaymentMethod.WeChatPay => "微信支付",
                        PaymentMethod.BankTransfer => "银行转账",
                        PaymentMethod.Cash => "现金",
                        PaymentMethod.Other => "其他",
                        _ => pm.ToString()
                    },
                    Description = pm switch
                    {
                        PaymentMethod.CreditCard => "使用信用卡支付",
                        PaymentMethod.DebitCard => "使用借记卡支付",
                        PaymentMethod.PayPal => "使用PayPal账户支付",
                        PaymentMethod.Alipay => "使用支付宝支付",
                        PaymentMethod.WeChatPay => "使用微信支付",
                        PaymentMethod.BankTransfer => "银行转账支付",
                        PaymentMethod.Cash => "货到付款",
                        PaymentMethod.Other => "其他支付方式",
                        _ => "未知支付方式"
                    }
                });

            return Ok(paymentMethods);
        }

        /// <summary>
        /// 发起支付
        /// </summary>
        [HttpPost("pay")]
        public async Task<IActionResult> Pay(PaymentDto dto)
        {
            if (CurrentUserId == null)
                return BadRequest("Invalid user");

            // 直接用 CurrentUserId.Value
            // ...
            return Ok();
        }
    }
}
