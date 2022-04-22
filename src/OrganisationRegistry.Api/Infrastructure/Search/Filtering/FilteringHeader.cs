namespace OrganisationRegistry.Api.Infrastructure.Search.Filtering
{
    public class FilteringHeader<T>
    {
        public T? Filter { get; set; }

        public FilteringHeader(T? filter)
        {
            Filter = filter;
        }
    }
}
