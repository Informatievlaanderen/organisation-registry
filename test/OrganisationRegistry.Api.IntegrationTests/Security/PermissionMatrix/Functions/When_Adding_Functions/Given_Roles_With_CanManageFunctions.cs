namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Functions.When_Adding_Functions;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Function;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_CanManageFunctions
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_CanManageFunctions(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Algemeenbeheerder_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());

        var response = await AddFunction(client, organisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOwnOrganisation_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var response = await AddFunction(client, _apiFixture.DecentraalbeheerderOrganisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithChildOrganisationInScope_Then_Returns_Created()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var response = await AddFunction(client, _apiFixture.DecentraalbeheerderChildOrganisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOrganisationOutsideScope_Then_Returns_Forbidden()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());

        var response = await AddFunction(client, organisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<HttpResponseMessage> AddFunction(HttpClient client, Guid organisationId)
    {
        var entityId = _apiFixture.Fixture.Create<Guid>();
        var functionTypeId = await _apiFixture.Create.Function();
        var personId = await _apiFixture.Create.Person();

        return await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/functions",
            new AddOrganisationFunctionRequest()
            {
                OrganisationFunctionId = entityId,
                FunctionId = functionTypeId,
                PersonId = personId,
                Contacts = null,
                ValidFrom = null,
                ValidTo = null,
            });
    }
}
