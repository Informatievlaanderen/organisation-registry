namespace OrganisationRegistry.ElasticSearch.Projections.Metrics.SerilogReporter;

using System;
using App.Metrics;
using App.Metrics.Filters;
using Serilog.Events;

/// <summary>
/// Provides programmatic configuration for Serilog Reporting in the App Metrics framework.
/// </summary>
public class SerilogMetricsReporterOptions
{
    /// <summary>
    /// Gets or sets the required <see cref="LogEventLevel"/> for writing metric snapshots.
    /// </summary>
    public LogEventLevel LogEventLevel { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="TimeSpan" /> interval to flush metrics values.
    /// </summary>
    /// <remarks>
    /// Defaults to <see cref="AppMetricsConstants.Reporting.DefaultFlushInterval" />.
    /// </remarks>
    public TimeSpan FlushInterval { get; set; }

    /// <summary>
    /// Gets or sets the <see cref="IFilterMetrics" /> to use for this reporter.
    /// </summary>
    public IFilterMetrics? Filter { get; set; }

    /// <summary>
    /// Gets or sets the Serilog message template used for different metric types.
    /// </summary>
    public MessageTemplates MessageTemplates { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SerilogMetricsReporterOptions"/> class.
    /// </summary>
    public SerilogMetricsReporterOptions()
    {
        LogEventLevel = LogEventLevel.Debug;

        FlushInterval = AppMetricsConstants.Reporting.DefaultFlushInterval;

        MessageTemplates = new MessageTemplates();
    }
}
