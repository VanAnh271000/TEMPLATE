# TEMPLATE
A production-ready .NET Web API template with authentication, authorization, background jobs, and common best practices.

## Tech Stack

- .NET 8 Web API
- Entity Framework Core
- SQL Server / PostgreSQL
- JWT Authentication
- Hangfire
- Serilog
- Swagger / OpenAPI

## Features

- Authentication & Authorization (JWT, Role-based)
- Global Exception Handling
- Logging with Serilog
- Monitoring & Observability
- Background Jobs with Hangfire
- API Versioning
- Swagger UI
- Health Checks
- Clean Architecture

## Project Structure

src/  
 ├── **Api**: Controllers, Middlewares  
 ├── **Application**: Business logic  
 ├── **Domain**: Entities, Interfaces  
 ├── **Infrastructure**: Database, External services  
 └── **Shared**


## Monitoring & Observability (Prometheus + Grafana)
This project includes **built-in observability** using **OpenTelemetry**, **Prometheus**, and **Grafana** to monitor application health, performance, and business metrics.
<img width="848" height="739" alt="image" src="https://github.com/user-attachments/assets/93273f2f-1c88-4500-85e1-70c25bebfa40" />
### Architecture Overview
.NET API  
└── /metrics (OpenTelemetry / Prometheus format)  
↓  
Prometheus (scraping & storage)
↓  
Grafana (visualization UI)  

---


### Metrics Endpoint
The API exposes metrics at: /metrics (Example (local): http://localhost:7230/metrics)

---


### Technology Stack

| Component     | Tool |
|---------------|------|
| Metrics       | OpenTelemetry |
| Scraping      | Prometheus |
| Visualization | Grafana (OSS – Free) |
| Logging       | Serilog + Seq |

---


### Running Prometheus (Docker)

#### 1. Create `prometheus.yml`

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
#### 2. Run Prometheus
```bash
docker run -d  -p 9090:9090 -v %cd%/prometheus.yml:/etc/prometheus/prometheus.yml --name prometheus prom/prometheus
```
Access Prometheus UI: http://localhost:9090
Check scrape status: Status → Targets
### Running Grafana
#### 1. Start Grafana
```bash
docker run -d -p 3000:3000 --name grafana grafana/grafana
```
- Grafana UI: http://localhost:3000
- Default login: Username - admin, Password: admin
#### 2. Add Prometheus Data Source
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
## Notification

The Notification System is a modular, extensible component designed to handle multi-channel notifications such as **Email**, **SMS**, and **Firebase Push**.  
It follows **Clean Architecture principles**, supports **background processing with Hangfire**, and provides **observability via metrics and logging**.

---

### Features

- Multi-channel notifications
  - Email (SMTP)
  - SMS (pluggable providers)
  - Firebase Push Notification
- Clean Architecture compliant
- Background processing using Hangfire
- Extensible channel-based design (Open–Closed Principle)
- Observability
  - Metrics (Prometheus)
  - Logging (Serilog + CorrelationId)
- Ready for Grafana dashboards & alerts

---

### Architecture Overview
Application  
│  
├── Interfaces  
│ ├── Notifications  
│ │ ├── INotificationService  
│ │ ├── INotificationChannel  
│ │ └── INotificationDispatcher    
│  
Infrastructure  
│  
├── Notifications  
│ ├── Channels  
│ │ ├── EmailNotificationChannel  
│ │ ├── EmailSender  
│ │ ├── SmsNotificationChannel  
│ │ ├── SmsSender  
│ │ └── FirebaseNotificationChannel  
│ │ └── FirebaseSender  
│ ├── Jobs  
│ │ └── NotificationJob  

---

### Design Principles
#### Why **Interface-based Channels** (not enum)?

- Avoids `switch/case` explosion
- Open for extension, closed for modification
- Each channel encapsulates its own logic
- Easy to enable/disable channels per environment
```csharp
public interface INotificationChannel
{
    bool CanHandle(NotificationMessage message);
    Task SendAsync(NotificationMessage message);
}
```
#### Notification Flow
- Application Service requests a notification
- NotificationJob is enqueued via Hangfire
- Job resolves all INotificationChannel implementations
- Each channel decides whether it can handle the message
- Metrics & logs are recorded per channel
```css
Controller / Service
        ↓
INotificationService
        ↓
Hangfire (Background Job)
        ↓
NotificationJob
        ↓
INotificationChannel (Email / SMS / Firebase)
```
#### Background Processing (Hangfire)
Notifications are executed asynchronously to avoid blocking API requests.  
Benefits:
- Better API performance
- Retry & failure handling
- Scalable background execution

---


### Metrics & Observability
Notification Metrics: Metrics are tracked using OpenTelemetry and exposed via /metrics for Prometheus.
```csharp
public static class NotificationMetrics
{
    public const string MeterName = "Template.Notification";

    public static readonly Counter<long> Sent =
        new Meter(MeterName).CreateCounter<long>("notification_sent_total");

    public static readonly Counter<long> Failed =
        new Meter(MeterName).CreateCounter<long>("notification_failed_total");
}
```
Example Metric Labels
- channel: email | sms | firebase
- status: success | failed
- provider: smtp | twilio | firebase

---


### Benefits
- Clean separation of concerns
- Easy to extend with new channels (Slack, WhatsApp, etc.)
- Production-ready observability
- Works seamlessly with background jobs
- Suitable for monolith or future microservice extraction

---


### Future Improvements
- Retry & dead-letter strategy per channel
- Notification templates & localization
- User preferences per channel
- Rate limiting per provider
- Distributed tracing (notification span)
