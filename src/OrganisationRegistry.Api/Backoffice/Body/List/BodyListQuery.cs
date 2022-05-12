namespace OrganisationRegistry.Api.Backoffice.Body.List
{
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
    using OrganisationRegistry.SqlServer.Body;
    using OrganisationRegistry.SqlServer.Infrastructure;

    public class BodyListQueryResult
    {
        [ExcludeFromCsv]
        public Guid Id { get; }

        [DisplayName("Orgaan nummer")]
        public string? BodyNumber { get; }

        [DisplayName("Naam")]
        public string Name { get; }

        [DisplayName("Korte naam")]
        public string? ShortName { get; }

        [ExcludeFromCsv]
        public Guid? OrganisationId { get; }

        [DisplayName("Organisatie")]
        public string? Organisation { get; }

        public BodyListQueryResult(
            Guid id,
            string? bodyNumber,
            string name,
            string? shortName,
            Guid? organisationId,
            string? organisation)
        {
            Id = id;
            BodyNumber = bodyNumber;
            Name = name;
            ShortName = shortName;
            OrganisationId = organisationId;
            Organisation = organisation;
        }
    }

    public class BodyListQuery : Query<BodyListItem, BodyListItemFilter, BodyListQueryResult>
    {
        private readonly OrganisationRegistryContext _context;

        protected override ISorting Sorting => new BodyListSorting();

        protected override Expression<Func<BodyListItem, BodyListQueryResult>> Transformation =>
            x => new BodyListQueryResult(
                x.Id,
                x.BodyNumber,
                x.Name,
                x.ShortName,
                x.OrganisationId,
                x.Organisation);

        public BodyListQuery(OrganisationRegistryContext context)
        {
            _context = context;
        }

        protected override IQueryable<BodyListItem> Filter(FilteringHeader<BodyListItemFilter> filtering)
        {
            var bodies = _context.BodyList.AsQueryable();

            if (filtering.Filter is not { } filter)
                return bodies;

            if (filter.Name is { } name && name.IsNotEmptyOrWhiteSpace())
                bodies = bodies.Where(x => x.Name.Contains(name) || (x.ShortName != null && x.ShortName.Contains(name)));

            if (filter.Organisation is { } organisation && organisation.IsNotEmptyOrWhiteSpace())
                bodies = bodies.Where(x => x.Organisation != null && x.Organisation.Contains(organisation));

            if (filter.ActiveOnly)
                bodies = bodies.Where(x =>
                    x.BodyLifecyclePhaseValidities.Any(y =>
                        y.RepresentsActivePhase &&
                        (!y.ValidFrom.HasValue || y.ValidFrom <= DateTime.Today) &&
                        (!y.ValidTo.HasValue || y.ValidTo >= DateTime.Today)));

            return bodies;
        }

        private class BodyListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(BodyListItem.BodyNumber),
                nameof(BodyListItem.Name),
                nameof(BodyListItem.Organisation)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(BodyListItem.Name), SortOrder.Ascending);
        }
    }

    public class BodyListItemFilter
    {
        public string? Name { get; set; }
        public string? Organisation { get; set; }
        public bool ActiveOnly { get; set; }
    }
}
