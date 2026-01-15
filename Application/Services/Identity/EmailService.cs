using Application.DTOs.Identity;
using Application.Interfaces.Services.Identity;
using MailKit.Net.Smtp;
using MimeKit;
using Serilog;
using Shared.Results;

namespace Application.Services.Identity
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfiguration _emailConfig;
        public EmailService(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }

        private MimeMessage CreateEmailMessage(EmailMessage message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.ApplicationName, _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = $"{message.Content}<br>{_emailConfig.ApplicationName}" };
            return emailMessage;
        }

        public void SendEmail(EmailMessage message)
        {
            var emailMessage = CreateEmailMessage(message);
            Send(emailMessage);
        }

        private ServiceResult Send(MimeMessage mailMessage)
        {
            using var client = new SmtpClient();
            try
            {
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfig.UserName, _emailConfig.Password);

                client.Send(mailMessage);
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Email could not be sent.");
                return ServiceResult.InternalServerError("Email could not be sent.");
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
            }
        }
    }
}
