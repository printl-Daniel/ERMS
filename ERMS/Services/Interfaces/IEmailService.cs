namespace ERMS.Services.Interfaces
{
    public interface IEmailService
    {
        Task<bool> SendPasswordEmailAsync(string toEmail, string recipientName, string username, string password, bool isManager);
    }
}
