namespace OrganisationRegistry.Api.Infrastructure.Search.Sorting
{
    using System.Collections.Generic;

    public interface ISorting
    {
        IEnumerable<string> SortableFields { get; }

        SortingHeader DefaultSortingHeader { get; }
    }
}
