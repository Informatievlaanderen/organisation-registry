namespace OrganisationRegistry.ElasticSearch.Projections.Metrics.SerilogReporter;

/// <summary>
/// Serilog message templates for the different metric types.
/// </summary>
public class MessageTemplates
{
    /// <summary>
    /// Gets or sets the fallback message template to use when the metric types cannot be identified.
    /// </summary>
    public string Fallback { get; set; }

    /// <summary>
    /// Gets or sets the message template to use for the Counter metric type.
    /// </summary>
    public string Counter { get; set; }

    /// <summary>
    /// Gets or sets the message template to use for Counter item metric type.
    /// </summary>
    public string CounterItem { get; set; }

    /// <summary>
    /// Gets or sets the message template to use for the Gauge metric type.
    /// </summary>
    public string Gauge { get; set; }

    /// <summary>
    /// Gets or sets the message template to use for the Histogram metric type.
    /// </summary>
    public string Histogram { get; set; }

    /// <summary>
    /// Gets or sets the message template to use for the Meter metric type.
    /// </summary>
    public string Meter { get; set; }

    /// <summary>
    /// Gets or sets the message template to use for Meter item metric type.
    /// </summary>
    public string MeterItem { get; set; }

    /// <summary>
    /// Gets or sets the message template to use for Timer metric type.
    /// </summary>
    public string Timer { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MessageTemplates"/> class.
    /// </summary>
    public MessageTemplates()
    {
        Fallback = "[Metric] Context: {context}, Name: {name}";

        Counter = "[Counter] Context: {context}, Name: {name}, Value: {value}";

        CounterItem = "[CounterItem] Context: {context}, Name: {name}, Item: {item}, Percent: {percent}, Total: {total}";

        Gauge = "[Gauge] Context: {context}, Name: {name}, Value: {value}";

        Histogram = "[Histogram] Context: {context}, Name: {name}, Min: {min}, Mean: {mean}, Max: {max}";

        Meter = "[Meter] Context: {context}, Name: {name}, Count: {count_meter}, Mean: {rate_mean}";

        MeterItem = "[MeterItem] Context: {context}, Name: {name}, Item: {item}, Percent: {percent}, Count: {count_meter}, Mean: {rate_mean}";

        Timer = "[Timer] Context: {context}, Name: {name}, Min: {min}, Mean: {mean}, Max: {max}";
    }
}
