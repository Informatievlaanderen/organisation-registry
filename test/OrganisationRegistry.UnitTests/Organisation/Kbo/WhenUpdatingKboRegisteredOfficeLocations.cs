namespace OrganisationRegistry.UnitTests.Organisation.Kbo;

using System;
using AutoFixture;
using AutoFixture.Kernel;
using FluentAssertions;
using Location;
using Location.Commands;
using LocationType;
using OrganisationRegistry.Organisation;
using Tests.Shared.TestDataBuilders;
using Xunit;

public class WhenUpdatingKboRegisteredOfficeLocations
{
    [Fact]
    public void Given_the_MainLocationFlag_is_different_Then_it_is_not_overwritten()
    {
        var fixture = new Fixture();
        var dateTimeProviderStub = new DateTimeProviderStub(fixture.Create<DateTime>());

        var organisationRoot = new OrganisationBuilder(fixture, dateTimeProviderStub)
            .Build();

        var organisationLocationId = fixture.Create<Guid>();
        var locationId = fixture.Create<Guid>();
        var locationType = new LocationType(new LocationTypeId(fixture.Create<Guid>()), new LocationTypeName(fixture.Create<string>()));

        CreateKboLocation(organisationLocationId, organisationRoot, locationId, fixture, locationType, dateTimeProviderStub);

        SetKboLocationAsMainLocation(organisationLocationId, organisationRoot, locationId);

        SimulateSyncFromKbo(organisationRoot, fixture);

        VerifyKboLocationIsStillMainLocation(organisationRoot);
    }

    private static void CreateKboLocation(Guid organisationLocationId, Organisation organisationRoot, Guid locationId, ISpecimenBuilder fixture, LocationType locationType, IDateTimeProvider dateTimeProviderStub)
    {
        var newKboRegisteredOffice = CreateKboRegisteredOffice(organisationLocationId, organisationRoot, locationId, false);
        var location = new Location(new LocationId(locationId), fixture.Create<string>(), fixture.Create<Address>());

        organisationRoot.AddKboRegisteredOfficeLocation(newKboRegisteredOffice.OrganisationLocationId, location, locationType, new Period(), dateTimeProviderStub);
    }

    private static void SetKboLocationAsMainLocation(Guid organisationLocationId, Organisation organisationRoot, Guid locationId)
    {
        var updatedKboRegisteredOffice = CreateKboRegisteredOffice(organisationLocationId, organisationRoot, locationId, true);
        organisationRoot.UpdateKboRegisteredOfficeLocationIsMainLocation(updatedKboRegisteredOffice);
    }

    private static void SimulateSyncFromKbo(Organisation organisationRoot, ISpecimenBuilder fixture)
    {
        var newLocationId = fixture.Create<Guid>();
        var location = new Location(new LocationId(newLocationId), fixture.Create<string>(), fixture.Create<Address>());

        var kboRegisteredOffice = new KboRegisteredOffice(location, null, null);
        organisationRoot.UpdateKboRegisteredOfficeLocations(kboRegisteredOffice, fixture.Create<LocationType>());
    }

    private static OrganisationLocation CreateKboRegisteredOffice(Guid organisationLocationId, Organisation organisationRoot, Guid locationId, bool isMainLocation)
        => new(organisationLocationId, organisationRoot.Id, locationId, "", isMainLocation, null, "", new Period(), Source.Kbo);

    private static void VerifyKboLocationIsStillMainLocation(Organisation organisationRoot)
        => organisationRoot.KboState.KboRegisteredOffice?.IsMainLocation.Should().BeTrue();
}
