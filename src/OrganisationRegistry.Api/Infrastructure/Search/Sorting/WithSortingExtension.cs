namespace OrganisationRegistry.Api.Infrastructure.Search.Sorting;

using System;
using System.Linq;
using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;

public static class WithSortingExtension
{
    public static IQueryable<T> WithSorting<T>(this IQueryable<T> source, SortingHeader sortingHeader, ISorting sorting)
    {
        sortingHeader = sortingHeader.ShouldSort
                        && sorting.SortableFields.Contains(sortingHeader.SortBy, StringComparer.OrdinalIgnoreCase)
            ? sortingHeader
            : sorting.DefaultSortingHeader;

        return sortingHeader.SortOrder == SortOrder.Ascending
            ? source.OrderBy(sortingHeader.SortBy)
            : source.OrderByDescending(sortingHeader.SortBy);
    }
}
