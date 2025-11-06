namespace Corporate_Banking_Payment_Application.Services.IService
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string body);
    }
}