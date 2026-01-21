using Asp.Versioning.ApiExplorer;
using Infrastructure.Versioning;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace API.Installers
{
    public sealed class ConfigureSwaggerOptions
    : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(
            IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                var major = description.ApiVersion.MajorVersion ?? 0;
                var isDeprecated = ApiVersionPolicy.DeprecatedMajorVersions.Contains(major);
                options.SwaggerDoc(
                    description.GroupName,
                    new OpenApiInfo
                    {
                        Title = "Template API",
                        Version = description.ApiVersion.ToString(),
                        Description = isDeprecated
                            ? "This API version has been deprecated."
                            : "API documentation"
                    });
            }
        }
    }
}
