# Caching
This project uses **Redis** as a distributed cache to improve performance, reduce database load, and support horizontal scaling.  
Caching is implemented using the **Cache-Aside pattern**, fully decoupled from business logic via interfaces.  

---

## Architecture Overview
```text
Application
 └── DTOs
     └── CacheKeys
 └── Interface
     └── ICacheService
     └── CacheExtensions
Infrastructure
 └── Caching
     ├── RedisConnection
     └── RedisCacheService
     └── MemoryCacheService
```
Key Principles:
- Application layer depends only on ICacheService
- Infrastructure provides Redis implementation
- No Redis dependency leaks into Application
- Environment-based isolation using key prefixes
Packages:
```bash
dotnet add package StackExchange.Redis
```
Configuration:
```json
{
  "Redis": {
    "ConnectionString": "localhost:6379",
    "InstanceName": "local:"
  }
}
```

---

## Testing Strategy
Integration Tests 
- Use a dedicated Redis instance
- Use a separate InstanceName (e.g. test:)
- Validate TTL, prefix invalidation
```bash
docker run -d -p 6379:6379 redis:7
```
Check redis:
```bash
docker exec -it redis-test redis-cli
keys local:*
```