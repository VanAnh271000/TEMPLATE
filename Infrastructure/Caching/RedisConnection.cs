using Application.DTOs.Configuration;
using StackExchange.Redis;

namespace Infrastructure.Caching
{
    public class RedisConnection
    {
        private readonly Lazy<ConnectionMultiplexer> _connection;
        private readonly RedisConfiguration _config;
        public RedisConnection(RedisConfiguration config)
        {
            _config = config;
            _connection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(_config.ConnectionString));
        }
        public IDatabase Database => _connection.Value.GetDatabase();
        
        public IServer Server => _connection.Value.GetServer(_connection.Value.GetEndPoints().First());
    }
}
