using CM.ApplicationService.Payment.Abstracts;
using CM.Domain.Auth;
using Microsoft.AspNetCore.Mvc;
using CM.ApplicantService.Auth.Permission.Abstracts;

[Route("api/payment")]
public class PaymentController : Controller
{
    private readonly IPaymentService _paymentService;
    private readonly ILogger<PaymentController> _logger;
    private readonly IPermissionService _permissionService;

    public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger, IPermissionService permissionService)
    {
        _paymentService = paymentService;
        _logger = logger;
        _permissionService = permissionService;

    }

    // Phương thức này xử lý việc tạo URL thanh toán
    [HttpPost("create-payment-url")]
    public IActionResult CreatePaymentUrl([FromForm] int ticket)
    {
        // Kiểm tra quyền
        var userId = int.Parse(User.FindFirst("Id")?.Value);
        if (!_permissionService.CheckPermission(userId, "CreatePayment"))
        {
            return Unauthorized("You do not have permission to create payments.");
        }

        try
        {
            // Gọi service để tạo URL thanh toán
            var paymentUrl = _paymentService.CreatePaymentUrl(HttpContext, ticket);

            return Ok(new { paymentUrl });
        }
        catch (Exception ex)
        {
            // Không ghi log ở controller, chỉ trả về lỗi
            return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
        }
    }

    // Phương thức này xử lý phản hồi thanh toán từ VnPay
    [HttpGet("PaymentCallbackVnpay")]
    public async Task<IActionResult> VnPayResponseAsync()
    {
        // Kiểm tra quyền
        var userId = int.Parse(User.FindFirst("Id")?.Value);
        if (!_permissionService.CheckPermission(userId, "ViewPayment"))
        {
            return Unauthorized("You do not have permission to view payments.");
        }

        try
        {
            // Gọi service để xử lý phản hồi thanh toán từ VnPay
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
            // Không ghi log ở controller, chỉ trả về lỗi
            return StatusCode(500, $"Lỗi hệ thống: {ex.Message}");
        }
    }
}