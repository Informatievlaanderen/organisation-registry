namespace OrganisationRegistry.ElasticSearch.Projections.Metrics
{
    using System.Collections.Generic;
    using App.Metrics;
    using App.Metrics.Counter;
    using OrganisationRegistry.Infrastructure.Events;

    public class EnvelopeMetrics
    {
        private readonly IMetricsRoot _metrics;
        private readonly CounterOptions _counter;

        public EnvelopeMetrics(IMetricsRoot metrics, string projectionName)
        {
            _metrics = metrics;
            _counter = GetCounter(projectionName);
        }

        private static CounterOptions GetCounter(string projectionName) =>
            new()
            {
                Name = $"{projectionName}: Number of envelopes found",
                MeasurementUnit = Unit.Calls,
            };

        public void LogEnvelopeCount(IReadOnlyCollection<IEnvelope> envelopes) =>
            _metrics.Measure.Counter.Increment(_counter, envelopes.Count);
    }
}
