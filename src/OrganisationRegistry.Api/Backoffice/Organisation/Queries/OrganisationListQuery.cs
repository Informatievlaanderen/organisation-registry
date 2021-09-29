namespace OrganisationRegistry.Api.Backoffice.Organisation.Queries
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
    using OrganisationRegistry.Infrastructure.Authorization;
    using SqlServer.Infrastructure;
    using SqlServer.Organisation;

    public class OrganisationListQueryResult
    {
        [ExcludeFromCsv]
        public Guid Id { get; }

        [DisplayName("OVO-nummer")]
        public string OvoNumber { get; }

        [DisplayName("Naam")]
        public string Name { get; }

        [DisplayName("Korte naam")]
        public string ShortName { get; }

        [DisplayName("OVO-nummer moeder entiteit")]
        public string ParentOrganisationOvoNumber { get; }

        [DisplayName("Moeder entiteit")]
        public string ParentOrganisation { get; }

        [ExcludeFromCsv]
        public Guid? ParentOrganisationId { get; }

        public OrganisationListQueryResult(
            Guid id,
            string ovoNumber,
            string name,
            string shortName,
            string parentOrganisation,
            Guid? parentOrganisationId,
            string parentOrganisationOvoNumber)
        {
            Id = id;
            OvoNumber = ovoNumber;
            Name = name;
            ShortName = shortName;
            ParentOrganisation = parentOrganisation;
            ParentOrganisationId = parentOrganisationId;
            ParentOrganisationOvoNumber = parentOrganisationOvoNumber;
        }
    }

    public class OrganisationListQuery : Query<OrganisationListItem, OrganisationListItemFilter, OrganisationListQueryResult>
    {
        private readonly OrganisationRegistryContext _context;
        private readonly Func<SecurityInformation> _securityInformationFunc;

        protected override ISorting Sorting => new OrganisationListSorting();

        protected override Expression<Func<OrganisationListItem, OrganisationListQueryResult>> Transformation =>
            x => new OrganisationListQueryResult(
                x.OrganisationId,
                x.OvoNumber,
                x.Name,
                x.ShortName,
                x.ParentOrganisation,
                x.ParentOrganisationId,
                x.ParentOrganisationOvoNumber);

        public OrganisationListQuery(OrganisationRegistryContext context, Func<SecurityInformation> securityInformationFunc)
        {
            _context = context;
            _securityInformationFunc = securityInformationFunc;
        }

        protected override IQueryable<OrganisationListItem> Filter(FilteringHeader<OrganisationListItemFilter> filtering)
        {
            var organisations = _context.OrganisationList.AsQueryable();

            if (!filtering.ShouldFilter)
                return organisations.Where(x => x.FormalFrameworkId == null);

            if (!filtering.Filter.Name.IsNullOrWhiteSpace())
                organisations = organisations.Where(x => x.Name.Contains(filtering.Filter.Name) || x.ShortName.Contains(filtering.Filter.Name));

            if (!filtering.Filter.OvoNumber.IsNullOrWhiteSpace())
                organisations = organisations.Where(x => x.OvoNumber.Contains(filtering.Filter.OvoNumber));

            if (filtering.Filter.ActiveOnly)
                organisations = organisations.Where(x =>
                    (!x.ValidFrom.HasValue || x.ValidFrom <= DateTime.Today) &&
                    (!x.ValidTo.HasValue || x.ValidTo >= DateTime.Today));

            organisations = filtering.Filter.FormalFrameworkId.IsEmptyGuid() ?
                organisations.Where(x => x.FormalFrameworkId == null) :
                organisations.Where(x => x.FormalFrameworkId == filtering.Filter.FormalFrameworkId);

            if (filtering.Filter.ActiveOnly && !filtering.Filter.FormalFrameworkId.IsEmptyGuid())
                organisations = organisations.Where(x =>
                    x.FormalFrameworkValidities.Any(y =>
                        (!y.ValidFrom.HasValue || y.ValidFrom <= DateTime.Today) &&
                        (!y.ValidTo.HasValue || y.ValidTo >= DateTime.Today)));

            if (!filtering.Filter.OrganisationClassificationId.IsEmptyGuid())
                organisations = organisations.Where(x =>
                    x.OrganisationClassificationValidities.Any(y =>
                        y.OrganisationClassificationId == filtering.Filter.OrganisationClassificationId &&
                        (!filtering.Filter.ActiveOnly ||
                         ((!y.ValidFrom.HasValue || y.ValidFrom <= DateTime.Today) &&
                          (!y.ValidTo.HasValue || y.ValidTo >= DateTime.Today)))));

            if (!filtering.Filter.OrganisationClassificationTypeId.IsEmptyGuid())
                organisations = organisations.Where(x =>
                    x.OrganisationClassificationValidities.Any(y =>
                        y.OrganisationClassificationTypeId == filtering.Filter.OrganisationClassificationTypeId &&
                        (!filtering.Filter.ActiveOnly ||
                         ((!y.ValidFrom.HasValue || y.ValidFrom <= DateTime.Today) &&
                         (!y.ValidTo.HasValue || y.ValidTo >= DateTime.Today)))));

            if (filtering.Filter.AuthorizedOnly)
            {
                var securityInformation = _securityInformationFunc();
                if (!securityInformation.Roles.Contains(Role.OrganisationRegistryBeheerder))
                    organisations = organisations.Where(x => securityInformation.OvoNumbers.Contains(x.OvoNumber));
            }

            return organisations;
        }

        private class OrganisationListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(OrganisationListItem.Name),
                nameof(OrganisationListItem.ParentOrganisation),
                nameof(OrganisationListItem.OvoNumber)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(OrganisationListItem.Name), SortOrder.Ascending);
        }
    }

    public class OrganisationListItemFilter
    {
        public string Name { get; set; }
        public string OvoNumber { get; set; }
        public Guid? FormalFrameworkId { get; set; }
        public Guid? OrganisationClassificationId { get; set; }
        public Guid? OrganisationClassificationTypeId { get; set; }
        public bool ActiveOnly { get; set; }
        public bool AuthorizedOnly { get; set; }
    }
}
