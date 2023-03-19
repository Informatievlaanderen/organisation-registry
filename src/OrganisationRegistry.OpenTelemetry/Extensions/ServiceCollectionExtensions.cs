namespace OrganisationRegistry.OpenTelemetry.Extensions;

using System.Reflection;
using global::OpenTelemetry.Exporter;
using global::OpenTelemetry.Logs;
using global::OpenTelemetry.Metrics;
using global::OpenTelemetry.Resources;
using global::OpenTelemetry.Trace;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class ServiceCollectionExtensions
{
    public static TracerProviderBuilder AddAspNetCoreWithDistributedTracing(this TracerProviderBuilder builder, string headerName = "traceparent")
    {
        builder.AddAspNetCoreInstrumentation(
            options =>
            {
                options.EnrichWithHttpRequest =
                    (activity, request) => activity.SetParentId(request.Headers[headerName]);
                options.Filter = context => context.Request.Method != HttpMethods.Options;
            });
        return builder;
    }
    public static IServiceCollection AddOpenTelemetry(this IServiceCollection services, Action<TracerProviderBuilder> customize)
    {
        var executingAssembly = Assembly.GetEntryAssembly()!;
        var serviceName = executingAssembly.GetName().Name!;
        var assemblyVersion = executingAssembly.GetName().Version?.ToString() ?? "unknown";
        var collectorUrl = Environment.GetEnvironmentVariable("COLLECTOR_URL") ?? "http://localhost:4317";

        var configureResource = BuildConfigureResource(serviceName, assemblyVersion);

        services.AddOpenTelemetryTracing(
            builder =>
            {
                builder
                    .AddSource(serviceName)
                    .ConfigureResource(configureResource)
                    .AddOtlpExporter(
                        options =>
                        {
                            options.Protocol = OtlpExportProtocol.Grpc;
                            options.Endpoint = new Uri(collectorUrl);
                        });

                customize(builder);
            });

        services.AddLogging(
            builder =>
                builder
                    .AddOpenTelemetry(
                        options =>
                        {
                            options.ConfigureResource(configureResource);

                            options.IncludeScopes = true;
                            options.IncludeFormattedMessage = true;
                            options.ParseStateValues = true;

                            options.AddOtlpExporter(
                                exporter =>
                                {
                                    exporter.Protocol = OtlpExportProtocol.Grpc;
                                    exporter.Endpoint = new Uri(collectorUrl);
                                });
                        }));

        services.AddOpenTelemetryMetrics(
            options =>
                options
                    .ConfigureResource(configureResource)
                    .AddRuntimeInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddAspNetCoreInstrumentation()
                    .AddOtlpExporter(
                        exporter =>
                        {
                            exporter.Protocol = OtlpExportProtocol.Grpc;
                            exporter.Endpoint = new Uri(collectorUrl);
                        }).AddConsoleExporter(
                        (exporterOptions, readerOptions) =>
                        {
                            readerOptions.PeriodicExportingMetricReaderOptions = new PeriodicExportingMetricReaderOptions()
                            {
                                ExportIntervalMilliseconds = (int)TimeSpan.FromMinutes(10).TotalMilliseconds
                            };
                        })
                    .AddMeter(OpenTelemetryMetrics.ElasticSearchProjections.MeterNameFunc("Organisation"))
                    .AddMeter(OpenTelemetryMetrics.ElasticSearchProjections.MeterNameFunc("Body"))
                    .AddMeter(OpenTelemetryMetrics.ElasticSearchProjections.MeterNameFunc("Person"))
                );
        return services;
    }

    private static Action<ResourceBuilder> BuildConfigureResource(string serviceName, string assemblyVersion)
    {
        return r => r
            .AddService(
                serviceName,
                serviceVersion: assemblyVersion,
                serviceInstanceId: Environment.MachineName)
            .AddAttributes(
                new Dictionary<string, object>
                {
                    ["deployment.environment"] =
                        Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLowerInvariant()
                        ?? "unknown",
                });
    }

    public static ILoggingBuilder ConfigureLogging(ILoggingBuilder builder)
    {
        var executingAssembly = Assembly.GetEntryAssembly()!;
        var serviceName = executingAssembly.GetName().Name!;
        var assemblyVersion = executingAssembly.GetName().Version?.ToString() ?? "unknown";
        var configureResource = BuildConfigureResource(serviceName, assemblyVersion);

        builder.AddOpenTelemetry(options =>
        {
            options.ConfigureResource(configureResource);

            options.IncludeScopes = true;
            options.IncludeFormattedMessage = true;
            options.ParseStateValues = true;

            options.AddOtlpExporter(exporter =>
            {
                exporter.Protocol = OtlpExportProtocol.Grpc;
                exporter.Endpoint = new Uri("http://localhost:4319");
            });

            // .AddConsoleExporter();
        }).AddSimpleConsole();
        return builder;
    }
}
