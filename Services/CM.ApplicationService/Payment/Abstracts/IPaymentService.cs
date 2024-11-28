using CM.Domain.Payment;
using CM.Dtos.Payment;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CM.ApplicationService.Payment.Abstracts
{
    public interface IPaymentService
    {
        string CreatePaymentUrl(HttpContext context, VnPayRequest request);
        //Task<string> CreatePaymentUrl(HttpContext context, VnPayRequest request, int ticketId);
        VnPayResponse PaymentExcute(IQueryCollection collections);

        //(bool Success, string Result) CreateOrder(long amount, string appTransId, string description);
    }
}
