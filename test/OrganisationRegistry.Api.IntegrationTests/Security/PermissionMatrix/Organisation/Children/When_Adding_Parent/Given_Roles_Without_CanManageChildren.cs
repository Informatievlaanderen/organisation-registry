namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Organisation.Children.When_Adding_Parent;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Parent;
using Xunit;

/// <summary>
/// Rollen zonder <c>CanManageChildren</c> mogen de ouder/kind-structuur van een
/// organisatie niet beheren en krijgen 403 terug.
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
    public async Task Then_Returns_Forbidden(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);

        var childOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(childOrganisationId, _apiFixture.Fixture.Create<string>());
        var parentOrganisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(parentOrganisationId, _apiFixture.Fixture.Create<string>());

        var response = await ApiFixture.Post(
            client,
            $"/v1/organisations/{childOrganisationId}/parents",
            new AddOrganisationParentRequest
            {
                OrganisationOrganisationParentId = _apiFixture.Fixture.Create<Guid>(),
                ParentOrganisationId = parentOrganisationId,
                ValidFrom = null,
                ValidTo = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
