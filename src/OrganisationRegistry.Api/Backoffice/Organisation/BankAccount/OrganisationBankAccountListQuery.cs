namespace OrganisationRegistry.Api.Backoffice.Organisation.BankAccount
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using SqlServer.Infrastructure;
    using SqlServer.Organisation;

    public class OrganisationBankAccountListQueryResult
    {
        public Guid OrganisationBankAccountId { get; }
        public string BankAccountNumber { get; }
        public bool IsIban { get; }
        public string? Bic { get; }
        public bool IsBic { get; }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }

        public bool IsActive { get; }

        public bool IsEditable { get; }

        public OrganisationBankAccountListQueryResult(
            Guid organisationBankAccountId,
            string bankAccountNumber,
            bool isIban,
            string? bic,
            bool isBic,
            DateTime? validFrom,
            DateTime? validTo,
            bool isEditable)
        {
            OrganisationBankAccountId = organisationBankAccountId;
            BankAccountNumber = bankAccountNumber;
            IsIban = isIban;
            Bic = bic;
            IsBic = isBic;
            ValidFrom = validFrom;
            ValidTo = validTo;

            IsActive = new Period(new ValidFrom(validFrom), new ValidTo(validTo)).OverlapsWith(DateTime.Today);
            IsEditable = isEditable;
        }
    }

    public class OrganisationBankAccountListQuery : Query<OrganisationBankAccountListItem, OrganisationBankAccountListItemFilter, OrganisationBankAccountListQueryResult>
    {
        private readonly OrganisationRegistryContext _context;
        private readonly Guid _organisationId;

        protected override ISorting Sorting => new OrganisationBankAccountListSorting();

        protected override Expression<Func<OrganisationBankAccountListItem, OrganisationBankAccountListQueryResult>> Transformation =>
            x => new OrganisationBankAccountListQueryResult(
                x.OrganisationBankAccountId,
                x.BankAccountNumber,
                x.IsIban,
                x.Bic,
                x.IsBic,
                x.ValidFrom,
                x.ValidTo,
                x.IsEditable);

        public OrganisationBankAccountListQuery(OrganisationRegistryContext context, Guid organisationId)
        {
            _context = context;
            _organisationId = organisationId;
        }

        protected override IQueryable<OrganisationBankAccountListItem> Filter(FilteringHeader<OrganisationBankAccountListItemFilter> filtering)
        {
            var organisationBankAccounts = _context.OrganisationBankAccountList
                .AsQueryable()
                .Where(x => x.OrganisationId == _organisationId).AsQueryable();

            if (filtering.Filter is not { } filter)
                return organisationBankAccounts;

            if (filter.ActiveOnly)
                organisationBankAccounts = organisationBankAccounts.Where(x =>
                    (!x.ValidFrom.HasValue || x.ValidFrom <= DateTime.Today) &&
                    (!x.ValidTo.HasValue || x.ValidTo >= DateTime.Today));

            return organisationBankAccounts;
        }

        private class OrganisationBankAccountListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(OrganisationBankAccountListItem.BankAccountNumber),
                nameof(OrganisationBankAccountListItem.IsIban),
                nameof(OrganisationBankAccountListItem.Bic),
                nameof(OrganisationBankAccountListItem.IsBic),
                nameof(OrganisationBankAccountListItem.ValidFrom),
                nameof(OrganisationBankAccountListItem.ValidTo)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(OrganisationBankAccountListItem.BankAccountNumber), SortOrder.Ascending);
        }
    }

    public class OrganisationBankAccountListItemFilter
    {
        public bool ActiveOnly { get; set; }
    }
}
