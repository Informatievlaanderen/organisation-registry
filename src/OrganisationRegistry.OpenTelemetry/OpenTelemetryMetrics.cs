namespace OrganisationRegistry.OpenTelemetry;

using System.Diagnostics.Metrics;

public class OpenTelemetryMetrics
{
    public class ElasticSearchProjections
    {
        public static Func<string, string> MeterNameFunc = name => $"OR.ES.{name}";
        private readonly Meter _meter;

        // public metrics
        public Histogram<int> NumberOfEnvelopesHandledHistogram { get; }
        public int NumberOfEnvelopesHandledGauge { get; set; }
        public int NumberOfEnvelopesHandledCounter { get; set; }

        public Histogram<int> NumberOfEnvelopesBehindHistogram { get; }
        public int NumberOfEnvelopesBehindGauge { get; set; }
        public int NumberOfEnvelopesBehindCounter { get; set; }

        public int LastProcessedEventNumberGauge { get; set; }
        public int LastProcessedEventNumberCounter { get; set; }

        // public int NumberOfOrganisationsToRebuildGauge { get; set; }
        // public int NumberOfOrganisationsToRebuildCounter { get; set; }

        public int MaxEventNumberToProcessGauge { get; set; }
        public int MaxEventNumberToProcessCounter { get; set; }

        private static class MeterNames
        {
            public static Func<string, string, string> EnvelopesHandled = (meterName, meterType) => $"{meterName}.{nameof(EnvelopesHandled)}.{meterType}";
            public static Func<string, string, string> EnvelopesBehind = (meterName, meterType) => $"{meterName}.{nameof(EnvelopesBehind)}.{meterType}";
            public static Func<string, string, string> LastProcessedEvent = (meterName, meterType) =>$"{meterName}.{nameof(LastProcessedEvent)}.{meterType}";
            public static Func<string, string, string> MaxEventNumberToProcess = (meterName, meterType) =>$"{meterName}.{nameof(MaxEventNumberToProcess)}.{meterType}";
            public static Func<string, string, string> OrganisationsToRebuild = (meterName, meterType) =>$"{meterName}.{nameof(OrganisationsToRebuild)}.{meterType}";
        }

        public ElasticSearchProjections(string runnerName)
        {
            const string histogram = "Histogram";
            const string counter = "Counter";
            const string gauge = "Gauge";

            var meterName = MeterNameFunc(runnerName);
            _meter = new Meter(meterName);

            NumberOfEnvelopesHandledHistogram = _meter.CreateHistogram<int>(MeterNames.EnvelopesHandled(meterName, histogram), "envelopes", "number of envelopes handled");
            _meter.CreateObservableGauge(MeterNames.EnvelopesHandled(meterName, gauge), () => NumberOfEnvelopesHandledGauge, "envelopes", "number of envelopes handled");
            _meter.CreateObservableUpDownCounter(MeterNames.EnvelopesHandled(meterName, counter), () => NumberOfEnvelopesHandledCounter, "envelopes", "number of envelopes handled");

            _meter.CreateObservableUpDownCounter(MeterNames.LastProcessedEvent(meterName, counter), () => LastProcessedEventNumberCounter);
            _meter.CreateObservableGauge(MeterNames.LastProcessedEvent(meterName, gauge), () => LastProcessedEventNumberGauge);

            _meter.CreateObservableUpDownCounter(MeterNames.MaxEventNumberToProcess(meterName, counter), () => MaxEventNumberToProcessCounter);
            _meter.CreateObservableGauge(MeterNames.MaxEventNumberToProcess(meterName, gauge), () => MaxEventNumberToProcessGauge);

            NumberOfEnvelopesBehindHistogram = _meter.CreateHistogram<int>(MeterNames.EnvelopesBehind(meterName, histogram), "envelopes", "number of envelopes behind");
            _meter.CreateObservableUpDownCounter(MeterNames.EnvelopesBehind(meterName, counter), () => NumberOfEnvelopesBehindCounter, "envelopes", "number of envelopes behind");
            _meter.CreateObservableGauge(MeterNames.EnvelopesBehind(meterName, gauge), () => NumberOfEnvelopesBehindGauge, "envelopes", "number of envelopes behind");

            // _meter.CreateObservableUpDownCounter(MeterNames.OrganisationsToRebuild(meterName, counter), () => NumberOfOrganisationsToRebuildCounter);
            // _meter.CreateObservableGauge(MeterNames.OrganisationsToRebuild(meterName, gauge), () => NumberOfOrganisationsToRebuildGauge);
        }
    }
}
