namespace OrganisationRegistry.Api.Backoffice.Person.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using SqlServer.Infrastructure;
    using SqlServer.Person;

    public class PersonFunctionListQueryResult
    {
        public Guid FunctionId { get; }
        public string FunctionName { get; }
        public Guid OrganisationId { get; }
        public string OrganisationName { get; }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }

        public PersonFunctionListQueryResult(
            Guid functionId,
            string functionName,
            Guid organisationId,
            string organisationName,
            DateTime? validFrom,
            DateTime? validTo)
        {
            FunctionId = functionId;
            FunctionName = functionName;
            OrganisationId = organisationId;
            OrganisationName = organisationName;
            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }

    public class PersonFunctionListQuery : Query<PersonFunctionListItem, PersonFunctionListItem, PersonFunctionListQueryResult>
    {
        private readonly OrganisationRegistryContext _context;
        private readonly Guid _personId;

        protected override ISorting Sorting => new PersonFunctionListSorting();

        protected override Expression<Func<PersonFunctionListItem, PersonFunctionListQueryResult>> Transformation =>
            x => new PersonFunctionListQueryResult(
                x.FunctionId,
                x.FunctionName,
                x.OrganisationId,
                x.OrganisationName,
                x.ValidFrom,
                x.ValidTo);

        public PersonFunctionListQuery(OrganisationRegistryContext context, Guid personId)
        {
            _context = context;
            _personId = personId;
        }

        protected override IQueryable<PersonFunctionListItem> Filter(FilteringHeader<PersonFunctionListItem> filtering)
            => _context.PersonFunctionList
                .AsQueryable()
                .Where(x => x.PersonId == _personId).AsQueryable();

        private class PersonFunctionListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(PersonFunctionListItem.OrganisationName),
                nameof(PersonFunctionListItem.FunctionName),
                nameof(PersonFunctionListItem.ValidFrom),
                nameof(PersonFunctionListItem.ValidTo)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(PersonFunctionListItem.OrganisationName), SortOrder.Ascending);
        }
    }
}
