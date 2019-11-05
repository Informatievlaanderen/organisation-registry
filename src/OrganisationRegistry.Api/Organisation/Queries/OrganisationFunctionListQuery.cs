namespace OrganisationRegistry.Api.Organisation.Queries
{
    using System.Collections.Generic;
    using System.Linq;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using SqlServer.Infrastructure;
    using SqlServer.Organisation;
    using System;
    using System.Linq.Expressions;

    public class OrganisationFunctionListQueryResult
    {
        public string FunctionName { get; }
        public Guid OrganisationFunctionId { get; }
        public Guid PersonId { get; }
        public string PersonName { get; }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }

        public bool IsActive { get; }

        public OrganisationFunctionListQueryResult(
            Guid organisationFunctionId,
            Guid personId,
            string personName,
            string functionName,
            DateTime? validFrom,
            DateTime? validTo)
        {
            OrganisationFunctionId = organisationFunctionId;
            PersonId = personId;
            PersonName = personName;
            FunctionName = functionName;
            ValidFrom = validFrom;
            ValidTo = validTo;

            IsActive = new Period(new ValidFrom(validFrom), new ValidTo(validTo)).OverlapsWith(DateTime.Today);
        }
    }

    public class OrganisationFunctionListQuery : Query<OrganisationFunctionListItem, OrganisationFunctionListItemFilter, OrganisationFunctionListQueryResult>
    {
        private readonly OrganisationRegistryContext _context;
        private readonly Guid _organisationId;

        protected override ISorting Sorting => new OrganisationFunctionListSorting();

        protected override Expression<Func<OrganisationFunctionListItem, OrganisationFunctionListQueryResult>> Transformation =>
            x => new OrganisationFunctionListQueryResult(
                x.OrganisationFunctionId,
                x.PersonId,
                x.PersonName,
                x.FunctionName,
                x.ValidFrom,
                x.ValidTo);

        public OrganisationFunctionListQuery(OrganisationRegistryContext context, Guid organisationId)
        {
            _context = context;
            _organisationId = organisationId;
        }

        protected override IQueryable<OrganisationFunctionListItem> Filter(FilteringHeader<OrganisationFunctionListItemFilter> filtering)
        {
            var organisationFunctions = _context.OrganisationFunctionList.Where(x => x.OrganisationId == _organisationId).AsQueryable();

            if (!filtering.ShouldFilter)
                return organisationFunctions;

            if (filtering.Filter.ActiveOnly)
                organisationFunctions = organisationFunctions.Where(x =>
                    (!x.ValidFrom.HasValue || x.ValidFrom <= DateTime.Today) &&
                    (!x.ValidTo.HasValue || x.ValidTo >= DateTime.Today));

            return organisationFunctions;
        }

        private class OrganisationFunctionListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(OrganisationFunctionListItem.FunctionName),
                nameof(OrganisationFunctionListItem.PersonName),
                nameof(OrganisationFunctionListItem.ValidFrom),
                nameof(OrganisationFunctionListItem.ValidTo)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(OrganisationFunctionListItem.PersonName), SortOrder.Ascending);
        }
    }

    public class OrganisationFunctionListItemFilter
    {
        public bool ActiveOnly { get; set; }
    }
}
