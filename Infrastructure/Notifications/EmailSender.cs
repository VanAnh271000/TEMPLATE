using Application.DTOs.Commons;
using Application.DTOs.Configuration;
using Application.DTOs.Notification;
using Application.Interfaces.Services.Notification.Senders;
using MailKit.Net.Smtp;
using MimeKit;
using Serilog;
using Shared.Results;
using System.Diagnostics;

namespace Infrastructure.Notifications
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly EmailConfiguration _emailConfig;
        public SmtpEmailSender(EmailConfiguration emailConfig)
        {
            _emailConfig = emailConfig;
        }
        
        public async Task SendAsync(EmailNotification message)
        {
            var emailMessage = CreateEmailMessage(message);
            await Send(emailMessage);
        }

        #region Private Methods
        private MimeMessage CreateEmailMessage(EmailNotification message)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.ApplicationName, _emailConfig.From));
            emailMessage.To.AddRange(message.To);
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = $"{message.Content}<br>{_emailConfig.ApplicationName}" };
            return emailMessage;
        }

        private async Task<ServiceResult> Send(MimeMessage mailMessage)
        {
            var sw = Stopwatch.StartNew();

            NotificationMetrics.SendTotal.Add(1,
                new KeyValuePair<string, object?>[]
                {
                    new("channel", "email")
                });
            using var client = new SmtpClient();
            try
            {
                client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
                client.AuthenticationMechanisms.Remove("XOAUTH2");
                client.Authenticate(_emailConfig.UserName, _emailConfig.Password);

                await client.SendAsync(mailMessage);
                return ServiceResult.Success();
            }
            catch (Exception ex)
            {
                NotificationMetrics.SendFailed.Add(1,
                    new KeyValuePair<string, object?>[] { new("channel", "email") });
                Log.Error(ex, "Email could not be sent.");
                return ServiceResult.InternalServerError("Email could not be sent.");
            }
            finally
            {
                client.Disconnect(true);
                client.Dispose();
                sw.Stop();

                NotificationMetrics.SendDuration.Record(
                    sw.Elapsed.TotalSeconds,
                    new KeyValuePair<string, object?>[] { new("channel", "email") });
            }
        }

        #endregion
    }
}
