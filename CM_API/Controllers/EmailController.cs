using Microsoft.AspNetCore.Mvc;
using MimeKit;

using System.Net.Mail;
using MailKit.Net.Smtp;
using System.Net;


namespace CM_API.Controllers
{
    [Route("api/[controller]")]
    public class EmailController : Controller
    {
        [HttpPost("SendEmail")]
        public IActionResult SendEmail(string body)
        {
            string fromEmail = "thanh0220166@huce.edu.vn";
            string fromPassword = "uprqkyoenyftuzcu";
            
            var message = new MailMessage();
            message.From = new MailAddress(fromEmail);
            message.Subject = "Test";
            message.To.Add(new MailAddress("thanh218203@gmail.com"));
            message.Body = "<html><body> Test Body </body><html>";
            message.IsBodyHtml = true;

            var smtpClient = new System.Net.Mail.SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(fromEmail, fromPassword),
                EnableSsl = true,
            };
            smtpClient.Send(message);
            return Ok();
        }
    }
}
