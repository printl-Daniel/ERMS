using ERMS.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace ERMS.Services.Implementations
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;

            // Load SMTP settings from appsettings.json
            _smtpHost = _configuration["EmailSettings:SmtpHost"];
            _smtpPort = int.Parse(_configuration["EmailSettings:SmtpPort"]);
            _smtpUsername = _configuration["EmailSettings:SmtpUsername"];
            _smtpPassword = _configuration["EmailSettings:SmtpPassword"];
            _fromEmail = _configuration["EmailSettings:FromEmail"];
            _fromName = _configuration["EmailSettings:FromName"];
        }

        public async Task<bool> SendPasswordEmailAsync(string toEmail, string recipientName, string username, string password, bool isManager)
        {
            try
            {
                var subject = "Welcome to Employee Management System - Your Login Credentials";
                var roleTitle = isManager ? "Manager" : "Employee";

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
                                    <p><strong>Login URL:</strong> <a href='https://yourcompany.com/login'>https://yourcompany.com/login</a></p>
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

                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(_fromEmail, _fromName),
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(toEmail);

                    await smtpClient.SendMailAsync(mailMessage);
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log the error (in production, use a logging framework)
                Console.WriteLine($"Email sending failed: {ex.Message}");
                return false;
            }
        }
    }
}