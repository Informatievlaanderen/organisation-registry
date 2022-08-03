namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationContact;

using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using ContactType;
using ContactType.Events;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using Xunit;
using Xunit.Abstractions;

public class WhenUpdatingAnOrganisationContact
    : Specification<UpdateOrganisationContactCommandHandler, UpdateOrganisationContact>
{
    private readonly Guid _organisationId;
    private readonly Guid _contactType1Id;
    private readonly string _contactType1Name;
    private readonly Guid _organisationContactId;
    private readonly Guid _contactType2Id;
    private readonly string _contactType2Name;
    private readonly string _newContactValue;
    private readonly string _oldContactValue;
    private readonly string _contactType1Regex;
    private readonly string _contactType1Example;
    private readonly string _contactType2Regex;
    private readonly string _contactType2Example;

    public WhenUpdatingAnOrganisationContact(ITestOutputHelper helper) : base(helper)
    {
        var fixture = new Fixture();
        _organisationId = fixture.Create<Guid>();
        _contactType1Id = fixture.Create<Guid>();
        _contactType1Name = fixture.Create<string>();
        _contactType1Regex = ".*";
        _contactType1Example = fixture.Create<string>();
        _contactType2Id = fixture.Create<Guid>();
        _contactType2Name = fixture.Create<string>();
        _contactType2Regex = ".*";
        _contactType2Example = fixture.Create<string>();
        _organisationContactId = fixture.Create<Guid>();
        _newContactValue = fixture.Create<string>();
        _oldContactValue = fixture.Create<string>();
    }

    protected override UpdateOrganisationContactCommandHandler BuildHandler(ISession session)
        => new(Mock.Of<ILogger<UpdateOrganisationContactCommandHandler>>(), session);

    private UpdateOrganisationContact UpdateOrganisationContactCommand
        => new(
            _organisationContactId,
            new OrganisationId(_organisationId),
            new ContactTypeId(_contactType2Id),
            _newContactValue,
            new ValidFrom(),
            new ValidTo());

    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(OrganisationCreated, ContactType1Created, ContactType2Created, OrganisationContact1Added)
            .When(UpdateOrganisationContactCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(1);
    }

    [Fact]
    public async Task UpdatesAnOrganisationContact()
    {
        await Given(OrganisationCreated, ContactType1Created, ContactType2Created, OrganisationContact1Added)
            .When(UpdateOrganisationContactCommand, TestUser.AlgemeenBeheerder)
            .Then();
        var updatedEvent = PublishedEvents
            .First()
            .UnwrapBody<OrganisationContactUpdated>();

        updatedEvent.OrganisationId.Should().Be(_organisationId);
        updatedEvent.Value.Should().Be(_newContactValue);
        updatedEvent.PreviousValue.Should().Be(_oldContactValue);
        updatedEvent.ValidFrom.Should().BeNull();
        updatedEvent.ValidTo.Should().BeNull();
        updatedEvent.ContactTypeId.Should().Be(_contactType2Id);
        updatedEvent.ContactTypeName.Should().Be(_contactType2Name);
        updatedEvent.OrganisationContactId.Should().Be(_organisationContactId);
        updatedEvent.PreviouslyValidFrom.Should().BeNull();
        updatedEvent.PreviouslyValidTo.Should().BeNull();
        updatedEvent.PreviousContactTypeId.Should().Be(_contactType1Id);
        updatedEvent.PreviousContactTypeName.Should().Be(_contactType1Name);
    }

    private OrganisationCreated OrganisationCreated
        => new OrganisationCreatedBuilder().WithId(_organisationId);

    private ContactTypeCreated ContactType1Created
        => new(_contactType1Id, _contactType1Name, _contactType1Regex, _contactType1Example);

    private ContactTypeCreated ContactType2Created
        => new(_contactType2Id, _contactType2Name, _contactType2Regex, _contactType2Example);

    private OrganisationContactAdded OrganisationContact1Added
        => new(_organisationId, _organisationContactId, _contactType1Id, _contactType1Name, _oldContactValue, null, null);
}
