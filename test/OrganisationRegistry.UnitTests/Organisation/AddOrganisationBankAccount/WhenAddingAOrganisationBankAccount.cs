namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationBankAccount;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class WhenAddingAOrganisationBankAccount
    : Specification<AddOrganisationBankAccountCommandHandler, AddOrganisationBankAccount>
{
    private readonly Guid _organisationId;
    private readonly Guid _organisationBankAccountId;
    private readonly DateTime _validTo;
    private readonly DateTime _validFrom;

    public WhenAddingAOrganisationBankAccount(ITestOutputHelper helper) : base(helper)
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
    public async Task PublishesOneEvent()
    {
        await Given(Events).When(AddOrganisationBankAccountCommand, TestUser.AlgemeenBeheerder).ThenItPublishesTheCorrectNumberOfEvents(1);
    }

    [Fact]
    public async Task AddsAnOrganisationBankAccount()
    {
        await Given(Events).When(AddOrganisationBankAccountCommand, TestUser.AlgemeenBeheerder).Then();

        PublishedEvents
            .First()
            .UnwrapBody<OrganisationBankAccountAdded>()
            .Should()
            .BeEquivalentTo(
                new OrganisationBankAccountAdded(
                    _organisationId,
                    _organisationBankAccountId,
                    "BG72UNCR70001522734456",
                    true,
                    "KREDBEBB",
                    true,
                    _validFrom,
                    _validTo),
                opt =>
                    opt.Excluding(e => e.Timestamp)
                        .Excluding(e => e.Version));
    }
}
