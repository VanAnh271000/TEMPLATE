using Application.DTOs.Configuration;
using Application.DTOs.Metrics;
using Application.DTOs.Notification;
using Application.Interfaces.Services.Notification.Senders;
using System.Diagnostics;
using System.Net.Http.Json;

namespace Infrastructure.Notifications
{
    public class SmsSender : ISmsSender
    {
        private readonly HttpClient _httpClient;
        private readonly SmsConfiguration _smsConfig;
        public SmsSender(SmsConfiguration smsConfig, HttpClient httpClient)
        {
            _smsConfig = smsConfig;
            _httpClient = httpClient;
        }

        public async Task SendAsync(SmsNotification message)
        {
            var sw = Stopwatch.StartNew();

            NotificationMetrics.SendTotal.Add(1,
                new KeyValuePair<string, object?>[]
                {
                    new("channel", "sms")
                });

            try
            {
                var payload = new
                {
                    to = message.PhoneNumber,
                    message = message.Message,
                    sender = _smsConfig.SenderId
                };

                using var request = new HttpRequestMessage(
                    HttpMethod.Post,
                    _smsConfig.ApiUrl)
                {
                    Content = JsonContent.Create(payload)
                };
                request.Headers.Add(
                "Authorization",
                $"Bearer {_smsConfig.ApiKey}");

                var response = await _httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();
            }
            catch (Exception)
            {
                NotificationMetrics.SendFailed.Add(1,
                    new KeyValuePair<string, object?>[] { new("channel", "sms") });
                throw;
            }
            finally
            {
                sw.Stop();

                NotificationMetrics.SendDuration.Record(
                    sw.Elapsed.TotalSeconds,
                    new KeyValuePair<string, object?>[] { new("channel", "sms") });
            }
        }
    }
}
