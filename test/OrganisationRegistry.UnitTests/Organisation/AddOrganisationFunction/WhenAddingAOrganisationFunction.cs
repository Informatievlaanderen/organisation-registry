namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationFunction;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using ContactType;
using ContactType.Events;
using FluentAssertions;
using Function;
using Function.Events;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Person;
using Person.Events;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using Xunit;
using Xunit.Abstractions;

public class WhenAddingAOrganisationFunction : Specification<AddOrganisationFunctionCommandHandler, AddOrganisationFunction>
{
    private readonly Fixture _fixture;
    private readonly Guid _organisationFunctionId;
    private readonly Guid _organisationId;
    private readonly Guid _functionTypeId;
    private readonly Guid _personId;
    private readonly Guid _contactTypeId;

    public WhenAddingAOrganisationFunction(ITestOutputHelper helper) : base(helper)
    {
        _fixture = new Fixture();
        _organisationFunctionId = _fixture.Create<Guid>();
        _organisationId = _fixture.Create<Guid>();
        _functionTypeId = _fixture.Create<Guid>();
        _personId = _fixture.Create<Guid>();
        _contactTypeId = _fixture.Create<Guid>();
    }

    protected override AddOrganisationFunctionCommandHandler BuildHandler(ISession session)
        => new(Mock.Of<ILogger<AddOrganisationFunctionCommandHandler>>(), session);

    private AddOrganisationFunction AddOrganisationFunctionCommand
        => new(
            _organisationFunctionId,
            new OrganisationId(_organisationId),
            new FunctionTypeId(_functionTypeId),
            new PersonId(_personId),
            new Dictionary<ContactTypeId, string>
            {
                { new ContactTypeId(_contactTypeId), _fixture.Create<string>() },
            },
            new ValidFrom(),
            new ValidTo());

    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(
                OrganisationCreated(),
                PersonCreated(),
                FunctionCreated(),
                ContactTypeCreated())
            .When(AddOrganisationFunctionCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(1);

        var organisationFunctionAdded = PublishedEvents[0].UnwrapBody<OrganisationFunctionAdded>();
        organisationFunctionAdded.OrganisationId.Should().Be(_organisationId);
        organisationFunctionAdded.PersonId.Should().Be(_personId);
        organisationFunctionAdded.FunctionId.Should().Be(_functionTypeId);
        organisationFunctionAdded.OrganisationFunctionId.Should().Be(_organisationFunctionId);
        organisationFunctionAdded.Contacts.Should().ContainKey(_contactTypeId);
    }

    private OrganisationCreated OrganisationCreated()
        => new OrganisationCreatedBuilder().WithId(_organisationId);

    private PersonCreated PersonCreated()
        => new(_personId, _fixture.Create<string>(), _fixture.Create<string>(), null, null);

    private FunctionCreated FunctionCreated()
        => new(_functionTypeId, _fixture.Create<string>());

    private ContactTypeCreated ContactTypeCreated()
        => new(_contactTypeId, _fixture.Create<string>(), ".*", _fixture.Create<string>());
}
