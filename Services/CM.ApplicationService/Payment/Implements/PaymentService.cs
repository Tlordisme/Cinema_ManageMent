using CM.ApplicationService.Common;
using CM.ApplicationService.Notification.Abstracts;
using CM.ApplicationService.Payment.Abstracts;
using CM.Domain.Payment;
using CM.Domain.Seat;
using CM.Domain.Ticket;
using CM.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

public class PaymentService : ServiceBase, IPaymentService
{
    private readonly IConfiguration _config;
    private readonly INotificationService _notificationService;
    private readonly ILogger<PaymentService> _logger;

    public PaymentService(
        CMDbContext dbContext,
        ILogger<ServiceBase> logger,
        IConfiguration config,
        INotificationService notificationService,
        ILogger<PaymentService> loggerService
    )
        : base(logger, dbContext)
    {
        _config = config;
        _notificationService = notificationService;
        _logger = loggerService;
    }

    public string CreatePaymentUrl(HttpContext context, int ticketId)
    {
        _logger.LogInformation("Bắt đầu tạo URL thanh toán cho vé ID: {TicketId}", ticketId);

        var ticket = _dbContext.Tickets.FirstOrDefault(t => t.Id == ticketId);
        if (ticket == null || ticket.Status != TicketStatus.Pending)
        {
            _logger.LogWarning("Vé không hợp lệ hoặc không phải trạng thái 'Pending'. Vé ID: {TicketId}", ticketId);
            return "Vé không hợp lệ";
        }

        var tick = ticket.Id.ToString();
        var vnpay = new VnPayLibrary();

        vnpay.AddRequestData("vnp_Version", _config["VnPay:Version"]);
        vnpay.AddRequestData("vnp_Command", _config["VnPay:Command"]);
        vnpay.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]);
        vnpay.AddRequestData("vnp_Amount", (ticket.TotalPrice * 100).ToString("0"));
        vnpay.AddRequestData("vnp_CreateDate", ticket.BookingDate.ToString("yyyyMMddHHmmss"));
        vnpay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"]);
        vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
        vnpay.AddRequestData("vnp_Locale", _config["VnPay:Locale"]);
        vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toán đơn hàng: {ticket.Id}");
        vnpay.AddRequestData("vnp_OrderType", "other"); // default value: other
        vnpay.AddRequestData("vnp_ReturnUrl", _config["VnPay:PaymentBackUrl"]);
        vnpay.AddRequestData("vnp_TxnRef", tick);

        var paymentUrl = vnpay.CreateRequestUrl(
            _config["VnPay:BaseUrl"],
            _config["VnPay:HashSecret"]
        );

        _logger.LogInformation("URL thanh toán được tạo thành công cho vé ID: {TicketId}, URL: {PaymentUrl}", ticketId, paymentUrl);
        return paymentUrl;
    }

    public async Task<VnPayResponse> PaymentExcute(IQueryCollection collections)
    {
        _logger.LogInformation("Bắt đầu xử lý phản hồi thanh toán từ VnPay.");

        var vnpay = new VnPayLibrary();
        foreach (var (key, value) in collections)
        {
            if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
            {
                vnpay.AddResponseData(key, value.ToString());
            }
        }

        var vnp_orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
        var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
        var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
        var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
        var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");

        bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _config["VnPay:HashSecret"]);
        if (!checkSignature)
        {
            _logger.LogWarning("Lỗi chữ ký khi xử lý phản hồi thanh toán. Mã phản hồi: {ResponseCode}", vnp_ResponseCode);
            return new VnPayResponse { Success = false };
        }

        var ticket = await _dbContext.Tickets.FirstOrDefaultAsync(t => t.Id == vnp_orderId);

        if (vnp_ResponseCode == "00")
        {
            // Cập nhật trạng thái vé và ghế
            ticket.Status = TicketStatus.Paid;
            var seats = await _dbContext
                .TicketSeats.Where(ts => ts.TicketId == vnp_orderId)
                .Select(ts => ts.Seat)
                .ToListAsync();

            foreach (var seat in seats)
            {
                seat.Status = SeatStatus.Booked;
            }

            await _notificationService.SendNotification(ticket.Id);
            await _dbContext.SaveChangesAsync();

            _logger.LogInformation("Thanh toán thành công cho vé ID: {TicketId}, Mã giao dịch VnPay: {TransactionId}", vnp_orderId, vnp_TransactionId);

            return new VnPayResponse
            {
                Success = true,
                OrderDescription = vnp_OrderInfo,
                OrderId = vnp_orderId.ToString(),
                TransactionId = vnp_TransactionId.ToString(),
                Token = vnp_SecureHash,
                VnPayResponseCode = vnp_ResponseCode,
            };
        }
        else
        {
            // Thanh toán thất bại
            ticket.Status = TicketStatus.Canceled;
            var seats = await _dbContext
                .TicketSeats.Where(ts => ts.TicketId == vnp_orderId)
                .Select(ts => ts.Seat)
                .ToListAsync();

            foreach (var seat in seats)
            {
                seat.Status = SeatStatus.Available;
            }

            await _dbContext.SaveChangesAsync();

            _logger.LogWarning("Thanh toán thất bại cho vé ID: {TicketId}, Mã phản hồi VnPay: {ResponseCode}", vnp_orderId, vnp_ResponseCode);

            return new VnPayResponse
            {
                Success = false,
                OrderDescription = vnp_OrderInfo,
                OrderId = vnp_orderId.ToString(),
                TransactionId = vnp_TransactionId.ToString(),
                Token = vnp_SecureHash,
                VnPayResponseCode = vnp_ResponseCode,
            };
        }
    }
}