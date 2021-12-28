namespace OrganisationRegistry.Api.Infrastructure.Search.Filtering
{
    using System.Collections.Generic;
    using Newtonsoft.Json;

    public class FilteringHeader<T>
    {
        [JsonIgnore]
        public bool ShouldFilter => !EqualityComparer<T>.Default.Equals(Filter, default(T));

        public T? Filter { get; set; }

        public FilteringHeader(T filter)
        {
            Filter = filter;
        }
    }
}
