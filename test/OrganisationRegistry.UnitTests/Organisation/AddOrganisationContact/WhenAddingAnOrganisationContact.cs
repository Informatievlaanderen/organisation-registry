namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationContact;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContactType;
using ContactType.Events;
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

public class WhenAddingAnOrganisationContact
    : Specification<AddOrganisationContactCommandHandler, AddOrganisationContact>
{
    private readonly Guid _organisationId;
    private readonly Guid _contactTypeId;
    private readonly string _contactTypeRegex;
    private readonly string _contactTypeExample;
    private readonly Guid _organisationContactId;
    private readonly DateTime _validTo;
    private readonly DateTime _validFrom;
    private readonly string _contactValue;

    public WhenAddingAnOrganisationContact(ITestOutputHelper helper) : base(helper)
    {
        var dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);

        _contactTypeId = Guid.NewGuid();
        _organisationContactId = Guid.NewGuid();
        _validFrom = dateTimeProviderStub.Today;
        _validTo = dateTimeProviderStub.Today.AddDays(2);
        _organisationId = Guid.NewGuid();
        _contactValue = "info@email.com";
        _contactTypeRegex = ".*";
        _contactTypeExample = "example@example.be";
    }

    protected override AddOrganisationContactCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<AddOrganisationContactCommandHandler>>().Object,
            session
        );

    private IEvent[] Events
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
            new ContactTypeCreated(_contactTypeId, "Contact type A", _contactTypeRegex, _contactTypeExample),
        };

    private AddOrganisationContact AddOrganisationContactCommand
        => new(
            _organisationContactId,
            new OrganisationId(_organisationId),
            new ContactTypeId(_contactTypeId),
            _contactValue,
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(Events)
            .When(AddOrganisationContactCommand, TestUser.User)
            .ThenItPublishesTheCorrectNumberOfEvents(1);
    }

    [Fact]
    public async Task AddsAnOrganisationContact()
    {
        await Given(Events)
            .When(AddOrganisationContactCommand, TestUser.User)
            .Then();
        PublishedEvents
            .First()
            .UnwrapBody<OrganisationContactAdded>()
            .Should()
            .BeEquivalentTo(
                new OrganisationContactAdded(
                    _organisationId,
                    _organisationContactId,
                    _contactTypeId,
                    "Contact type A",
                    _contactValue,
                    _validFrom,
                    _validTo
                ),
                opt => opt.ExcludeEventProperties());
    }
}
