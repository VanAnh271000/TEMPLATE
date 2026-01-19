using Application.DTOs.Configuration;
using Application.Interfaces.Services.Notification;
using Application.Interfaces.Services.Notification.Senders;
using Application.Services.Notification;
using Infrastructure.BackgroundJobs.Jobs;
using Infrastructure.Notifications;

namespace API.Installers
{
    public class NotificationInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<INotificationDispatcher, NotificationDispatcher>();
            services.AddScoped<ISmsSender, SmsSender>();
            services.AddScoped<IFirebaseSender, FirebaseSender>();
            services.AddScoped<IEmailSender, EmailSender>();

            services.AddTransient<NotificationJob>();
        }
    }
    public class EmailInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var emailConfig = configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
            services.AddSingleton(emailConfig);
        }
    }
    public class SmsInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var smsConfig = configuration.GetSection("SmsConfiguration").Get<SmsConfiguration>();
            services.AddSingleton(smsConfig);
        }
    }

    public class FirebaseInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var firebaseConfig = configuration.GetSection("FirebaseConfiguration").Get<FirebaseConfiguration>();
            services.AddSingleton(firebaseConfig);
        }
    }
}
