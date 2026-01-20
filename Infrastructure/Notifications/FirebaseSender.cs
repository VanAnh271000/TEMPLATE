using Application.DTOs.Configuration;
using Application.DTOs.Metrics;
using Application.DTOs.Notification;
using Application.Interfaces.Services.Notification.Senders;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using Serilog;
using System.Diagnostics;

namespace Infrastructure.Notifications
{
    public class FirebaseSender : IFirebaseSender
    {
        private readonly FirebaseConfiguration _firebaseConfig;
        private static FirebaseApp _firebaseApp;

        public FirebaseSender(FirebaseConfiguration firebaseConfig)
        {
            _firebaseConfig = firebaseConfig;
            InitializeFirebase();
        }

        private void InitializeFirebase()
        {
            if (_firebaseApp == null)
            {
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), _firebaseConfig.ServiceAccountKeyPath);

                var json = File.ReadAllText(filePath);

                dynamic config = JsonConvert.DeserializeObject(json);
                string projectId = config.project_id;

                GoogleCredential credential;
                using (var stream = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(json)))
                {
                    credential = GoogleCredential.FromStream(stream);
                }

                var options = new AppOptions()
                {
                    Credential = credential,
                    ProjectId = projectId
                };
                _firebaseApp = FirebaseApp.Create(options);
                Log.Information($"Firebase initialized for Project: {projectId}");
            }
        }

        public async Task SendAsync(FirebaseNotification notification)
        {
            var sw = Stopwatch.StartNew();

            NotificationMetrics.SendTotal.Add(1,
                new KeyValuePair<string, object?>[]
                {
                    new("channel", "firebase")
                });

            try
            {
                var message = new Message
                {
                    Token = notification.DeviceToken,
                    Notification = new Notification
                    {
                        Title = notification.Title,
                        Body = notification.Body
                    },
                    Data = notification.Data
                };

                await FirebaseMessaging.DefaultInstance.SendAsync(message);
            }
            catch (Exception)
            {
                NotificationMetrics.SendFailed.Add(1,
                    new KeyValuePair<string, object?>[] { new("channel", "firebase") });
                throw;
            }
            finally
            {
                sw.Stop();

                NotificationMetrics.SendDuration.Record(
                    sw.Elapsed.TotalSeconds,
                    new KeyValuePair<string, object?>[] { new("channel", "firebase") });
            }
        }
    }
}
