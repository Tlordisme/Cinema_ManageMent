using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using CM.ApplicationService.Payment.Abstracts;
//using CM.ApplicationService.Payment.ZaloPayment.Config;
//using CM.ApplicationService.Payment.ZaloPayment.Request;
using CM.Domain.Payment;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace CM.ApplicationService.Payment.Implements
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _config;
        //private readonly ZaloPayConfig zaloPayConfig;

        public PaymentService(IConfiguration config)
        {
            _config = config;
            //zaloPayConfig = config.GetSection(ZaloPayConfig.ConfigName).Get<ZaloPayConfig>();
        }

        public string CreatePaymentUrl(HttpContext context, VnPayRequest request)
        {
            var tick = DateTime.Now.Ticks.ToString();
            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", _config["VnPay:Version"]);
            vnpay.AddRequestData("vnp_Command", _config["VnPay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", (request.Amount * 100).ToString());

            vnpay.AddRequestData("vnp_CreateDate", request.CreateDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", _config["VnPay:Locale"]);

            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + request.TicketId);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", _config["VnPay:PaymentBackUrl"]);
            vnpay.AddRequestData("vnp_TxnRef", tick);

            var paymentUrl = vnpay.CreateRequestUrl(
                _config["VnPay:BaseUrl"],
                _config["VnPay:HashSecret"]
            );
            return paymentUrl;
        }

        public VnPayResponse PaymentExcute(IQueryCollection collections)
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



        //public (bool Success, string Result) CreateOrder(long amount, string appTransId, string description)
        //{
        //    try
        //    {
        //        var appTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        //        var request = new CreateZaloPayRequest(
        //            zaloPayConfig.AppId,
        //            zaloPayConfig.AppUser,
        //            appTime,
        //            amount,
        //            appTransId,
        //            "zalopayapp",
        //            description
        //        );

        //        // Generate signature
        //        request.MakeSignature(zaloPayConfig.Key1);

        //        var (success, result) = request.GetLink(zaloPayConfig.PaymentUrl);

        //        if (success)
        //        {
        //            return (true, result); // Return payment URL
        //        }
        //        else
        //        {
        //            return (false, $"ZaloPay Error: {result}");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return (false, $"Error: {ex.Message}");
        //    }

        //}
    }
}
