namespace OrganisationRegistry.Api.Backoffice.Parameters.Delegations.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using OrganisationRegistry.Infrastructure.Authorization;
    using SqlServer.Delegations;
    using SqlServer.Infrastructure;

    public class DelegationListQueryResult
    {
        public Guid Id { get; }

        public Guid OrganisationId { get; }
        public string? OrganisationName { get; }

        public Guid? FunctionTypeId { get; }
        public string? FunctionTypeName { get; }

        public Guid BodyId { get; }
        public string? BodyName { get; }

        public Guid? BodyOrganisationId { get; }
        public string? BodyOrganisationName { get; }

        public string? BodySeatName { get; }
        public string? BodySeatNumber { get; }
        public string? BodySeatTypeName { get; }

        public bool IsDelegated { get; }

        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }

        public DelegationListQueryResult(Guid id,
            Guid organisationId,
            string? organisationName,
            Guid? functionTypeId,
            string? functionTypeName,
            Guid bodyId,
            string? bodyName,
            Guid? bodyOrganisationId,
            string? bodyOrganisationName,
            string? bodySeatName,
            string? bodySeatNumber,
            string? bodySeatTypeName,
            bool isDelegated,
            DateTime? validFrom,
            DateTime? validTo)
        {
            Id = id;
            OrganisationId = organisationId;
            OrganisationName = organisationName;
            BodyId = bodyId;
            BodyName = bodyName;
            BodySeatName = bodySeatName;
            IsDelegated = isDelegated;
            ValidFrom = validFrom;
            ValidTo = validTo;
            BodySeatNumber = bodySeatNumber;
            BodySeatTypeName = bodySeatTypeName;
            FunctionTypeId = functionTypeId;
            FunctionTypeName = functionTypeName;
            BodyOrganisationId = bodyOrganisationId;
            BodyOrganisationName = bodyOrganisationName;
        }
    }

    public class DelegationListQuery : Query<DelegationListItem, DelegationListItemFilter, DelegationListQueryResult>
    {
        private readonly OrganisationRegistryContext _context;
        private readonly SecurityInformation _securityInformation;

        protected override ISorting Sorting => new DelegationListSorting();

        protected override Expression<Func<DelegationListItem, DelegationListQueryResult>> Transformation =>
            x => new DelegationListQueryResult(
                x.Id,
                x.OrganisationId,
                x.OrganisationName,
                x.FunctionTypeId,
                x.FunctionTypeName,
                x.BodyId,
                x.BodyName,
                x.BodyOrganisationId,
                x.BodyOrganisationName,
                x.BodySeatName,
                x.BodySeatNumber,
                x.BodySeatTypeName,
                x.IsDelegated,
                x.ValidFrom,
                x.ValidTo);

        public DelegationListQuery(OrganisationRegistryContext context, SecurityInformation securityInformation)
        {
            _context = context;
            _securityInformation = securityInformation;
        }

        protected override IQueryable<DelegationListItem> Filter(FilteringHeader<DelegationListItemFilter> filtering)
        {
            var delegations = _context.DelegationList.AsQueryable();

            // Only show relevant delegations,
            //  - OrganisationRegistryBeheerder can see everything, so we dont reduce
            //  - OrganisatieBeheerder can see only bodies owned by his organisation
            if (_securityInformation.Roles.Contains(Role.DecentraalBeheerder) && !_securityInformation.Roles.Contains(Role.AlgemeenBeheerder))
            {
                // If there are no organisations, prevent sending all delegations
                if (_securityInformation.OrganisationIds.Count == 0)
                    return new List<DelegationListItem>().AsAsyncQueryable();

                // https://github.com/aspnet/EntityFramework/issues/4114
                // EF cannot deal with securityInformation.OrganisationIds.Contains(x.BodyOrganisationId.Value)
                // It tries to evaluate it locally. You first have to turn a Guid list into a Nullable Guid list to get the proper SQL
                var organisationIds = _securityInformation.OrganisationIds.Select(organisationId => (Guid?) organisationId).ToList();

                delegations = delegations
                    .Where(x => organisationIds.Contains(x.BodyOrganisationId));
            }

            if (filtering.Filter is not { } filter)
                return delegations;

            if (filter.BodyName is { } bodyName && bodyName.IsNotEmptyOrWhiteSpace())
                delegations = delegations.Where(x => x.BodyName != null && x.BodyName.Contains(bodyName));

            if (filter.BodyOrganisationName is { } bodyOrganisationName && bodyOrganisationName.IsNotEmptyOrWhiteSpace())
                delegations = delegations.Where(x => x.BodyOrganisationName != null && x.BodyOrganisationName.Contains(bodyOrganisationName));

            if (filter.OrganisationName is { } organisationName && organisationName.IsNotEmptyOrWhiteSpace())
                delegations = delegations.Where(x => x.OrganisationName != null && x.OrganisationName.Contains(organisationName));

            if (filter.FunctionTypeName is { } functionTypeName && functionTypeName.IsNotEmptyOrWhiteSpace())
                delegations = delegations.Where(x => x.FunctionTypeName != null && x.FunctionTypeName.Contains(functionTypeName));

            if (filter.BodySeatName is { } bodySeatName && bodySeatName.IsNotEmptyOrWhiteSpace())
                delegations = delegations.Where(x => x.BodySeatName != null && x.BodySeatName.Contains(bodySeatName));

            if (filter.BodySeatNumber is { } bodySeatNumber && bodySeatNumber.IsNotEmptyOrWhiteSpace())
                delegations = delegations.Where(x => x.BodySeatNumber != null && x.BodySeatNumber.Contains(filter.BodySeatNumber));

            if (filter.EmptyDelegationsOnly)
                delegations = delegations.Where(x => !x.IsDelegated);

            if (filter.ActiveMandatesOnly)
                delegations = delegations.Where(x =>
                    (!x.ValidFrom.HasValue || x.ValidFrom <= DateTime.Today) &&
                    (!x.ValidTo.HasValue || x.ValidTo >= DateTime.Today));

            return delegations;
        }

        private class DelegationListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(DelegationListItem.BodyName),
                nameof(DelegationListItem.BodyOrganisationName),
                nameof(DelegationListItem.OrganisationName),
                nameof(DelegationListItem.FunctionTypeName),
                nameof(DelegationListItem.BodySeatName),
                nameof(DelegationListItem.ValidFrom),
                nameof(DelegationListItem.ValidTo),
                nameof(DelegationListItem.IsDelegated),
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(DelegationListItem.BodyName), SortOrder.Ascending);
        }
    }

    public class DelegationListItemFilter
    {
        public string? BodyName { get; set; }
        public string? BodyOrganisationName { get; set; }
        public string? OrganisationName { get; set; }
        public string? FunctionTypeName { get; set; }
        public string? BodySeatName { get; set; }
        public string? BodySeatNumber { get; set; }
        public bool ActiveMandatesOnly { get; set; }
        public bool EmptyDelegationsOnly { get; set; }
    }
}
