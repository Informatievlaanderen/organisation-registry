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

    public class OrganisationCapacityListQueryResult
    {
        public Guid OrganisationCapacityId { get; }
        public string CapacityName { get; }
        public string FunctionName { get; }
        public Guid? PersonId { get; }
        public string PersonName { get; }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }

        public bool IsActive { get; }

        public OrganisationCapacityListQueryResult(
            Guid organisationCapacityId,
            string capacityName,
            string functionName,
            Guid? personId,
            string personName,
            DateTime? validFrom,
            DateTime? validTo)
        {
            OrganisationCapacityId = organisationCapacityId;
            CapacityName = capacityName;
            FunctionName = functionName;
            PersonId = personId;
            PersonName = personName;
            ValidFrom = validFrom;
            ValidTo = validTo;

            IsActive = new Period(new ValidFrom(validFrom), new ValidTo(validTo)).OverlapsWith(DateTime.Today);
        }
    }

    public class OrganisationCapacityListQuery : Query<OrganisationCapacityListItem, OrganisationCapacityListItemFilter, OrganisationCapacityListQueryResult>
    {
        private readonly OrganisationRegistryContext _context;
        private readonly Guid _organisationId;

        protected override ISorting Sorting => new OrganisationCapacityListSorting();

        protected override Expression<Func<OrganisationCapacityListItem, OrganisationCapacityListQueryResult>> Transformation =>
            x => new OrganisationCapacityListQueryResult(
                x.OrganisationCapacityId,
                x.CapacityName,
                x.FunctionName,
                x.PersonId,
                x.PersonName,
                x.ValidFrom,
                x.ValidTo);

        public OrganisationCapacityListQuery(OrganisationRegistryContext context, Guid organisationId)
        {
            _context = context;
            _organisationId = organisationId;
        }

        protected override IQueryable<OrganisationCapacityListItem> Filter(FilteringHeader<OrganisationCapacityListItemFilter> filtering)
        {
            var organisationCapacities = _context.OrganisationCapacityList.Where(x => x.OrganisationId == _organisationId).AsQueryable();

            if (!filtering.ShouldFilter)
                return organisationCapacities;

            if (filtering.Filter.ActiveOnly)
                organisationCapacities = organisationCapacities.Where(x =>
                    (!x.ValidFrom.HasValue || x.ValidFrom <= DateTime.Today) &&
                    (!x.ValidTo.HasValue || x.ValidTo >= DateTime.Today));

            return organisationCapacities;
        }

        private class OrganisationCapacityListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(OrganisationCapacityListItem.CapacityName),
                nameof(OrganisationCapacityListItem.PersonName),
                nameof(OrganisationCapacityListItem.FunctionName),
                nameof(OrganisationCapacityListItem.ValidFrom),
                nameof(OrganisationCapacityListItem.ValidTo)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(OrganisationCapacityListItem.CapacityName), SortOrder.Ascending);
        }
    }

    public class OrganisationCapacityListItemFilter
    {
        public bool ActiveOnly { get; set; }
    }
}
