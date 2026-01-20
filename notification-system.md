# Notification

The Notification System is a modular, extensible component designed to handle multi-channel notifications such as **Email**, **SMS**, and **Firebase Push**.  
It follows **Clean Architecture principles**, supports **background processing with Hangfire**, and provides **observability via metrics and logging**.

---

## Features

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

## Architecture Overview
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

## Design Principles
### Why **Interface-based Channels** (not enum)?

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
### Notification Flow
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
### Background Processing (Hangfire)
Notifications are executed asynchronously to avoid blocking API requests.  
Benefits:
- Better API performance
- Retry & failure handling
- Scalable background execution

---


## Metrics & Observability
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


## Benefits
- Clean separation of concerns
- Easy to extend with new channels (Slack, WhatsApp, etc.)
- Production-ready observability
- Works seamlessly with background jobs
- Suitable for monolith or future microservice extraction

---


## Future Improvements
- Retry & dead-letter strategy per channel
- Notification templates & localization
- User preferences per channel
- Rate limiting per provider
- Distributed tracing (notification span)
