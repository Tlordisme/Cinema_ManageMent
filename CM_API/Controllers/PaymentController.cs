using CloudinaryDotNet;
using CM.ApplicationService.Payment.Abstracts;
using CM.ApplicationService.Payment.Implements;
using CM.Domain.Payment;
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
        public IActionResult CreatePaymentUrl([FromBody] VnPayRequest request)
        {
            if (request == null || request.Amount <= 0)
            {
                return BadRequest("Thông tin thanh toán không hợp lệ.");
            }

            try
            {
                // Tạo URL thanh toán
                var paymentUrl = _paymentService.CreatePaymentUrl(HttpContext, request);
                return Ok(new { Url = paymentUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
            }
        }

        // Phương thức này xử lý phản hồi thanh toán từ VnPay
        [HttpGet("PaymentCallbackVnpay")]
        public IActionResult VnPayResponse()
        {
            try
            {
                // Xử lý phản hồi từ VnPay
                var vnPayResponse = _paymentService.PaymentExcute(Request.Query);

             
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


        //[HttpPost("create-payment")]
        //public IActionResult CreatePayment(long amount, string appTransId, string description)
        //{
        //    var (success, result) = _paymentService.CreateOrder(amount, appTransId, description);

        //    if (success)
        //    {
        //        return Ok(new { PaymentUrl = result });
        //    }
        //    else
        //    {
        //        return BadRequest(new { Message = result });
        //    }
        //}

        //// API endpoint để nhận kết quả thanh toán từ ZaloPay
        //[HttpGet("zalopay-return")]
        //public IActionResult ZaloPayReturn(string orderId, string status, string errorMessage)
        //{
        //    // Xử lý kết quả trả về từ ZaloPay (nếu cần)
        //    if (status == "1")
        //    {
        //        return Ok(new { Message = "Thanh toán thành công", OrderId = orderId });
        //    }
        //    else
        //    {
        //        return BadRequest(new { Message = $"Thanh toán thất bại: {errorMessage}" });
        //    }
        //}

        //// API endpoint để nhận IPN (Instant Payment Notification) từ ZaloPay
        //[HttpPost("zalopay-ipn")]
        //public IActionResult ZaloPayIpn([FromBody] ZaloPayIpnData ipnData)
        //{
        //    // Xử lý IPN (nếu cần)
        //    return Ok(new { Message = "IPN nhận thành công", Data = ipnData });
        //}

        //public class ZaloPayIpnData
        //{
        //    public string OrderId { get; set; } = string.Empty;
        //    public string AppTransId { get; set; } = string.Empty;
        //    public int ReturnCode { get; set; }
        //    public string ErrorMessage { get; set; } = string.Empty;
        //}
    }
}
