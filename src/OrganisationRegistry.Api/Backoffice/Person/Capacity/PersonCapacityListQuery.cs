namespace OrganisationRegistry.Api.Backoffice.Person.Capacity;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using OrganisationRegistry.Api.Infrastructure.Search;
using OrganisationRegistry.Api.Infrastructure.Search.Filtering;
using OrganisationRegistry.Api.Infrastructure.Search.Sorting;
using OrganisationRegistry.SqlServer.Infrastructure;
using OrganisationRegistry.SqlServer.Person;

public class PersonCapacityListQueryResult
{
    public string CapacityName { get; }
    public string? FunctionName { get; }
    public Guid OrganisationId { get; }
    public string OrganisationName { get; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    public PersonCapacityListQueryResult(
        string capacityName,
        string? functionName,
        Guid organisationId,
        string organisationName,
        DateTime? validFrom,
        DateTime? validTo)
    {
        CapacityName = capacityName;
        FunctionName = functionName;
        OrganisationId = organisationId;
        OrganisationName = organisationName;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}

public class PersonCapacityListQuery : Query<PersonCapacityListItem, PersonCapacityListItem, PersonCapacityListQueryResult>
{
    private readonly OrganisationRegistryContext _context;
    private readonly Guid _personId;

    protected override ISorting Sorting => new PersonCapacityListSorting();

    protected override Expression<Func<PersonCapacityListItem, PersonCapacityListQueryResult>> Transformation =>
        x => new PersonCapacityListQueryResult(
            x.CapacityName,
            x.FunctionName,
            x.OrganisationId,
            x.OrganisationName,
            x.ValidFrom,
            x.ValidTo);

    public PersonCapacityListQuery(OrganisationRegistryContext context, Guid personId)
    {
        _context = context;
        _personId = personId;
    }

    protected override IQueryable<PersonCapacityListItem> Filter(FilteringHeader<PersonCapacityListItem> filtering)
        => _context.PersonCapacityList
            .AsQueryable()
            .Where(x => x.PersonId == _personId).AsQueryable();

    private class PersonCapacityListSorting : ISorting
    {
        public IEnumerable<string> SortableFields { get; } = new[]
        {
            nameof(PersonCapacityListItem.OrganisationName),
            nameof(PersonCapacityListItem.CapacityName),
            nameof(PersonCapacityListItem.FunctionName),
            nameof(PersonCapacityListItem.ValidFrom),
            nameof(PersonCapacityListItem.ValidTo),
        };

        public SortingHeader DefaultSortingHeader { get; } =
            new(nameof(PersonCapacityListItem.OrganisationName), SortOrder.Ascending);
    }
}
