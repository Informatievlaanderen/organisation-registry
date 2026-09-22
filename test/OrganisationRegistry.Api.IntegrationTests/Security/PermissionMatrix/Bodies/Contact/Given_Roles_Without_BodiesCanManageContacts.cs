namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Bodies.Contact;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Body.Contact;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class Given_Roles_Without_BodiesCanManageContacts
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_Without_BodiesCanManageContacts(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Theory]
    [InlineData(ApiFixture.Backoffice.Regelgevingbeheerder)]
    public async Task Then_Returns_Forbidden(string role)
    {
        var client = await _apiFixture.CreateDynamicClient(role);
        var bodyId = _apiFixture.Fixture.Create<Guid>();

        var response = await ApiFixture.Post(
            client,
            $"/v1/bodies/{bodyId}/contacts",
            new AddBodyContactRequest
            {
                BodyContactId = _apiFixture.Fixture.Create<Guid>(),
                ContactTypeId = _apiFixture.Fixture.Create<Guid>(),
                ContactValue = _apiFixture.Fixture.Create<string>(),
                ValidFrom = null,
                ValidTo = null,
            });

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
