namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.Children.When_Creating_Child_Organisation;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Detail;
using Xunit;

/// <summary>
/// Rollen zonder <c>CanManageChildren</c> mogen geen dochterorganisatie
/// registreren en krijgen 403 terug.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_CanManageChildren
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_CanManageChildren(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Cjmbeheerder)]
    [InlineData(ApiFixture.Backoffice.Regelgevingbeheerder)]
    [InlineData(ApiFixture.Backoffice.Orgaanbeheerder)]
    [InlineData(ApiFixture.Backoffice.Orafinbeheerder)]
    public async Task Then_Returns_Forbidden(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

        var parentOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(parentOrganisationId, _apiFixture.Fixture.Create<string>());

        var response = await ApiFixture.Post(
            client,
            "/v1/organisations",
            new CreateOrganisationRequest
            {
                Id = _apiFixture.Fixture.Create<Guid>(),
                Name = _apiFixture.Fixture.Create<string>(),
                ParentOrganisationId = parentOrganisationId,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
