namespace OrganisationRegistry.Api.Backoffice.Person.Queries
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Linq.Expressions;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
    using Infrastructure;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using SqlServer.Infrastructure;
    using SqlServer.Person;

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

            if (!filter.FirstName.IsNullOrWhiteSpace())
                people = people.Where(x => x.FirstName.Contains(filter.FirstName));

            if (!filter.Name.IsNullOrWhiteSpace())
                people = people.Where(x => x.Name.Contains(filter.Name));

            if (!filter.FullName.IsNullOrWhiteSpace())
                people = people.Where(x => x.FullName.Contains(filter.FullName));

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
        public string FirstName { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
    }
}
