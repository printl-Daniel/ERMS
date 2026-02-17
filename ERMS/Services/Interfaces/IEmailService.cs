namespace ERMS.Services.Interfaces
{
    public interface IEmailService
    {

        Task<bool> SendPasswordEmailAsync(string toEmail, string recipientName, string username, string password, bool isManager);
        Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetLink, string fullName);
        Task<bool> SendEmailAsync(string toEmail, string subject, string body);
    }
}

