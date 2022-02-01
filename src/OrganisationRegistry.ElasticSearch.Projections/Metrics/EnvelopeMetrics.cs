namespace OrganisationRegistry.ElasticSearch.Projections.Metrics
{
    using System.Collections.Generic;
    using App.Metrics;
    using App.Metrics.Counter;
    using App.Metrics.Meter;
    using OrganisationRegistry.Infrastructure.Events;

    public class EnvelopeMetrics
    {
        private readonly IMetricsRoot _metrics;
        private readonly MeterOptions _meter;
        private readonly CounterOptions _counter;

        public EnvelopeMetrics(IMetricsRoot metrics, string projectionName)
        {
            _metrics = metrics;
            _counter = GetCounter(projectionName);
            _meter = GetMeter(projectionName);
        }

        private static CounterOptions GetCounter(string projectionName) =>
            new()
            {
                Name = $"{projectionName}: [Counter] Number of envelopes",
                MeasurementUnit = Unit.Calls,
                ResetOnReporting = true,
            };

        private static MeterOptions GetMeter(string projectionName) =>
            new()
            {
                Name = $"{projectionName}: [Meter] Number of envelopes",
                MeasurementUnit = Unit.Calls,
                RateUnit = TimeUnit.Minutes,
            };

        public void CountEnvelopes(IReadOnlyCollection<IEnvelope> envelopes) =>
            _metrics.Measure.Counter.Increment(_counter, envelopes.Count);

        public void MeterEnvelopes(IReadOnlyCollection<IEnvelope> envelopes) =>
            _metrics.Measure.Meter.Mark(_meter, envelopes.Count);
    }
}
