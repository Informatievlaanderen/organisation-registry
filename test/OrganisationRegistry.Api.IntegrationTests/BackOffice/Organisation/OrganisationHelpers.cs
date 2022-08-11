namespace OrganisationRegistry.Api.IntegrationTests.BackOffice.Organisation;

using System;
using System.Threading.Tasks;
using AutoFixture;
using Backoffice.Organisation.OrganisationClassification;
using Backoffice.Parameters.Capacity.Requests;
using Backoffice.Parameters.ContactType.Requests;
using Backoffice.Parameters.FormalFramework.Requests;
using Backoffice.Parameters.FormalFrameworkCategory.Requests;
using Backoffice.Parameters.FunctionType.Requests;
using Backoffice.Parameters.KeyType.Requests;
using Backoffice.Parameters.OrganisationClassification.Requests;
using Backoffice.Parameters.OrganisationClassificationType.Requests;
using Backoffice.Parameters.OrganisationRelationType.Requests;
using Backoffice.Parameters.RegulationSubTheme.Requests;
using Backoffice.Parameters.RegulationTheme.Requests;
using Backoffice.Person.Detail;
using Person;

public class OrganisationHelpers
{
    private readonly ApiFixture _fixture;

    public OrganisationHelpers(ApiFixture fixture)
    {
        _fixture = fixture;
    }

    public async Task CreateOrganisation(Guid organisationId, string organisationName)
        => await ApiFixture.Post(_fixture.HttpClient, "/v1/organisations", new { id = organisationId, name = organisationName });
    
    public async Task<Guid> CreateOrganisationClassificationType(bool allowDifferentClassificationsToOverlap)
        => await _fixture.Create<Guid>(
            "/v1/organisationclassificationtypes",
            new CreateOrganisationClassificationTypeRequest
            {
                Name = _fixture.Fixture.Create<string>(),
                AllowDifferentClassificationsToOverlap = allowDifferentClassificationsToOverlap,
            });

    public async Task<Guid> CreateOrganisationClassification(Guid organisationClassificationTypeId)
        => await _fixture.Create<Guid>(
            "/v1/organisationclassifications",
            new CreateOrganisationClassificationRequest
            {
                Name = _fixture.Fixture.Create<string>(),
                Order = _fixture.Fixture.Create<int>(),
                OrganisationClassificationTypeId = organisationClassificationTypeId,
                Active = true,
                ExternalKey = null,
            }
        );

    public async Task<Guid> CreateOrganisationOrganisationClassification(Guid organisationId, Guid organisationClassificationTypeId, Guid organisationClassificationId)
    {
        var id = _fixture.Fixture.Create<Guid>();
        await ApiFixture.Post(
            _fixture.HttpClient,
            $"/v1/organisation/{organisationId}/classifications",
            new AddOrganisationOrganisationClassificationRequest
            {
                // cannot use _fixture.Create<> because no 'Id' property
                OrganisationOrganisationClassificationId = id,
                OrganisationClassificationTypeId = organisationClassificationTypeId,
                OrganisationClassificationId = organisationClassificationId,
                ValidFrom = null,
                ValidTo = null,
            });
        return id;
    }

    public async Task<Guid> CreateContactType(string? contactTypeName = null)
        => await _fixture.Create<Guid>(
            "/v1/contacttypes",
            new CreateContactTypeRequest
            {
                Name = contactTypeName ?? _fixture.Fixture.Create<string>(),
                Example = "test",
                Regex = ".*",
            });

    public async Task<Guid> CreateFormalFramework(Guid formalFrameworkCategoryId)
        => await _fixture.Create<Guid>(
            "/v1/formalframeworks",
            new CreateFormalFrameworkRequest
            {
                Name = _fixture.Fixture.Create<string>(),
                Code = _fixture.Fixture.Create<string>(),
                FormalFrameworkCategoryId = formalFrameworkCategoryId,
            });

    public async Task<Guid> CreateFormalFrameworkCategory()
        => await _fixture.Create<Guid>(
            "/v1/formalframeworkcategories",
            new CreateFormalFrameworkCategoryRequest()
            {
                Name = _fixture.Fixture.Create<string>(),
            });

    public async Task<Guid> CreatePerson()
        => await _fixture.Create<Guid>(
            "/v1/people",
            new CreatePersonRequest
            {
                Name = _fixture.Fixture.Create<string>(),
                FirstName = _fixture.Fixture.Create<string>(),
                Sex = _fixture.Fixture.Create<bool>() ? Sex.Male : Sex.Female,
                DateOfBirth = _fixture.Fixture.Create<DateTime>(),
            });

    public async Task<Guid> CreateFunction()
        => await _fixture.Create<Guid>(
            "/v1/functiontypes",
            new CreateFunctionTypeRequest
            {
                Name = _fixture.Fixture.Create<string>(),
            });

    public async Task<Guid> CreateCapacity()
        => await _fixture.Create<Guid>(
            "/v1/capacities",
            new CreateCapacityRequest
            {
                Name = _fixture.Fixture.Create<string>(),
            });

    public async Task<Guid> CreateKeyType()
        => await _fixture.Create<Guid>(
            "/v1/keytypes",
            new CreateKeyTypeRequest
            {
                Name = _fixture.Fixture.Create<string>(),
            });

    public async Task<Guid> CreateRegulationTheme()
        => await _fixture.Create<Guid>(
            "/v1/regulationthemes",
            new CreateRegulationThemeRequest
            {
                Name = _fixture.Fixture.Create<string>(),
            });

    public async Task<Guid> CreateOrganisationRelationType()
        => await _fixture.Create<Guid>(
            "/v1/organisationrelationtypes",
            new CreateOrganisationRelationTypeRequest
            {
                Name = _fixture.Fixture.Create<string>(),
                InverseName = _fixture.Fixture.Create<string>(),
            });

    public async Task<Guid> CreateRegulationSubTheme(Guid regulationThemeId)
        => await _fixture.Create<Guid>(
            "/v1/regulationsubthemes",
            new CreateRegulationSubThemeRequest
            {
                Name = _fixture.Fixture.Create<string>(),
                RegulationThemeId = regulationThemeId,
            });
}
