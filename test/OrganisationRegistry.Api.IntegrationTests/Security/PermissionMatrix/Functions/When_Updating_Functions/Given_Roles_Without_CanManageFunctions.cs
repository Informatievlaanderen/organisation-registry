namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Functions.When_Updating_Functions;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Function;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_CanManageFunctions
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_CanManageFunctions(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Regelgevingbeheerder)]
    [InlineData(ApiFixture.Backoffice.Orgaanbeheerder)]
    public async Task Then_Returns_Forbidden(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        var entityId = _apiFixture.Fixture.Create<Guid>();
        var functionTypeId = await _apiFixture.Create.Function();
        var personId = await _apiFixture.Create.Person();

        var response = await ApiFixture.Put(
            client,
            $"/v1/organisations/{organisationId}/functions/{entityId}",
            new UpdateOrganisationFunctionRequest()
            {
                OrganisationFunctionId = entityId,
                FunctionId = functionTypeId,
                PersonId = personId,
                Contacts = null,
                ValidFrom = null,
                ValidTo = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
