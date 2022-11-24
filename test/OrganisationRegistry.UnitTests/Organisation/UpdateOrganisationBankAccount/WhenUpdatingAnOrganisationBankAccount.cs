namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationBankAccount;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared;
using OrganisationRegistry.UnitTests.Infrastructure.Tests.Extensions.TestHelpers;
using Tests.Shared.Stubs;
using Xunit;
using Xunit.Abstractions;

public class WhenUpdatingAnOrganisationBankAccount
    : Specification<UpdateOrganisationBankAccountCommandHandler, UpdateOrganisationBankAccount>
{
    private readonly Guid _organisationId;
    private readonly Guid _organisationBankAccountId;
    private readonly DateTime _validTo;
    private readonly DateTime _validFrom;

    public WhenUpdatingAnOrganisationBankAccount(ITestOutputHelper helper) : base(helper)
    {
        var dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);

        _organisationBankAccountId = Guid.NewGuid();
        _validFrom = dateTimeProviderStub.Today;
        _validTo = dateTimeProviderStub.Today.AddDays(2);
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
                _organisationBankAccountId,
                "BG72UNCR70001522734456",
                true,
                "",
                false,
                null,
                null),
        };

    protected UpdateOrganisationBankAccount UpdateOrganisationBankAccount
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
        await Given(Events).When(UpdateOrganisationBankAccount, TestUser.AlgemeenBeheerder).ThenItPublishesTheCorrectNumberOfEvents(1);
    }

    [Fact]
    public async Task UpdatesAnOrganisationBankAccount()
    {
        await Given(Events).When(UpdateOrganisationBankAccount, TestUser.AlgemeenBeheerder).Then();

        PublishedEvents
            .First()
            .UnwrapBody<OrganisationBankAccountUpdated>()
            .Should()
            .BeEquivalentTo(
                new OrganisationBankAccountUpdated(
                    _organisationId,
                    _organisationBankAccountId,
                    "BG72UNCR70001522734456",
                    true,
                    "KREDBEBB",
                    true,
                    _validFrom,
                    _validTo,
                    "BG72UNCR70001522734456",
                    true,
                    "",
                    false,
                    null,
                    null),
                opt =>
                    opt.Excluding(e => e.Timestamp)
                        .Excluding(e => e.Version));
    }
}
