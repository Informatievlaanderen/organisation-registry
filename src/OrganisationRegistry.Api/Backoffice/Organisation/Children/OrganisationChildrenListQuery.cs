namespace OrganisationRegistry.Api.Backoffice.Organisation.Children;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure.Search;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Sorting;
using SqlServer.Infrastructure;
using SqlServer.Organisation;

public class OrganisationChildListQueryResult
{
    public Guid Id { get; }
    public string Name { get; }
    public string OvoNumber { get; }

    public OrganisationChildListQueryResult(Guid id, string ovoNumber, string name)
    {
        Id = id;
        OvoNumber = ovoNumber;
        Name = name;
    }
}

public class OrganisationChildListQuery : Query<OrganisationChildListItem, OrganisationChildListItem, OrganisationChildListQueryResult>
{
    private readonly OrganisationRegistryContext _context;
    private readonly Guid _organisationId;

    protected override ISorting Sorting => new OrganisationChildListSorting();

    protected override Expression<Func<OrganisationChildListItem, OrganisationChildListQueryResult>> Transformation =>
        x => new OrganisationChildListQueryResult(
            x.Id,
            x.OvoNumber,
            x.Name);

    public OrganisationChildListQuery(OrganisationRegistryContext context, Guid organisationId)
    {
        _context = context;
        _organisationId = organisationId;
    }

    /// <summary>
    /// Get active children without filtering
    /// </summary>
    /// <param name="filtering"></param>
    /// <returns></returns>
    protected override IQueryable<OrganisationChildListItem> Filter(FilteringHeader<OrganisationChildListItem> filtering)
        => _context.OrganisationChildrenList
            .AsQueryable()
            .Where(x => x.ParentOrganisationId == _organisationId)
            .AsQueryable()
            .Where(x =>
                (!x.ValidFrom.HasValue || x.ValidFrom <= DateTime.Today) &&
                (!x.ValidTo.HasValue || x.ValidTo >= DateTime.Today))
            .Where(x =>
                (!x.OrganisationValidFrom.HasValue || x.OrganisationValidFrom <= DateTime.Today) &&
                (!x.OrganisationValidTo.HasValue || x.OrganisationValidTo >= DateTime.Today));

    private class OrganisationChildListSorting : ISorting
    {
        public IEnumerable<string> SortableFields { get; } = new[]
        {
            nameof(OrganisationChildListItem.OvoNumber),
            nameof(OrganisationChildListItem.Name)
        };

        public SortingHeader DefaultSortingHeader { get; } =
            new SortingHeader(nameof(OrganisationChildListItem.Name), SortOrder.Ascending);
    }
}
