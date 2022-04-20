namespace OrganisationRegistry.ElasticSearch.Projections.Metrics.SerilogReporter
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using App.Metrics;
    using App.Metrics.Filters;
    using App.Metrics.Formatters;
    using App.Metrics.Logging;
    using App.Metrics.Reporting;
    using Serilog;
    using Serilog.Events;

    /// <summary>
    /// A metrics reporter for writing snapshots to Serilog.
    /// </summary>
    public class SerilogMetricsReporter : IReportMetrics
    {
        private static readonly ILog Logger = LogProvider.For<SerilogMetricsReporter>();
        private readonly LogEventLevel _logEventLevel;
        private readonly IMetricsOutputFormatter _formatter;

        /// <inheritdoc />
        public IFilterMetrics? Filter { get; set; }

        /// <inheritdoc />
        public TimeSpan FlushInterval { get; set; }

        /// <inheritdoc />
        public IMetricsOutputFormatter Formatter
        {
            get => _formatter;
            set => throw new NotSupportedException("The formatter cannot be replaced");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SerilogMetricSnapshotWriter"/> class.
        /// </summary>
        /// <param name="options">The Serilog metrics reporter options.</param>
        public SerilogMetricsReporter(SerilogMetricsReporterOptions options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            if (options.FlushInterval < TimeSpan.Zero)
                throw new ArgumentException($"{nameof(SerilogMetricsReporterOptions.FlushInterval)} must not be less than zero");

            _logEventLevel = options.LogEventLevel;

            _formatter = new SerilogMetricsOutputFormatter(options);

            FlushInterval = options.FlushInterval > TimeSpan.Zero
                ? options.FlushInterval
                : AppMetricsConstants.Reporting.DefaultFlushInterval;

            Filter = options.Filter;

            Logger.Info($"Using Metrics Reporter {this}. {nameof(SerilogMetricsReporterOptions.LogEventLevel)}: {options.LogEventLevel} {nameof(SerilogMetricsReporterOptions.FlushInterval)}: {FlushInterval}");
        }

        /// <inheritdoc />
        public async Task<bool> FlushAsync(MetricsDataValueSource metricsData, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!Log.IsEnabled(_logEventLevel))
            {
                Logger.Trace("Skipping metrics snapshot flush as required log event level not met");
                return true;
            }

            Logger.Trace("Flushing metrics snapshot");

            await Formatter.WriteAsync(null, metricsData, cancellationToken);

            Logger.Trace("Flushed metrics snapshot");

            return true;
        }
    }
}
