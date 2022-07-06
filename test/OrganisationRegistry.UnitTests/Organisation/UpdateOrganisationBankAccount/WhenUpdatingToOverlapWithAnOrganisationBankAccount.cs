namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationBankAccount;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared;
using OrganisationRegistry.UnitTests.Infrastructure.Tests.Extensions.TestHelpers;
using Xunit;
using Xunit.Abstractions;

public class WhenUpdatingToOverlapWithAnOrganisationBankAccount
    : Specification<UpdateOrganisationBankAccountCommandHandler, UpdateOrganisationBankAccount>
{
    private readonly Guid _organisationId;
    private readonly Guid _organisationBankAccountId;

    public WhenUpdatingToOverlapWithAnOrganisationBankAccount(ITestOutputHelper helper) : base(helper)
    {
        _organisationBankAccountId = Guid.NewGuid();
        _organisationId = Guid.NewGuid();
    }

    protected override UpdateOrganisationBankAccountCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateOrganisationBankAccountCommandHandler>>().Object,
            session);

    protected IEvent[] Events
        => new IEvent[]
        {
            new OrganisationCreated(
                _organisationId,
                "Kind en Gezin",
                "OVO000012345",
                "K&G",
                Article.None,
                "Kindjes en gezinnetjes",
                new List<Purpose>(),
                false,
                null,
                null,
                null,
                null),
            new OrganisationBankAccountAdded(
                _organisationId,
                Guid.NewGuid(),
                "BG72UNCR70001522734456",
                true,
                "",
                false,
                new DateTime(2022,01,01),
                new DateTime(2022,01,01)),
            new OrganisationBankAccountAdded(
                _organisationId,
                _organisationBankAccountId,
                "BG72UNCR70001522734456",
                true,
                "",
                false,
                new DateTime(2023,01,01),
                new DateTime(2023,01,01)),
        };

    protected UpdateOrganisationBankAccount UpdateOrganisationBankAccount
        => new(
            _organisationBankAccountId,
            new OrganisationId(_organisationId),
            "BG72UNCR70001522734456",
            true,
            "KREDBEBB",
            true,
            new ValidFrom(),
            new ValidTo());

    [Fact]
    public async Task ThrowsException()
    {
        await Given(Events).When(UpdateOrganisationBankAccount, TestUser.AlgemeenBeheerder).ThenThrows<BankAccountNumberAlreadyCoupledToInThisPeriod>();
    }
}
