namespace OrganisationRegistry.OpenTelemetry;

using System.Diagnostics.Metrics;

public class OpenTelemetryMetrics
{
    public class ElasticSearchProjections
    {
        public const string MeterName = "OR.ES";
        private readonly Meter _meter;

        // public metrics
        public Histogram<int> NumberOfEnvelopesHandledHistogram { get; }

        public int NumberOfEnvelopesHandledGauge { get; set; }
        public int NumberOfEnvelopesHandledCounter { get; set; }

        public int NumberOfEnvelopesBehindGauge { get; set; }
        public int NumberOfEnvelopesBehindCounter { get; set; }

        public int LastProcessedEventNumberGauge { get; set; }
        public int LastProcessedEventNumberCounter { get; set; }
        public int NumberOfOrganisationsToRebuildGauge { get; set; }
        public int NumberOfOrganisationsToRebuildCounter { get; set; }

        public int MaxEventNumberToProcessGauge { get; set; }
        public int MaxEventNumberToProcessCounter { get; set; }

        private static class MeterNames
        {
            public static Func<string, string> EnvelopesHandled = (meterType) => $"{MeterName}.{nameof(EnvelopesHandled)}.{meterType}";
            public static Func<string, string> EnvelopesBehind = (meterType) => $"{MeterName}.{nameof(EnvelopesBehind)}.{meterType}";
            public static Func<string, string> LastProcessedEvent = (meterType) =>$"{MeterName}.{nameof(LastProcessedEvent)}.{meterType}";
            public static Func<string, string> MaxEventNumberToProcess = (meterType) =>$"{MeterName}.{nameof(MaxEventNumberToProcess)}.{meterType}";
            public static Func<string, string> OrganisationsToRebuild = (meterType) =>$"{MeterName}.{nameof(OrganisationsToRebuild)}.{meterType}";
        }

        public ElasticSearchProjections(string runnerName)
        {
            const string histogram = "Histogram";
            const string counter = "Counter";
            const string gauge = "Gauge";

            _meter = new Meter($"{MeterName}.{runnerName}");

            NumberOfEnvelopesHandledHistogram = _meter.CreateHistogram<int>(MeterNames.EnvelopesHandled(histogram), "envelopes", "number of envelopes handled");
            _meter.CreateObservableGauge(MeterNames.EnvelopesHandled(gauge), () => NumberOfEnvelopesHandledGauge, "envelopes", "number of envelopes handled");
            _meter.CreateObservableCounter(MeterNames.EnvelopesHandled(counter), () => NumberOfEnvelopesHandledCounter, "envelopes", "number of envelopes handled");

            _meter.CreateObservableCounter(MeterNames.LastProcessedEvent(counter), () => LastProcessedEventNumberCounter);
            _meter.CreateObservableGauge(MeterNames.LastProcessedEvent(gauge), () => LastProcessedEventNumberGauge);

            _meter.CreateObservableCounter(MeterNames.MaxEventNumberToProcess(counter), () => MaxEventNumberToProcessCounter);
            _meter.CreateObservableGauge(MeterNames.MaxEventNumberToProcess(gauge), () => MaxEventNumberToProcessGauge);

            _meter.CreateObservableCounter(MeterNames.EnvelopesBehind(counter), () => NumberOfEnvelopesBehindCounter);
            _meter.CreateObservableGauge(MeterNames.EnvelopesBehind(gauge), () => NumberOfEnvelopesBehindGauge);

            _meter.CreateObservableCounter(MeterNames.OrganisationsToRebuild(counter), () => NumberOfOrganisationsToRebuildCounter);
            _meter.CreateObservableGauge(MeterNames.OrganisationsToRebuild(gauge), () => NumberOfOrganisationsToRebuildGauge);
        }
    }
}
