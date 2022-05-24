namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationParent;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class WhenAddingAVlimpersOrganisationParentAndUserIsVlimpers
    : Specification<AddOrganisationParentCommandHandler, AddOrganisationParent>
{
    private readonly Guid _organisationId;
    private readonly Guid _organisationOrganisationParentId;
    private readonly DateTime _validTo;
    private readonly DateTime _validFrom;
    private readonly DateTimeProviderStub _dateTimeProviderStub;
    private readonly Guid _organisationParentId;
    private readonly string _organisationParentName;

    public WhenAddingAVlimpersOrganisationParentAndUserIsVlimpers(ITestOutputHelper helper) : base(helper)
    {
        _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);
        _organisationOrganisationParentId = Guid.NewGuid();
        _validFrom = _dateTimeProviderStub.Today;
        _validTo = _dateTimeProviderStub.Today.AddDays(2);
        _organisationId = Guid.NewGuid();
        _organisationParentId = Guid.NewGuid();
        _organisationParentName = "Parent organisation";
    }

    protected override AddOrganisationParentCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<AddOrganisationParentCommandHandler>>().Object,
            session,
            _dateTimeProviderStub);

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
            new OrganisationPlacedUnderVlimpersManagement(_organisationId),
            new OrganisationCreated(
                _organisationParentId,
                _organisationParentName,
                "OVO000012346",
                "O&G",
                Article.None,
                _organisationParentName,
                new List<Purpose>(),
                false,
                null,
                null,
                null,
                null),
            new OrganisationPlacedUnderVlimpersManagement(_organisationParentId),
        };

    private AddOrganisationParent AddOrganisationParentCommand
        => new(
            _organisationOrganisationParentId,
            new OrganisationId(_organisationId),
            new OrganisationId(_organisationParentId),
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    [Fact]
    public async Task PublishesTwoEvents()
    {
        await Given(Events).When(AddOrganisationParentCommand, UserBuilder.VlimpersBeheerder())
            .ThenItPublishesTheCorrectNumberOfEvents(2);
    }

    [Fact]
    public async Task AddsAnOrganisationParent()
    {
        await Given(Events).When(AddOrganisationParentCommand, UserBuilder.VlimpersBeheerder()).Then();

        PublishedEvents[0]
            .UnwrapBody<OrganisationParentAdded>()
            .Should()
            .BeEquivalentTo(
                new OrganisationParentAdded(
                    _organisationId,
                    _organisationOrganisationParentId,
                    _organisationParentId,
                    _organisationParentName,
                    _validFrom,
                    _validTo),
                opt => opt.ExcludeEventProperties());
    }

    [Fact]
    public async Task AssignsAParent()
    {
        await Given(Events).When(AddOrganisationParentCommand, UserBuilder.VlimpersBeheerder()).Then();

        PublishedEvents[1]
            .UnwrapBody<ParentAssignedToOrganisation>()
            .Should()
            .BeEquivalentTo(
                new ParentAssignedToOrganisation(
                    _organisationId,
                    _organisationParentId,
                    _organisationOrganisationParentId),
                opt => opt.ExcludeEventProperties());
    }
}
