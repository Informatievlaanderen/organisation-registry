namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.OrganisationBankAccounts
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Organisation;
    using OrganisationRegistry.Organisation.Events;
    using TestBases;
    using Xunit;

    [Collection(SqlServerTestsCollection.Name)]
    public class OrganisationBankAccountListViewTests : ListViewTestBase
    {
        public OrganisationBankAccountListViewTests(SqlServerFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void BankAccountAdded()
        {
            var organisationBankAccountAdded1 = new OrganisationBankAccountAdded(Guid.NewGuid(), Guid.NewGuid(),
                string.Empty, true,
                string.Empty, true, DateTime.Today, DateTime.Today);

            var organisationBankAccountAdded2 = new OrganisationBankAccountAdded(Guid.NewGuid(), Guid.NewGuid(),
                string.Empty, true,
                string.Empty, true, DateTime.Today, DateTime.Today);

            HandleEvents(
                organisationBankAccountAdded1,
                organisationBankAccountAdded2);

            Context.OrganisationBankAccountList.Should().BeEquivalentTo(new List<OrganisationBankAccountListItem>
            {
                new OrganisationBankAccountListItem(
                    organisationBankAccountAdded1.OrganisationBankAccountId,
                    organisationBankAccountAdded1.OrganisationId,
                    organisationBankAccountAdded1.BankAccountNumber,
                    organisationBankAccountAdded1.IsIban,
                    organisationBankAccountAdded1.Bic,
                    organisationBankAccountAdded1.IsBic,
                    organisationBankAccountAdded1.ValidFrom,
                    organisationBankAccountAdded1.ValidTo,
                    null),
                new OrganisationBankAccountListItem(
                    organisationBankAccountAdded2.OrganisationBankAccountId,
                    organisationBankAccountAdded2.OrganisationId,
                    organisationBankAccountAdded2.BankAccountNumber,
                    organisationBankAccountAdded2.IsIban,
                    organisationBankAccountAdded2.Bic,
                    organisationBankAccountAdded2.IsBic,
                    organisationBankAccountAdded2.ValidFrom,
                    organisationBankAccountAdded2.ValidTo,
                    null),
            });
        }

        [Fact]
        public void KboBankAccountAdded()
        {
            var kboOrganisationBankAccountAdded1 = new KboOrganisationBankAccountAdded(Guid.NewGuid(), Guid.NewGuid(),
                string.Empty, true,
                string.Empty, true, DateTime.Today, DateTime.Today);

            var kboOrganisationBankAccountAdded2 = new KboOrganisationBankAccountAdded(Guid.NewGuid(), Guid.NewGuid(),
                string.Empty, true,
                string.Empty, true, DateTime.Today, DateTime.Today);

            HandleEvents(
                kboOrganisationBankAccountAdded1,
                kboOrganisationBankAccountAdded2);

            Context.OrganisationBankAccountList.Should().BeEquivalentTo(new List<OrganisationBankAccountListItem>
            {
                new OrganisationBankAccountListItem(
                    kboOrganisationBankAccountAdded1.OrganisationBankAccountId,
                    kboOrganisationBankAccountAdded1.OrganisationId,
                    kboOrganisationBankAccountAdded1.BankAccountNumber,
                    kboOrganisationBankAccountAdded1.IsIban,
                    kboOrganisationBankAccountAdded1.Bic,
                    kboOrganisationBankAccountAdded1.IsBic,
                    kboOrganisationBankAccountAdded1.ValidFrom,
                    kboOrganisationBankAccountAdded1.ValidTo,
                    Sources.Kbo),
                new OrganisationBankAccountListItem(
                    kboOrganisationBankAccountAdded2.OrganisationBankAccountId,
                    kboOrganisationBankAccountAdded2.OrganisationId,
                    kboOrganisationBankAccountAdded2.BankAccountNumber,
                    kboOrganisationBankAccountAdded2.IsIban,
                    kboOrganisationBankAccountAdded2.Bic,
                    kboOrganisationBankAccountAdded2.IsBic,
                    kboOrganisationBankAccountAdded2.ValidFrom,
                    kboOrganisationBankAccountAdded2.ValidTo,
                    Sources.Kbo),
            });
        }

        [Fact]
        public void KboBankAccountRemoved()
        {
            var kboOrganisationBankAccountAdded1 = new KboOrganisationBankAccountAdded(Guid.NewGuid(), Guid.NewGuid(),
                string.Empty, true,
                string.Empty, true, DateTime.Today, DateTime.Today);

            var kboOrganisationBankAccountAdded2 = new KboOrganisationBankAccountAdded(Guid.NewGuid(), Guid.NewGuid(),
                string.Empty, true,
                string.Empty, true, DateTime.Today, DateTime.Today);

            var kboOrganisationBankAccountRemoved = new KboOrganisationBankAccountRemoved(
                kboOrganisationBankAccountAdded1.OrganisationId,
                kboOrganisationBankAccountAdded1.OrganisationBankAccountId,
                kboOrganisationBankAccountAdded1.BankAccountNumber,
                kboOrganisationBankAccountAdded1.IsIban,
                kboOrganisationBankAccountAdded1.Bic,
                kboOrganisationBankAccountAdded1.IsBic,
                kboOrganisationBankAccountAdded1.ValidFrom,
                kboOrganisationBankAccountAdded1.ValidTo);

            HandleEvents(
                kboOrganisationBankAccountAdded1,
                kboOrganisationBankAccountAdded2,
                kboOrganisationBankAccountRemoved);

            Context.OrganisationBankAccountList.Should().BeEquivalentTo(new List<OrganisationBankAccountListItem>
            {
                new OrganisationBankAccountListItem(
                    kboOrganisationBankAccountAdded2.OrganisationBankAccountId,
                    kboOrganisationBankAccountAdded2.OrganisationId,
                    kboOrganisationBankAccountAdded2.BankAccountNumber,
                    kboOrganisationBankAccountAdded2.IsIban,
                    kboOrganisationBankAccountAdded2.Bic,
                    kboOrganisationBankAccountAdded2.IsBic,
                    kboOrganisationBankAccountAdded2.ValidFrom,
                    kboOrganisationBankAccountAdded2.ValidTo,
                    Sources.Kbo),
            });
        }
    }
}
