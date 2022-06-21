namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationLocation;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Location;
using OrganisationRegistry.Infrastructure.Events;
using Location.Events;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared;
using Tests.Shared.Stubs;
using Xunit;
using Xunit.Abstractions;

public class
    WhenAddingAMainOrganisationLocation : Specification<AddOrganisationLocationCommandHandler,
        AddOrganisationLocation>
{
    private readonly Guid _organisationId;
    private readonly Guid _locationId;
    private readonly Guid _organisationLocationId;
    private readonly bool _isMainLocation;
    private readonly DateTime _validTo;
    private readonly DateTime _validFrom;
    private readonly DateTimeProviderStub _dateTimeProviderStub;
    private readonly string _ovoNumber;
    private readonly string _formatedAdress;

    public WhenAddingAMainOrganisationLocation(ITestOutputHelper helper) : base(helper)
    {
        _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);

        _locationId = Guid.NewGuid();
        _organisationLocationId = Guid.NewGuid();
        _isMainLocation = true;
        _validFrom = _dateTimeProviderStub.Today;
        _validTo = _dateTimeProviderStub.Today.AddDays(2);
        _organisationId = Guid.NewGuid();
        _ovoNumber = "OVO000012345";
        _formatedAdress = "Albert 1 laan 32, 1000 Brussel";
    }

    protected override AddOrganisationLocationCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<AddOrganisationLocationCommandHandler>>().Object,
            session,
            _dateTimeProviderStub,
            new OrganisationRegistryConfigurationStub());

    private IUser User
        => new UserBuilder().AddRoles(Role.DecentraalBeheerder).AddOrganisations(_ovoNumber).Build();

    private IEvent[] Events
        => new IEvent[]
        {
            new OrganisationCreated(
                _organisationId,
                "Kind en Gezin",
                _ovoNumber,
                "K&G",
                Article.None,
                "Kindjes en gezinnetjes",
                new List<Purpose>(),
                false,
                null,
                null,
                null,
                null),
            new LocationCreated(
                _locationId,
                "12345",
                "Albert 1 laan 32, 1000 Brussel",
                "Albert 1 laan 32",
                "1000",
                "Brussel",
                "Belgie"),
        };

    private AddOrganisationLocation AddOrganisationLocationCommand
        => new(
            _organisationLocationId,
            new OrganisationId(_organisationId),
            new LocationId(_locationId),
            _isMainLocation,
            null,
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    [Fact]
    public async Task PublishesTwoEvents()
    {
        await Given(Events)
            .When(AddOrganisationLocationCommand, User)
            .ThenItPublishesTheCorrectNumberOfEvents(2);
    }

    [Fact]
    public async Task AddsAnOrganisationLocation()
    {
        await Given(Events).When(AddOrganisationLocationCommand, User).Then();

        PublishedEvents[0]
            .UnwrapBody<OrganisationLocationAdded>()
            .Should()
            .BeEquivalentTo(
                new OrganisationLocationAdded(
                    _organisationId,
                    _organisationLocationId,
                    _locationId,
                    _formatedAdress,
                    _isMainLocation,
                    null,
                    string.Empty,
                    _validFrom,
                    _validTo),
                opt => opt.ExcludeEventProperties());
    }

    [Fact]
    public async Task AssignsAMainLocation()
    {
        await Given(Events).When(AddOrganisationLocationCommand, User).Then();

        PublishedEvents[1]
#pragma warning disable CS0618
            .UnwrapBody<MainLocationAssignedToOrganisation>()
#pragma warning restore CS0618
            .Should()
            .BeEquivalentTo(
#pragma warning disable CS0618
                new MainLocationAssignedToOrganisation(
#pragma warning restore CS0618
                    _organisationId,
                    _locationId,
                    _organisationLocationId),
                opt => opt.ExcludeEventProperties());

    }
}
