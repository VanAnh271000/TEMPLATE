using Application.DTOs.Configuration;

namespace API.Installers
{
    public class NotificationInstaller : IInstaller
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
            var emailConfig = configuration.GetSection("SmsConfiguration").Get<SmsConfiguration>();
            services.AddSingleton(emailConfig);
        }
    }

    public class FirebaseInstaller : IInstaller
    {
        public void InstallServices(IServiceCollection services, IConfiguration configuration)
        {
            var emailConfig = configuration.GetSection("FirebaseConfiguration").Get<FirebaseConfiguration>();
            services.AddSingleton(emailConfig);
        }
    }
}
