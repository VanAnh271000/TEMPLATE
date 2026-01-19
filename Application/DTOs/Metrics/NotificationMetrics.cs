using System.Diagnostics.Metrics;

namespace Application.DTOs.Metrics
{
    public class NotificationMetrics
    {
        public const string MeterName = "Template.notifications";

        private static readonly Meter Meter = new(MeterName);

        public static readonly Counter<long> SendTotal =
            Meter.CreateCounter<long>(
                "notification_send_total",
                description: "Total number of notifications sent");

        public static readonly Counter<long> SendSuccess =
            Meter.CreateCounter<long>(
                "notification_send_success_total",
                description: "Total successful notifications");

        public static readonly Counter<long> SendFailed =
            Meter.CreateCounter<long>(
                "notification_send_failed_total",
                description: "Total failed notifications");

        public static readonly Histogram<double> SendDuration =
            Meter.CreateHistogram<double>(
                "notification_send_duration_seconds",
                unit: "s",
                description: "Notification send duration");

        public static readonly Counter<long> RetryTotal =
            Meter.CreateCounter<long>(
                "notification_retry_total",
                description: "Notification retry count");
    }
}
