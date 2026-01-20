using System.Diagnostics.Metrics;

namespace Application.DTOs.Metrics
{
    public class AppMetrics
    {
        public const string MeterName = "Template";

        public static readonly Counter<long> Requests =
            new Meter(MeterName).CreateCounter<long>("app_requests_total");
    }
}
