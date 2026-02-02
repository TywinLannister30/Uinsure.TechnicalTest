using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using System.Diagnostics;
using Uinsure.TechnicalTest.Application.Configuration;

namespace Uinsure.TechnicalTest.API.Extensions;

public static class TracingExtensions
{
    public static void AddOtlp(this IServiceCollection services, string serviceName, IConfiguration configuration)
    {
        services.AddOtlp(serviceName, configuration, (HttpContext ctx) => ctx.Request.Path.StartsWithSegments(new PathString("/api")));
    }

    private static void AddOtlp(this IServiceCollection services, string serviceName, IConfiguration configuration, Func<HttpContext, bool> routeFilter)
    {
        var otlpSettings = configuration?.GetSection("Otlp")?.Get<OtlpSettings>();

        if (otlpSettings?.Host == null || !otlpSettings.Enabled)
            return;

        var version = configuration?.GetValue<string>("Version");
        var @namespace = configuration?.GetValue<string>("Namespace");
        var endpoint = new Uri($"{otlpSettings.Scheme}://{otlpSettings.Host}:{otlpSettings.Port}");

        services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder
                    .AddSource("unisure")
                    .SetResourceBuilder(ResourceBuilder.CreateDefault()
                        .AddService(
                            serviceName: otlpSettings.ServiceName ?? serviceName,
                            serviceVersion: version,
                            serviceNamespace: @namespace))
                    .AddAspNetCoreInstrumentation(options => { options.Filter = routeFilter; })
                    .AddHttpClientInstrumentation()
                    .AddEntityFrameworkCoreInstrumentation()
                    .AddSqlClientInstrumentation(options =>
                    {
                        options.RecordException = true;
                        options.EnrichWithSqlCommand = delegate (Activity activity, object _)
                        {
                            activity.DisplayName = $"SQL Query to {activity.GetTagItem("db.name") ?? "untagged db name"}";
                        };
                    })
                    .AddOtlpExporter(options => { options.Endpoint = endpoint; });
            });
    }
}
