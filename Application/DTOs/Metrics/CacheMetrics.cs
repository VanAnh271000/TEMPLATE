using System.Diagnostics.Metrics;

namespace Application.DTOs.Metrics
{
    public class CacheMetrics
    {
        public const string MeterName = "Template.Cache";

        public static readonly Counter<long> Hit =
            new Meter(MeterName).CreateCounter<long>("cache_hit_total");

        public static readonly Counter<long> Miss =
            new Meter(MeterName).CreateCounter<long>("cache_miss_total");
    }
}
