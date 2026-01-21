namespace Infrastructure.Versioning
{
    public static class ApiVersionPolicy
    {
        public static readonly HashSet<int> DeprecatedMajorVersions = new()
        {
            1 // v1.x is deprecated
        };

        public static readonly DateTime SunsetDateV1 =
            new DateTime(2026, 12, 31);
    }

}
