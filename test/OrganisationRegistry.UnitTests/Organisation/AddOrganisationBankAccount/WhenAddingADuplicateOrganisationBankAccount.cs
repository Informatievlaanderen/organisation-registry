namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationBankAccount;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared;
using Tests.Shared.Stubs;
using Xunit;
using Xunit.Abstractions;

public class WhenAddingADuplicateOrganisationBankAccount
    : Specification<AddOrganisationBankAccountCommandHandler, AddOrganisationBankAccount>
{
    private readonly Guid _organisationId;
    private readonly Guid _organisationBankAccountId;
    private readonly DateTime _validTo;
    private readonly DateTime _validFrom;

    public WhenAddingADuplicateOrganisationBankAccount(ITestOutputHelper helper) : base(helper)
    {
        var dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);

        _organisationBankAccountId = Guid.NewGuid();
        _validFrom = dateTimeProviderStub.Today;
        _validTo = dateTimeProviderStub.Today.AddDays(2);
        _organisationId = Guid.NewGuid();
    }


    protected override AddOrganisationBankAccountCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<AddOrganisationBankAccountCommandHandler>>().Object,
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
                null,
                null),
        };

    protected AddOrganisationBankAccount AddOrganisationBankAccountCommand
        => new(
            _organisationBankAccountId,
            new OrganisationId(_organisationId),
            "BG72UNCR70001522734456",
            true,
            "KREDBEBB",
            true,
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    [Fact]
    public async Task ThrowsException()
    {
        await Given(Events)
            .When(
                AddOrganisationBankAccountCommand,
                TestUser.AlgemeenBeheerder)
            .ThenThrows<BankAccountNumberAlreadyCoupledToInThisPeriod>();
    }
}
