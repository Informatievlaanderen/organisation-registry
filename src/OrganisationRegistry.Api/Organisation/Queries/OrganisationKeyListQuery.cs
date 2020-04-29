﻿namespace OrganisationRegistry.Api.Organisation.Queries
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

    public class OrganisationKeyListQueryResult
    {
        public Guid OrganisationKeyId { get; }
        public string KeyTypeName { get; }
        public string KeyValue { get; }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }

        public bool IsActive { get; }

        public OrganisationKeyListQueryResult(
            Guid organisationKeyId,
            string keyTypeName,
            string keyValue,
            DateTime? validFrom,
            DateTime? validTo)
        {
            OrganisationKeyId = organisationKeyId;
            KeyTypeName = keyTypeName;
            KeyValue = keyValue;
            ValidFrom = validFrom;
            ValidTo = validTo;

            IsActive = new Period(new ValidFrom(validFrom), new ValidTo(validTo)).OverlapsWith(DateTime.Today);
        }
    }

    public class OrganisationKeyListQuery : Query<OrganisationKeyListItem, OrganisationKeyListItemFilter, OrganisationKeyListQueryResult>
    {
        private readonly OrganisationRegistryContext _context;
        private readonly Guid _organisationId;

        protected override ISorting Sorting => new OrganisationKeyListSorting();

        protected override Expression<Func<OrganisationKeyListItem, OrganisationKeyListQueryResult>> Transformation =>
            x => new OrganisationKeyListQueryResult(
                x.OrganisationKeyId,
                x.KeyTypeName,
                x.KeyValue,
                x.ValidFrom,
                x.ValidTo);

        public OrganisationKeyListQuery(OrganisationRegistryContext context, Guid organisationId)
        {
            _context = context;
            _organisationId = organisationId;
        }

        protected override IQueryable<OrganisationKeyListItem> Filter(FilteringHeader<OrganisationKeyListItemFilter> filtering)
        {
            var organisationKeys = _context.OrganisationKeyList
                .AsQueryable()
                .Where(x => x.OrganisationId == _organisationId).AsQueryable();

            if (!filtering.ShouldFilter)
                return organisationKeys;

            if (filtering.Filter.ActiveOnly)
                organisationKeys = organisationKeys.Where(x =>
                    (!x.ValidFrom.HasValue || x.ValidFrom <= DateTime.Today) &&
                    (!x.ValidTo.HasValue || x.ValidTo >= DateTime.Today));

            return organisationKeys;
        }

        private class OrganisationKeyListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(OrganisationKeyListItem.KeyTypeName),
                nameof(OrganisationKeyListItem.KeyValue),
                nameof(OrganisationKeyListItem.ValidFrom),
                nameof(OrganisationKeyListItem.ValidTo)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(OrganisationKeyListItem.KeyTypeName), SortOrder.Ascending);
        }
    }

    public class OrganisationKeyListItemFilter
    {
        public bool ActiveOnly { get; set; }
    }
}
