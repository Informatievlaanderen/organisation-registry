namespace OrganisationRegistry.ElasticSearch.Projections.Metrics.SerilogReporter;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Serialization;
using Serilog;
using Serilog.Events;
using Serilog.Parsing;

/// <summary>
/// A metric snapshot writer for Serilog.
/// </summary>
public class SerilogMetricSnapshotWriter : IMetricSnapshotWriter, IDisposable
{
    private readonly LogEventLevel _logEventLevel;
    private readonly MessageTemplates _messageTemplates;
    private readonly MessageTemplateParser _parser = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="SerilogMetricSnapshotWriter"/> class.
    /// </summary>
    /// <param name="options">The Serilog metrics reporter options.</param>
    public SerilogMetricSnapshotWriter(SerilogMetricsReporterOptions options)
    {
        _logEventLevel = options.LogEventLevel;
        _messageTemplates = options.MessageTemplates;
    }

    /// <inheritdoc />
    public void Write(string context, string name, string field, object value, MetricTags tags, DateTime timestamp)
    {
        var contextProperty = BuildContextProperty(context);
        var nameProperty = BuildNameProperty(name);
        var valueProperty = new LogEventProperty(field, new ScalarValue(value));
        var tagsProperty = BuildTagsProperty(tags.ToDictionary());
        var properties = new[] {contextProperty, nameProperty, valueProperty, tagsProperty};

        var messageTemplate = GetMessageTemplate(tags);
        var logEvent = new LogEvent(timestamp, _logEventLevel, null, _parser.Parse(messageTemplate), properties);

        Log.Write(logEvent);
    }

    /// <inheritdoc />
    public void Write(string context, string name, IEnumerable<string> columns, IEnumerable<object> values, MetricTags tags, DateTime timestamp)
    {
        var contextProperty = BuildContextProperty(context);
        var nameProperty = BuildNameProperty(name);
        var columnNames = columns as string[] ?? columns.ToArray();
        var properties = new List<LogEventProperty> {contextProperty, nameProperty};
        var fields = columnNames.Zip(values, (column, data) => new {column, data})
            .ToDictionary(pair => pair.column, pair => pair.data);
        properties.AddRange(fields.Select(field => new LogEventProperty(FixPropertyName(field.Key), new ScalarValue(field.Value))));
        var metricTags = tags.ToDictionary();
        AddItemProperty(columnNames, metricTags, properties);
        var tagsProperty = BuildTagsProperty(metricTags);
        properties.Add(tagsProperty);

        var messageTemplate = GetMessageTemplate(tags, fields.Keys);
        var logEvent = new LogEvent(timestamp, _logEventLevel, null, _parser.Parse(messageTemplate), properties);

        Log.Write(logEvent);
    }

    private string GetMessageTemplate(MetricTags metricTags, IEnumerable<string>? columns = null)
    {
        columns ??= Enumerable.Empty<string>();

        var tags = metricTags.ToDictionary();

        if (!tags.TryGetValue("mtype", out var mtype))
            return _messageTemplates.Fallback;

        switch (mtype)
        {
            case "counter":
                return columns.Contains("percent")
                    ? _messageTemplates.CounterItem
                    : _messageTemplates.Counter;

            case "gauge":
                return _messageTemplates.Gauge;

            case "histogram":
                return _messageTemplates.Histogram;

            case "meter":
                return columns.Contains("percent")
                    ? _messageTemplates.MeterItem
                    : _messageTemplates.Meter;

            case "timer":
                return _messageTemplates.Timer;

            default:
                return _messageTemplates.Fallback;
        }
    }

    private static string FixPropertyName(string propertyName)
        => propertyName.Replace(".", "_");

    private static void AddItemProperty(IEnumerable<string> columns, IDictionary<string, string> tags, ICollection<LogEventProperty> properties)
    {
        if (columns.Contains("percent") && tags.ContainsKey("item"))
            properties.Add(new LogEventProperty("item", new ScalarValue(tags["item"])));
    }

    private static LogEventProperty BuildContextProperty(string context)
        => new("context", new ScalarValue(context));

    private static LogEventProperty BuildNameProperty(string name)
        => new("name", new ScalarValue(name));

    private static LogEventProperty BuildTagsProperty(IDictionary<string, string> tags)
    {
        var tagValues = tags
            .Select(pair => new KeyValuePair<ScalarValue, LogEventPropertyValue>(
                new ScalarValue(pair.Key), new ScalarValue(pair.Value)));

        return new LogEventProperty("tags", new DictionaryValue(tagValues));
    }

    public ValueTask DisposeAsync()
        => ValueTask.CompletedTask;

    public void Dispose()
    {
        //throw new NotImplementedException();
    }
}
