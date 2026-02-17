using ERMS.Services.Interfaces;
using System.Net;
using System.Net.Mail;
using System.Collections.Concurrent;

namespace ERMS.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailService> _logger;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;

        // ✅ Rate limiting: Track sent emails
        private static readonly ConcurrentQueue<DateTime> _sentEmails = new ConcurrentQueue<DateTime>();
        private static readonly SemaphoreSlim _emailSemaphore = new SemaphoreSlim(1, 1);

        // ✅ Configuration: Adjust these based on your needs
        private const int MAX_EMAILS_PER_MINUTE = 20;
        private const int MAX_EMAILS_PER_HOUR = 100;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;

            // Load SMTP settings from appsettings.json
            _smtpHost = _configuration["EmailSettings:SmtpHost"];
            _smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            _smtpUsername = _configuration["EmailSettings:SmtpUsername"];
            _smtpPassword = _configuration["EmailSettings:SmtpPassword"];
            _fromEmail = _configuration["EmailSettings:FromEmail"];
            _fromName = _configuration["EmailSettings:FromName"];
        }

        private async Task<bool> CheckRateLimitAsync()
        {
            await _emailSemaphore.WaitAsync();
            try
            {
                var now = DateTime.UtcNow;

                // Remove emails older than 1 hour
                while (_sentEmails.TryPeek(out var oldestEmail) &&
                       (now - oldestEmail).TotalHours > 1)
                {
                    _sentEmails.TryDequeue(out _);
                }

                var emailsInLastMinute = _sentEmails.Count(e => (now - e).TotalMinutes <= 1);
                var emailsInLastHour = _sentEmails.Count();

                if (emailsInLastMinute >= MAX_EMAILS_PER_MINUTE)
                {
                    _logger.LogWarning($"Rate limit exceeded: {emailsInLastMinute} emails sent in the last minute");
                    return false;
                }

                if (emailsInLastHour >= MAX_EMAILS_PER_HOUR)
                {
                    _logger.LogWarning($"Rate limit exceeded: {emailsInLastHour} emails sent in the last hour");
                    return false;
                }

                _sentEmails.Enqueue(now);
                return true;
            }
            finally
            {
                _emailSemaphore.Release();
            }
        }

        public async Task<bool> SendPasswordEmailAsync(string toEmail, string recipientName, string username, string password, bool isManager)
        {
            // ✅ Check rate limit before sending
            if (!await CheckRateLimitAsync())
            {
                _logger.LogWarning($"Email to {toEmail} blocked due to rate limiting");
                return false;
            }

            try
            {
                var subject = "Welcome to Employee Management System - Your Login Credentials";
                var roleTitle = isManager ? "Manager" : "Employee";

                var baseUrl = _configuration["AppSettings:BaseUrl"] ?? "https://localhost:7280";
                var loginUrl = $"{baseUrl}/Auth/Login";

                var body = $@"
                    <html>
                    <head>
                        <style>
                            body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                            .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                            .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
                            .content {{ background-color: #f9f9f9; padding: 30px; margin-top: 20px; }}
                            .credentials {{ background-color: #fff; border-left: 4px solid #4CAF50; padding: 15px; margin: 20px 0; }}
                            .warning {{ background-color: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 20px 0; }}
                            .footer {{ text-align: center; margin-top: 30px; font-size: 12px; color: #666; }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <h1>Welcome to the Team!</h1>
                            </div>
                            <div class='content'>
                                <h2>Hello {recipientName},</h2>
                                <p>Your account has been created in our Employee Management System with the role of <strong>{roleTitle}</strong>.</p>
                                
                                <div class='credentials'>
                                    <h3>Your Login Credentials:</h3>
                                    <p><strong>Username:</strong> {username}</p>
                                    <p><strong>Temporary Password:</strong> {password}</p>
                                    <p><strong>Login URL:</strong> <a href='{loginUrl}'>{loginUrl}</a></p>
                                </div>

                                <div class='warning'>
                                    <h3>⚠️ Important Security Notice:</h3>
                                    <p>This is a temporary password. For security reasons, please change your password immediately after your first login.</p>
                                    <p>Do not share your credentials with anyone.</p>
                                </div>

                                <p>If you have any questions or need assistance, please contact your manager or HR department.</p>
                                
                                <p>Best regards,<br>Human Resources Team</p>
                            </div>
                            <div class='footer'>
                                <p>This is an automated message. Please do not reply to this email.</p>
                                <p>&copy; {DateTime.Now.Year} Employee Management System. All rights reserved.</p>
                            </div>
                        </div>
                    </body>
                    </html>
                ";

                using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    smtpClient.Timeout = 30000; // 30 seconds

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_fromEmail, _fromName),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(toEmail);

                    await smtpClient.SendMailAsync(mailMessage);
                    _logger.LogInformation($"Welcome email sent successfully to {toEmail}");
                    return true;
                }
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError($"SMTP error sending email to {toEmail}: {smtpEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Email sending failed to {toEmail}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendPasswordResetEmailAsync(string toEmail, string resetLink, string fullName)
        {
            // ✅ Check rate limit before sending
            if (!await CheckRateLimitAsync())
            {
                _logger.LogWarning($"Password reset email to {toEmail} blocked due to rate limiting");
                return false;
            }

            try
            {
                var subject = "Password Reset Request - ERMS";
                var body = $@"
<!DOCTYPE html>
<html>
<head>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; text-align: center; border-radius: 5px 5px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 5px 5px; }}
        .button {{ display: inline-block; padding: 12px 30px; background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #666; }}
        .link-box {{ word-break: break-all; background: white; padding: 10px; border-left: 4px solid #667eea; margin: 15px 0; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h2>Password Reset Request</h2>
        </div>
        <div class='content'>
            <p>Hello {fullName},</p>
            <p>We received a request to reset your password for your ERMS account. Click the button below to reset your password:</p>
            <p style='text-align: center;'>
                <a href='{resetLink}' class='button'>Reset Password</a>
            </p>
            <p>Or copy and paste this link into your browser:</p>
            <div class='link-box'>{resetLink}</div>
            <p><strong>This link will expire in 1 hour.</strong></p>
            <p>If you didn't request a password reset, please ignore this email or contact your administrator if you have concerns.</p>
            <p>Best regards,<br>ERMS Team</p>
        </div>
        <div class='footer'>
            <p>This is an automated email. Please do not reply to this message.</p>
            <p>&copy; {DateTime.Now.Year} Employee Management System. All rights reserved.</p>
        </div>
    </div>
</body>
</html>";

                using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    smtpClient.Timeout = 30000; // 30 seconds

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_fromEmail, _fromName),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(toEmail);

                    await smtpClient.SendMailAsync(mailMessage);
                    _logger.LogInformation($"Password reset email sent successfully to {toEmail}");
                    return true;
                }
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError($"SMTP error sending password reset email to {toEmail}: {smtpEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Password reset email sending failed to {toEmail}: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            // ✅ Check rate limit before sending
            if (!await CheckRateLimitAsync())
            {
                _logger.LogWarning($"Email to {toEmail} blocked due to rate limiting");
                return false;
            }

            try
            {
                using (var smtpClient = new SmtpClient(_smtpHost, _smtpPort))
                {
                    smtpClient.EnableSsl = true;
                    smtpClient.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                    smtpClient.Timeout = 30000; // 30 seconds

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_fromEmail, _fromName),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(toEmail);

                    await smtpClient.SendMailAsync(mailMessage);
                    _logger.LogInformation($"Email sent successfully to {toEmail}");
                    return true;
                }
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError($"SMTP error sending email to {toEmail}: {smtpEx.Message}");
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Email sending failed to {toEmail}: {ex.Message}");
                return false;
            }
        }
    }
}