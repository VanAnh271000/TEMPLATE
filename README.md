# TEMPLATE
A production-ready .NET Web API template with authentication, authorization, background jobs, and common best practices.

# Backend Template (.NET)

Enterprise-ready backend template with:

- Clean Architecture
- API Versioning & Documentation
- Monitoring & Observability
- Notification System
- Caching (Memory + Redis)

## Documentation

- [Overview](docs/overview.md)
- [Architecture](docs/architecture.md)
- [API Design](docs/api-design.md)
- [API Documentation](docs/api-documentation.md)
- [Monitoring & Observability](docs/monitoring-observability.md)
- [Notification System](docs/notification-system.md)
- [Caching & Performance](docs/caching-performance.md)

## Getting Started

```bash
docker run -d  -p 9090:9090 -v %cd%/prometheus.yml:/etc/prometheus/prometheus.yml --name prometheus prom/prometheus
```
Access Prometheus UI: http://localhost:9090
Check scrape status: Status ? Targets
### Running Grafana
#### 1. Start Grafana
```bash
docker run -d -p 3000:3000 --name grafana grafana/grafana
```
- Grafana UI: http://localhost:3000
- Default login: Username - admin, Password: admin
#### 2. Add Prometheus Data Source
- Open Configuration ? Data Sources
- Click Add data source
- Select Prometheus
- URL: http://host.docker.internal:9090  
Recommended Grafana Queries
1. Request Rate (RPS): rate(http_server_request_duration_seconds_count[1m])
2. P95 Latency:
   ```promql
   histogram_quantile(
   0.95,
   rate(http_server_request_duration_seconds_bucket[5m])
   )
   ```
3. Error Rate (5xx)
   ```promql
     sum(rate(http_server_request_duration_seconds_count{http_response_status_code=~"5.."}[5m]))
     /
     sum(rate(http_server_request_duration_seconds_count[5m]))
   ```


