namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationContact;

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

public class WhenAddingADuplicateOrganisationContact
    : Specification<AddOrganisationContactCommandHandler, AddOrganisationContact>
{
    private readonly Guid _organisationId;
    private readonly Guid _contactTypeId;
    private readonly Guid _organisationContactId;
    private readonly string _contactValue;
    private readonly Fixture _fixture;
    private string _contactTypeName;

    public WhenAddingADuplicateOrganisationContact(ITestOutputHelper helper) : base(helper)
    {
        _fixture = new Fixture();

        _contactTypeId = _fixture.Create<Guid>();
        _organisationContactId = _fixture.Create<Guid>();
        _organisationId = _fixture.Create<Guid>();
        _contactValue = _fixture.Create<string>();
        _contactTypeName = _fixture.Create<string>();
    }

    protected override AddOrganisationContactCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<AddOrganisationContactCommandHandler>>().Object,
            session
        );

    private AddOrganisationContact AddOrganisationContactCommand
        => new(
            _fixture.Create<Guid>(),
            new OrganisationId(_organisationId),
            new ContactTypeId(_contactTypeId),
            _contactValue,
            new ValidFrom(),
            new ValidTo());

    [Fact]
    public async Task PublishesNoEvents()
    {
        await Given(
                OrganisationCreated,
                ContactTypeCreated,
                OrganisationContactAdded)
            .When(AddOrganisationContactCommand, TestUser.User)
            .ThenItPublishesTheCorrectNumberOfEvents(0);
    }

    [Fact]
    public async Task ThrowsException()
    {
        await Given(
                OrganisationCreated,
                ContactTypeCreated,
                OrganisationContactAdded)
            .When(AddOrganisationContactCommand, TestUser.User)
            .ThenThrows<CannotAddDuplicateContact>();
    }

    private OrganisationCreated OrganisationCreated
        => new OrganisationCreatedBuilder().WithId(_organisationId);

    private ContactTypeCreated ContactTypeCreated
        => new(_contactTypeId, _contactTypeName);

    private OrganisationContactAdded OrganisationContactAdded
        => new(_organisationId,_organisationContactId,_contactTypeId, _contactTypeName,_contactValue, null, null);
}
