namespace Application.DTOs.Configuration
{
    public class RedisConfiguration
    {
        public string ConnectionString { get; set; } = string.Empty;    
        public string InstanceName { get; set; } = string.Empty;
    }
}
