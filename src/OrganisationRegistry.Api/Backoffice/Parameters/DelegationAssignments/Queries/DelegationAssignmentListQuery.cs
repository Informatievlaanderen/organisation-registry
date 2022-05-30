namespace OrganisationRegistry.Api.Backoffice.Parameters.DelegationAssignments.Queries;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure.Search;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Sorting;
using SqlServer.DelegationAssignments;
using SqlServer.Infrastructure;

public class DelegationAssignmentListQueryResult
{
    public Guid Id { get; }

    public Guid PersonId { get; }
    public string? PersonName { get; }

    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    public DelegationAssignmentListQueryResult(
        Guid id,
        Guid personId,
        string? personName,
        DateTime? validFrom,
        DateTime? validTo)
    {
        Id = id;
        PersonId = personId;
        PersonName = personName;

        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}

public class DelegationAssignmentListQuery : Query<DelegationAssignmentListItem, DelegationAssignmentListItemFilter, DelegationAssignmentListQueryResult>
{
    private readonly OrganisationRegistryContext _context;
    private readonly Guid _delegationId;

    protected override ISorting Sorting => new DelegationAssignmentListSorting();

    protected override Expression<Func<DelegationAssignmentListItem, DelegationAssignmentListQueryResult>> Transformation =>
        x => new DelegationAssignmentListQueryResult(
            x.Id,
            x.PersonId,
            x.PersonName,
            x.ValidFrom,
            x.ValidTo);

    public DelegationAssignmentListQuery(OrganisationRegistryContext context, Guid delegationId)
    {
        _context = context;
        _delegationId = delegationId;
    }

    protected override IQueryable<DelegationAssignmentListItem> Filter(FilteringHeader<DelegationAssignmentListItemFilter> filtering)
    {
        return _context.DelegationAssignmentList
            .AsQueryable()
            .Where(x => x.BodyMandateId == _delegationId);
    }

    private class DelegationAssignmentListSorting : ISorting
    {
        public IEnumerable<string> SortableFields { get; } = new[]
        {
            nameof(DelegationAssignmentListItem.PersonName),
            nameof(DelegationAssignmentListItem.ValidFrom),
            nameof(DelegationAssignmentListItem.ValidTo),
        };

        public SortingHeader DefaultSortingHeader { get; } =
            new SortingHeader(nameof(DelegationAssignmentListItem.PersonName), SortOrder.Ascending);
    }
}

public class DelegationAssignmentListItemFilter
{
    public string? PersonName { get; set; }
}