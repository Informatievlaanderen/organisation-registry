namespace OrganisationRegistry.Api.Backoffice.Parameters.OrganisationRelationType.Queries;

using System.Collections.Generic;
using System.Linq;
using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
using Infrastructure.Search;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Sorting;
using SqlServer.Infrastructure;
using SqlServer.OrganisationRelationType;

public class OrganisationRelationTypeListQuery : Query<OrganisationRelationTypeListItem>
{
    private readonly OrganisationRegistryContext _context;

    protected override ISorting Sorting => new OrganisationRelationTypeListSorting();

    public OrganisationRelationTypeListQuery(OrganisationRegistryContext context)
    {
        _context = context;
    }

    protected override IQueryable<OrganisationRelationTypeListItem> Filter(FilteringHeader<OrganisationRelationTypeListItem> filtering)
    {
        var organisationRelationTypes = _context.OrganisationRelationTypeList.AsQueryable();

        if (filtering.Filter is not { } filter)
            return organisationRelationTypes;

        if (!filter.Name.IsNullOrWhiteSpace())
            organisationRelationTypes = organisationRelationTypes.Where(x =>
                x.Name.Contains(filter.Name) ||
                x.InverseName.Contains(filter.Name));

        return organisationRelationTypes;
    }

    private class OrganisationRelationTypeListSorting : ISorting
    {
        public IEnumerable<string> SortableFields { get; } = new[]
        {
            nameof(OrganisationRelationTypeListItem.Name),
        };

        public SortingHeader DefaultSortingHeader { get; } =
            new(nameof(OrganisationRelationTypeListItem.Name), SortOrder.Ascending);
    }
}
