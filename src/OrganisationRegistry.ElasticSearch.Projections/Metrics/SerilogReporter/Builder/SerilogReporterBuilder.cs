using System;
using App.Metrics.Builder;
using Serilog.Events;

// ReSharper disable once CheckNamespace
namespace App.Metrics
{
    using OrganisationRegistry.ElasticSearch.Projections.Metrics.SerilogReporter;

    /// <summary>
    /// Builders for configuring Serilog metrics reporting using an <see cref="IMetricsReportingBuilder" />.
    /// </summary>
    public static class SerilogReporterBuilder
    {
        /// <summary>
        ///     Add the <see cref="SerilogMetricsReporter" /> allowing metrics to be reported to Serilog.
        /// </summary>
        /// <param name="metricReporterProviderBuilder">
        ///     The <see cref="IMetricsReportingBuilder" /> used to configure metrics reporters.
        /// </param>
        /// <param name="setupAction">The Serilog reporting options to use.</param>
        /// <returns>
        ///     An <see cref="IMetricsBuilder" /> that can be used to further configure App Metrics.
        /// </returns>
        public static IMetricsBuilder ToSerilog(
            this IMetricsReportingBuilder metricReporterProviderBuilder,
            Action<SerilogMetricsReporterOptions>? setupAction)
        {
            if (metricReporterProviderBuilder == null)
                throw new ArgumentNullException(nameof(metricReporterProviderBuilder));

            var options = new SerilogMetricsReporterOptions();

            setupAction?.Invoke(options);

            var reporter = new SerilogMetricsReporter(options);

            return metricReporterProviderBuilder.Using(reporter);
        }

        /// <summary>
        ///     Add the <see cref="SerilogMetricsReporter" /> allowing metrics to be reported to Serilog.
        /// </summary>
        /// <param name="metricReporterProviderBuilder">
        ///     The <see cref="IMetricsReportingBuilder" /> used to configure metrics reporters.
        /// </param>
        /// <param name="logEventLevel">The log event level to control when metrics are written.</param>
        /// <returns>
        ///     An <see cref="IMetricsBuilder" /> that can be used to further configure App Metrics.
        /// </returns>
        public static IMetricsBuilder ToSerilog(
            this IMetricsReportingBuilder metricReporterProviderBuilder,
            LogEventLevel logEventLevel)
        {
            return ToSerilog(metricReporterProviderBuilder, options => options.LogEventLevel = logEventLevel);
        }
    }
}
