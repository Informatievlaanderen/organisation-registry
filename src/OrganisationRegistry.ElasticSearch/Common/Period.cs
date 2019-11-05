namespace OrganisationRegistry.ElasticSearch.Common
{
    using System;
    using Nest;

    public class Period
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }

        public Period() { }

        public Period(DateTime? start, DateTime? end)
        {
            Start = start;
            End = end;
        }

        public static IPromise<IProperties> Mapping(PropertiesDescriptor<Period> map) => map
            .Date(d => d
                .Name(p => p.Start)
                .Format("yyyy-MM-dd'T'HH:mm:ss||yyyy-MM-dd HH:mm:ss||yyyy-MM-dd||epoch_millis"))

            .Date(d => d
                .Name(p => p.End)
                .Format("yyyy-MM-dd'T'HH:mm:ss||yyyy-MM-dd HH:mm:ss||yyyy-MM-dd||epoch_millis"));
    }
}
