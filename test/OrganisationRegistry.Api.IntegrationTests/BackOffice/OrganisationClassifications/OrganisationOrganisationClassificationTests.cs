namespace OrganisationRegistry.Api.IntegrationTests.BackOffice.OrganisationClassifications;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using Backoffice.Organisation.OrganisationClassification;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class OrganisationOrganisationClassificationTests
{
    private readonly ApiFixture _apiFixture;

    public OrganisationOrganisationClassificationTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task TypeAllowsDuplicates_Ok()
    {
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        var organisationName = _apiFixture.Fixture.Create<string>();
        await _apiFixture.Create.Organisation(organisationId, organisationName);

        var organisationClassificationTypeId = await _apiFixture.Create.CreateOrganisationClassificationType(allowDifferentClassificationsToOverlap: true);
        var organisationClassification1Id = await _apiFixture.Create.OrganisationClassification(organisationClassificationTypeId);
        var organisationClassification2Id = await _apiFixture.Create.OrganisationClassification(organisationClassificationTypeId);

        var response1 = await AddOrganisationOrganisationClassification(organisationId, organisationClassificationTypeId, organisationClassification1Id);
        await ApiFixture.VerifyStatusCode(response1, HttpStatusCode.Created);

        var response2 = await AddOrganisationOrganisationClassification(organisationId, organisationClassificationTypeId, organisationClassification2Id);
        await ApiFixture.VerifyStatusCode(response2, HttpStatusCode.Created);
    }

    [Fact]
    public async Task TypeDoesNotAllowsDuplicates_Overlap_BadRequest()
    {
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        var organisationName = _apiFixture.Fixture.Create<string>();
        await _apiFixture.Create.Organisation(organisationId, organisationName);

        var organisationClassificationTypeId = await _apiFixture.Create.CreateOrganisationClassificationType(allowDifferentClassificationsToOverlap: false);
        var organisationClassification1Id = await _apiFixture.Create.OrganisationClassification(organisationClassificationTypeId);
        var organisationClassification2Id = await _apiFixture.Create.OrganisationClassification(organisationClassificationTypeId);

        var response1 = await AddOrganisationOrganisationClassification(organisationId, organisationClassificationTypeId, organisationClassification1Id);
        await ApiFixture.VerifyStatusCode(response1, HttpStatusCode.Created);

        var response2 = await AddOrganisationOrganisationClassification(organisationId, organisationClassificationTypeId, organisationClassification2Id);
        await ApiFixture.VerifyStatusCode(response2, HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task TypeDoesNotAllowsDuplicates_NoOverlap_Ok()
    {
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        var organisationName = _apiFixture.Fixture.Create<string>();
        await _apiFixture.Create.Organisation(organisationId, organisationName);

        var organisationClassificationTypeId = await _apiFixture.Create.CreateOrganisationClassificationType(allowDifferentClassificationsToOverlap: false);
        var organisationClassification1Id = await _apiFixture.Create.OrganisationClassification(organisationClassificationTypeId);
        var organisationClassification2Id = await _apiFixture.Create.OrganisationClassification(organisationClassificationTypeId);

        var response1 = await AddOrganisationOrganisationClassification(organisationId, organisationClassificationTypeId, organisationClassification1Id, validTo: DateTime.Today);
        await ApiFixture.VerifyStatusCode(response1, HttpStatusCode.Created);

        var response2 = await AddOrganisationOrganisationClassification(organisationId, organisationClassificationTypeId, organisationClassification2Id, validFrom: DateTime.Today.AddDays(1));
        await ApiFixture.VerifyStatusCode(response2, HttpStatusCode.Created);
    }

    private async Task<HttpResponseMessage> AddOrganisationOrganisationClassification(
        Guid organisationId,
        Guid organisationClassificationTypeId,
        Guid organisationClassificationId,
        DateTime? validFrom = null,
        DateTime? validTo = null)
        => await ApiFixture.Post(
            _apiFixture.HttpClient,
            $"/v1/organisations/{organisationId}/classifications",
            new AddOrganisationOrganisationClassificationRequest
            {
                OrganisationClassificationId = organisationClassificationId,
                OrganisationClassificationTypeId = organisationClassificationTypeId,
                OrganisationOrganisationClassificationId = _apiFixture.Fixture.Create<Guid>(),
                ValidFrom = validFrom,
                ValidTo = validTo,
            });
}
