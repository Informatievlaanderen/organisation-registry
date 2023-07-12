namespace OrganisationRegistry.ElasticSearch.Tests.Scenario;

using System;
using Location.Events;
using Organisation.Events;
using Projections.Organisations;
using Specimen;

/// <summary>
/// Sets up a fixture which uses the same organisationId for all events
/// </summary>
public class OrganisationScenario : ScenarioBase<Organisation>
{
    public OrganisationScenario(Guid organisationId) :
        base(new ParameterNameArg<Guid>("organisationId", organisationId))
    {
        var functionTypeId = Guid.NewGuid();
        AddCustomization(new ParameterNameArg<Guid>("functionId", functionTypeId));
        AddCustomization(new ParameterNameArg<Guid>("functionTypeId", functionTypeId));
    }

    public OrganisationRegulationUpdated CreateOrganisationRegulationUpdated(OrganisationRegulationAdded organisationRegulationAdded)
        => new(
            organisationId: organisationRegulationAdded.OrganisationId,
            organisationRegulationAdded.OrganisationRegulationId,
            Create<Guid>(),
            Create<string>(),
            Create<Guid>(),
            Create<string>(),
            Create<string>(),
            Create<string>(),
            Create<string>(),
            Create<DateTime?>(),
            Create<string>(),
            Create<string>(),
            Create<DateTime?>(),
            Create<DateTime?>(),
            Create<Guid>(),
            Create<string>(),
            Create<Guid>(),
            Create<string>(),
            Create<string>(),
            Create<DateTime?>(),
            Create<DateTime?>(),
            Create<string>(),
            Create<string>(),
            Create<DateTime?>(),
            Create<string>(),
            Create<string>());

    public OrganisationLocationUpdated CreateOrganisationLocationUpdated(OrganisationLocationAdded added)
        => new(
            organisationId: added.OrganisationId,
            organisationLocationId: added.OrganisationLocationId,
            locationId: Create<Guid>(),
            locationFormattedAddress: Create<string>(),
            isMainLocation: Create<bool>(),
            locationTypeId: Create<Guid>(),
            locationTypeName: Create<string>(),
            validFrom: Create<DateTime?>(),
            validTo: Create<DateTime?>(),
            previousLocationId: Create<Guid>(),
            previousLocationFormattedAddress: Create<string>(),
            previousIsMainLocation: Create<bool>(),
            previousLocationTypeId: Create<Guid>(),
            previousLocationTypeName: Create<string>(),
            previouslyValidFrom: Create<DateTime?>(),
            previouslyValidTo: Create<DateTime?>());

    public LocationUpdated CreateLocationUpdated(OrganisationLocationUpdated added)
        => new(
            locationId: added.LocationId,
            crabLocationId: Create<string>(),
            formattedAddress: Create<string>(),
            street: Create<string>(),
            zipCode: Create<string>(),
            city: Create<string>(),
            country: Create<string>(),
            previousCrabLocationId: Create<string>(),
            previousFormattedAddress: Create<string>(),
            previousStreet: Create<string>(),
            previousZipCode: Create<string>(),
            previousCity: Create<string>(),
            previousCountry: Create<string>());
}
