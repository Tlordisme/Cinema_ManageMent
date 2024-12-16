using CloudinaryDotNet;
using CM.ApplicationService.Payment.Abstracts;
using CM.ApplicationService.Payment.Implements;
using CM.Domain.Payment;
using CM.Domain.Ticket;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CM_API.Controllers
{
    [Route("api/payment")]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        // Phương thức này xử lý việc tạo URL thanh toán
        [HttpPost("create-payment-url")]
        public IActionResult CreatePaymentUrl([FromForm] int ticket)
        {
            _logger.LogInformation("Bắt đầu xử lý tạo URL thanh toán cho ticket ID: {TicketId}", ticket);

            try
            {
                // Tạo URL thanh toán
                var paymentUrl = _paymentService.CreatePaymentUrl(HttpContext, ticket);
                _logger.LogInformation("URL thanh toán được tạo thành công cho ticket ID: {TicketId}, URL: {PaymentUrl}", ticket, paymentUrl);

                return Ok(new { paymentUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo URL thanh toán cho ticket ID: {TicketId}", ticket);
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        // Phương thức này xử lý phản hồi thanh toán từ VnPay
        [HttpGet("PaymentCallbackVnpay")]
        public async Task<IActionResult> VnPayResponseAsync()
        {
            _logger.LogInformation("Bắt đầu xử lý phản hồi thanh toán từ VnPay.");

            try
            {
                // Xử lý phản hồi từ VnPay
                var vnPayResponse = await _paymentService.PaymentExcute(Request.Query);

                if (vnPayResponse == null || vnPayResponse.VnPayResponseCode != "00")
                {
                    _logger.LogWarning("Thanh toán thất bại. Mã phản hồi VnPay: {ResponseCode}", vnPayResponse?.VnPayResponseCode);
                    return BadRequest("Thanh toán thất bại.");
                }
                else
                {
                    _logger.LogInformation("Thanh toán thành công. Mã phản hồi VnPay: {ResponseCode}, Data: {ResponseData}", vnPayResponse.VnPayResponseCode, vnPayResponse);
                    return Ok(new { Message = "Thanh toán thành công", Data = vnPayResponse });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý phản hồi thanh toán từ VnPay.");
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }
    }
}