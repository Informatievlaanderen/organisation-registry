namespace OrganisationRegistry.ElasticSearch.Common;

using System;
using OpenSearch.Client;

public class Period
{
    private Period()
    {
    }

    private Period(DateTime? start, DateTime? end)
    {
        Start = start;
        End = end;
    }

    public DateTime? Start { get; set; }
    public DateTime? End { get; set; }

    public static Period Infinite()
        => new();

    public bool IsInfinite()
        => Start == null && End == null;

    public static Period FromDates(DateTime? start, DateTime? end)
        => new(start, end);

    public bool OverlapsWith(DateTime date)
        => (!Start.HasValue || Start.Value <= date) &&
           (!End.HasValue || End.Value >= date);

    public static IPromise<IProperties> Mapping(PropertiesDescriptor<Period> map)
        => map
            .Date(
                d => d
                    .Name(p => p.Start)
                    .Format("yyyy-MM-dd'T'HH:mm:ss||yyyy-MM-dd HH:mm:ss||yyyy-MM-dd||epoch_millis"))
            .Date(
                d => d
                    .Name(p => p.End)
                    .Format("yyyy-MM-dd'T'HH:mm:ss||yyyy-MM-dd HH:mm:ss||yyyy-MM-dd||epoch_millis"));
}
