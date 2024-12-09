using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using Catel.Reflection;
using CM.ApplicationService.Common;
using CM.ApplicationService.Notification.Abstracts;
using CM.ApplicationService.Payment.Abstracts;
using CM.Domain.Payment;
using CM.Domain.Seat;
using CM.Domain.Showtime;
using CM.Domain.Ticket;
using CM.Dtos.Ticket;
using CM.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CM.ApplicationService.Payment.Implements
{
    public class PaymentService : ServiceBase, IPaymentService
    {
        private readonly IConfiguration _config;
        private readonly IEmailService _emailService;

        public PaymentService(
            CMDbContext dbContext,
            ILogger<ServiceBase> logger,
            IConfiguration config,
            IEmailService emailService
        )
            : base(logger, dbContext)
        {
            _config = config;
            _emailService = emailService;
        }

        public string CreatePaymentUrl(HttpContext context, int ticketId)
        {
            var ticket = _dbContext.Tickets.FirstOrDefault(t => t.Id == ticketId);
            if (ticket.Status != TicketStatus.Pending)
            {
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

            vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang: {ticket.Id}");
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", _config["VnPay:PaymentBackUrl"]);
            vnpay.AddRequestData("vnp_TxnRef", tick);

            var paymentUrl = vnpay.CreateRequestUrl(
                _config["VnPay:BaseUrl"],
                _config["VnPay:HashSecret"]
            );
            return paymentUrl;
        }

        public async Task<VnPayResponse> PaymentExcute(IQueryCollection collections)
        {
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
            bool checkSignature = vnpay.ValidateSignature(
                vnp_SecureHash,
                _config["VnPay:HashSecret"]
            );
            if (!checkSignature)
            {
                return new VnPayResponse { Success = false };
            }
            var ticketId = vnp_orderId;
            var ticket = await _dbContext.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId); // Sử dụng FirstOrDefaultAsync

            if (vnp_ResponseCode == "00")
            {
                // Cập nhật trạng thái vé và ghế
                ticket.Status = TicketStatus.Paid;
                var seats = await _dbContext
                    .TicketSeats.Where(ts => ts.TicketId == ticketId)
                    .Select(ts => ts.Seat)
                    .ToListAsync();
                foreach (var seat in seats)
                {
                    seat.Status = SeatStatus.Booked;
                }

                await _emailService.SendEmailAsync(ticket.Id); // Gọi phương thức bất đồng bộ
                await _dbContext.SaveChangesAsync(); // Sử dụng SaveChangesAsync để xử lý bất đồng bộ

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
                    .TicketSeats.Where(ts => ts.TicketId == ticketId)
                    .Select(ts => ts.Seat)
                    .ToListAsync();
                foreach (var seat in seats)
                {
                    seat.Status = SeatStatus.Available;
                }

                await _dbContext.SaveChangesAsync(); // Sử dụng SaveChangesAsync để xử lý bất đồng bộ

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
}
