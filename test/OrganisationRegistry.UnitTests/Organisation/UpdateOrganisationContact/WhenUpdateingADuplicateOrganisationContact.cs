namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationContact;

using System;
using System.Threading.Tasks;
using AutoFixture;
using ContactType;
using ContactType.Events;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using Xunit;
using Xunit.Abstractions;

public class WhenUpdateingADuplicateOrganisationContact
    : Specification<UpdateOrganisationContactCommandHandler, UpdateOrganisationContact>
{
    private readonly Guid _organisationId;
    private readonly Guid _contactType1Id;
    private readonly string _contactType1Name;
    private readonly Guid _organisationContact1Id;
    private readonly Guid _contactType2Id;
    private readonly string _contactType2Name;
    private readonly string _contactValue2;
    private readonly string _contactValue1;
    private readonly Guid _organisationContact2Id;

    public WhenUpdateingADuplicateOrganisationContact(ITestOutputHelper helper) : base(helper)
    {
        var fixture = new Fixture();
        _organisationId = fixture.Create<Guid>();
        _contactType1Id = fixture.Create<Guid>();
        _contactType1Name = fixture.Create<string>();
        _contactType2Id = fixture.Create<Guid>();
        _contactType2Name = fixture.Create<string>();
        _organisationContact1Id = fixture.Create<Guid>();
        _organisationContact2Id = fixture.Create<Guid>();
        _contactValue1 = fixture.Create<string>();
        _contactValue2 = fixture.Create<string>();
    }

    private UpdateOrganisationContact UpdateOrganisationContact1To2Command
        => new(
            _organisationContact1Id,
            new OrganisationId(_organisationId),
            new ContactTypeId(_contactType2Id),
            _contactValue2,
            new ValidFrom(),
            new ValidTo());

    protected override UpdateOrganisationContactCommandHandler BuildHandler(ISession session)
        => new(Mock.Of<ILogger<UpdateOrganisationContactCommandHandler>>(), session);

    [Fact]
    public async Task PublishesNoEvents()
    {
        await Given(
                OrganisationCreated,
                ContactType1Created,
                ContactType2Created,
                OrganisationContact1Added,
                OrganisationContact2Added)
            .When(UpdateOrganisationContact1To2Command, TestUser.User)
            .ThenItPublishesTheCorrectNumberOfEvents(0);
    }

    [Fact]
    public async Task ThrowsException()
    {
        await Given(
                OrganisationCreated,
                ContactType1Created,
                ContactType2Created,
                OrganisationContact1Added,
                OrganisationContact2Added)
            .When(UpdateOrganisationContact1To2Command, TestUser.User)
            .ThenThrows<CannotAddDuplicateContact>();
    }

    private OrganisationCreated OrganisationCreated
        => new OrganisationCreatedBuilder().WithId(_organisationId);

    private ContactTypeCreated ContactType1Created
        => new(_contactType1Id, _contactType1Name);

    private ContactTypeCreated ContactType2Created
        => new(_contactType2Id, _contactType2Name);

    private OrganisationContactAdded OrganisationContact1Added
        => new(_organisationId, _organisationContact1Id, _contactType1Id, _contactType1Name, _contactValue1, null, null);

    private OrganisationContactAdded OrganisationContact2Added
        => new(_organisationId, _organisationContact2Id, _contactType2Id, _contactType2Name, _contactValue2, null, null);
}
