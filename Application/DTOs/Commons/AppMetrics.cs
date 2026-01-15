using System.Diagnostics.Metrics;

namespace Application.DTOs.Commons
{
    public class AppMetrics
    {
        public const string MeterName = "Template";

        public static readonly Counter<long> Requests =
            new Meter(MeterName).CreateCounter<long>("app_requests_total");
    }
}
