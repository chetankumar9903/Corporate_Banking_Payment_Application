using Corporate_Banking_Payment_Application.Services.IService;
using System.Net;
using System.Net.Mail;

namespace Corporate_Banking_Payment_Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {

            var host = _config["SmtpSettings:Host"];
            var port = int.Parse(_config["SmtpSettings:Port"]);
            var username = _config["SmtpSettings:Username"];
            var password = _config["SmtpSettings:Password"];
            var enableSsl = bool.Parse(_config["SmtpSettings:EnableSsl"]);

            var mailMessage = new MailMessage
            {
                From = new MailAddress(username, "Corporate Banking App"),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };
            mailMessage.To.Add(toEmail);


            using (var smtpClient = new SmtpClient(host, port))
            {
                smtpClient.Credentials = new NetworkCredential(username, password);
                smtpClient.EnableSsl = enableSsl;

                await smtpClient.SendMailAsync(mailMessage);
            }
        }
    }
}