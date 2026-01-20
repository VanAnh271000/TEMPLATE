# Monitoring & Observability (Prometheus + Grafana)
This project includes **built-in observability** using **OpenTelemetry**, **Prometheus**, and **Grafana** to monitor application health, performance, and business metrics.
<img width="848" height="739" alt="image" src="https://github.com/user-attachments/assets/93273f2f-1c88-4500-85e1-70c25bebfa40" />
## Architecture Overview
.NET API  
└── /metrics (OpenTelemetry / Prometheus format)  
↓  
Prometheus (scraping & storage)
↓  
Grafana (visualization UI)  

---


## Metrics Endpoint
The API exposes metrics at: /metrics (Example (local): http://localhost:7230/metrics)

---


## Technology Stack

| Component     | Tool |
|---------------|------|
| Metrics       | OpenTelemetry |
| Scraping      | Prometheus |
| Visualization | Grafana (OSS – Free) |
| Logging       | Serilog + Seq |

---


## Running Prometheus (Docker)

### 1. Create `prometheus.yml`

```yaml
global:
  scrape_interval: 5s

scrape_configs:
  - job_name: "template-api"
    metrics_path: /metrics
    scheme: https
    tls_config:
      insecure_skip_verify: true
    static_configs:
      - targets:
          - host.docker.internal:7230
```
Use http or **insecure_skip_verify: true* for local development.
In production, use https with a valid certificate.
### 2. Run Prometheus
```bash
docker run -d  -p 9090:9090 -v %cd%/prometheus.yml:/etc/prometheus/prometheus.yml --name prometheus prom/prometheus
```
Access Prometheus UI: http://localhost:9090
Check scrape status: Status → Targets
## Running Grafana
### 1. Start Grafana
```bash
docker run -d -p 3000:3000 --name grafana grafana/grafana
```
- Grafana UI: http://localhost:3000
- Default login: Username - admin, Password: admin
### 2. Add Prometheus Data Source
- Open Configuration → Data Sources
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