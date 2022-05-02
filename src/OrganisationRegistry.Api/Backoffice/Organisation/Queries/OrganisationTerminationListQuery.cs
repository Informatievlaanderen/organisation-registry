namespace OrganisationRegistry.Api.Backoffice.Organisation.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using OrganisationRegistry.Infrastructure;
    using SqlServer.Infrastructure;
    using SqlServer.Organisation;

    public class OrganisationTermination
    {
        public Guid Id { get; }
        public string Name { get; }
        public string? KboNumber { get; }
        public DateTime Date { get; }
        public string Code { get; }
        public string Reason { get; }

        private OrganisationTermination(Guid id, string name, string? kboNumber, in DateTime date, string code, string reason)
        {
            Id = id;
            Name = name;
            KboNumber = kboNumber;
            Date = date;
            Code = code;
            Reason = reason;
        }

        public OrganisationTermination(OrganisationTerminationListItem x)
        	: this(
	            x.Id,
	            x.Name,
	            x.KboNumber,
	            x.Date,
	            x.Code,
	            x.Reason) { }
    }

    public class OrganisationTerminationListQuery: Query<OrganisationTerminationListItem, OrganisationTerminationListItemFilter, OrganisationTermination>
    {
        private readonly OrganisationRegistryContext _context;

        protected override ISorting Sorting => new OrganisationTerminationListSorting();

        protected override Expression<Func<OrganisationTerminationListItem, OrganisationTermination>> Transformation =>
            x => new OrganisationTermination(x);

        public OrganisationTerminationListQuery(OrganisationRegistryContext context)
        {
            _context = context;
        }

        protected override IQueryable<OrganisationTerminationListItem> Filter(FilteringHeader<OrganisationTerminationListItemFilter> filtering)
        {
            var organisationTerminations = _context.OrganisationTerminationList.AsQueryable();

            if (filtering.Filter is not { } filter)
                return organisationTerminations;

            if (filter.Name is { } name && name.IsNotEmptyOrWhiteSpace())
                organisationTerminations = organisationTerminations.Where(x => x.Name.Contains(name));

            return organisationTerminations;
        }

        private class OrganisationTerminationListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(OrganisationTerminationListItem.Name),
                nameof(OrganisationTerminationListItem.KboNumber),
                nameof(OrganisationTerminationListItem.Date),
                nameof(OrganisationTerminationListItem.Code),
                nameof(OrganisationTerminationListItem.Reason),
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(OrganisationTerminationListItem.Date), SortOrder.Descending);
        }
    }

    public class OrganisationTerminationListItemFilter
    {
        public string? Name { get; set; }
        public string? KboNumber { get; set; }
        public DateTime Date { get; set; }
        public string? Code { get; set; }
        public string? Reason { get; set; }
    }
}
