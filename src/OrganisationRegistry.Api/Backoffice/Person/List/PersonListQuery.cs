namespace OrganisationRegistry.Api.Backoffice.Person.List;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Search;
using OrganisationRegistry.Api.Infrastructure.Search.Filtering;
using OrganisationRegistry.Api.Infrastructure.Search.Sorting;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.SqlServer.Infrastructure;
using OrganisationRegistry.SqlServer.Person;

public class PersonListQueryResult
{
    [ExcludeFromCsv]
    public Guid Id { get; }

    [DisplayName("Voornaam")]
    public string FirstName { get; }

    [DisplayName("Naam")]
    public string Name { get; }

    public PersonListQueryResult(
        Guid id,
        string firstName,
        string name)
    {
        Id = id;
        FirstName = firstName;
        Name = name;
    }
}

public class PersonListQuery : Query<PersonListItem, PersonListItemFilter, PersonListQueryResult>
{
    private readonly OrganisationRegistryContext _context;

    protected override ISorting Sorting => new PersonListSorting();

    protected override Expression<Func<PersonListItem, PersonListQueryResult>> Transformation =>
        x => new PersonListQueryResult(
            x.Id,
            x.FirstName,
            x.Name);

    public PersonListQuery(OrganisationRegistryContext context)
    {
        _context = context;
    }

    protected override IQueryable<PersonListItem> Filter(FilteringHeader<PersonListItemFilter> filtering)
    {
        var people = _context.PersonList.AsQueryable();

        if (filtering.Filter is not { } filter)
            return people;

        if (filter.FirstName is { } firstName && firstName.IsNotEmptyOrWhiteSpace())
            people = people.Where(x => x.FirstName.Contains(firstName));

        if (filter.Name is { } name && name.IsNotEmptyOrWhiteSpace())
            people = people.Where(x => x.Name.Contains(name));

        if (filter.FullName is { } fullName && fullName.IsNotEmptyOrWhiteSpace())
            people = people.Where(x => x.FullName.Contains(fullName));

        return people;
    }

    private class PersonListSorting : ISorting
    {
        public IEnumerable<string> SortableFields { get; } = new[]
        {
            nameof(PersonListItem.Name),
            nameof(PersonListItem.FirstName),
        };

        public SortingHeader DefaultSortingHeader { get; } =
            new SortingHeader(nameof(PersonListItem.Name), SortOrder.Ascending);
    }
}

public class PersonListItemFilter
{
    public string? FirstName { get; set; }
    public string? Name { get; set; }
    public string? FullName { get; set; }
}
