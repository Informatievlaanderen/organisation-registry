namespace OrganisationRegistry.OpenTelemetry;

using System.Diagnostics.Metrics;

public static class OpenTelemetryMetrics
{
    public class ElasticSearchProjections
    {
        public const string MeterName = nameof(ElasticSearchProjections);
        private readonly Meter _meter = new(MeterName);

        // public metrics
        public Histogram<int> NumberOfEnvelopesHandled { get; set; }
        public int NumberOfEnvelopesBehind { get; set; }
        public Measurement<int> LastProcessedEventNumber { get; set; }
        public int NumberOfOrganisationsToRebuild { get; set; }
        public int MaxEventNumberToProcess { get; set; }

        private static class MeterNames
        {
            public static Func<string, string> NumberOfEnvelopesHandled = runnerName => $"{MeterName}.{runnerName}.{nameof(NumberOfEnvelopesHandled)}";
            public static Func<string, string> NumberOfEnvelopesBehind = runnerName => $"{MeterName}.{runnerName}.{nameof(NumberOfEnvelopesBehind)}";
            public static Func<string, string> LastProcessedEventNumber = runnerName =>$"{MeterName}.{runnerName}.{nameof(LastProcessedEventNumber)}";
            public static string MaxEventNumberToProcess = $"{MeterName}.{nameof(MaxEventNumberToProcess)}";
            public static Func<string, string> OrganisationsToRebuildCounter = runnerName =>$"{MeterName}.{runnerName}.{nameof(OrganisationsToRebuildCounter)}";
        }

        public ElasticSearchProjections(string runnerName)
        {
            NumberOfEnvelopesHandled = _meter.CreateHistogram<int>(MeterNames.NumberOfEnvelopesHandled(runnerName), "envelopes", "number of envelopes handled");
            _meter.CreateObservableCounter(MeterNames.LastProcessedEventNumber(runnerName), () => LastProcessedEventNumber);
            _meter.CreateObservableGauge(MeterNames.MaxEventNumberToProcess, () => MaxEventNumberToProcess);
            _meter.CreateObservableGauge(MeterNames.NumberOfEnvelopesBehind(runnerName), () => NumberOfEnvelopesBehind);
            _meter.CreateObservableGauge(MeterNames.OrganisationsToRebuildCounter(runnerName), () => NumberOfOrganisationsToRebuild);
        }
    }
}
