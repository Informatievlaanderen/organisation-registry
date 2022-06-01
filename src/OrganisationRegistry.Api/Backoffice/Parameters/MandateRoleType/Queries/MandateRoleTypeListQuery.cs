namespace OrganisationRegistry.Api.Backoffice.Parameters.MandateRoleType.Queries;

using System.Collections.Generic;
using System.Linq;
using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
using Infrastructure.Search;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Sorting;
using SqlServer.Infrastructure;
using SqlServer.MandateRoleType;

public class MandateRoleTypeListQuery: Query<MandateRoleTypeListItem>
{
    private readonly OrganisationRegistryContext _context;

    protected override ISorting Sorting => new MandateRoleTypeListSorting();

    public MandateRoleTypeListQuery(OrganisationRegistryContext context)
    {
        _context = context;
    }

    protected override IQueryable<MandateRoleTypeListItem> Filter(FilteringHeader<MandateRoleTypeListItem> filtering)
    {
        var mandateRoleTypes = _context.MandateRoleTypeList.AsQueryable();

        if (filtering.Filter is not { } filter)
            return mandateRoleTypes;

        if (!filter.Name.IsNullOrWhiteSpace())
            mandateRoleTypes = mandateRoleTypes.Where(x => x.Name.Contains(filter.Name));

        return mandateRoleTypes;
    }

    private class MandateRoleTypeListSorting : ISorting
    {
        public IEnumerable<string> SortableFields { get; } = new[]
        {
            nameof(MandateRoleTypeListItem.Name)
        };

        public SortingHeader DefaultSortingHeader { get; } =
            new SortingHeader(nameof(MandateRoleTypeListItem.Name), SortOrder.Ascending);
    }
}
