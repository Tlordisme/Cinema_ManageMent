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

        public PaymentController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        // Phương thức này xử lý việc tạo URL thanh toán
        [HttpPost("create-payment-url")]
        public IActionResult CreatePaymentUrl([FromForm] int ticket)
        {


            try
            {
                // Tạo URL thanh toán
                var paymentUrl = _paymentService.CreatePaymentUrl(HttpContext, ticket);
                return Ok(new { paymentUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        // Phương thức này xử lý phản hồi thanh toán từ VnPay
        [HttpGet("PaymentCallbackVnpay")]
        public async Task<IActionResult> VnPayResponseAsync()
        {
            try
            {
                // Xử lý phản hồi từ VnPay
                var vnPayResponse = await _paymentService.PaymentExcute(Request.Query);

             
                if (vnPayResponse == null || vnPayResponse.VnPayResponseCode != "00")
                {
                        return BadRequest("Thanh toán thất bại.");
                    
                }
                else
                {
                    return Ok(new { Message = "Thanh toán thành công", Data = vnPayResponse });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }


       
    }
}
