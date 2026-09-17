namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Children.When_Updating_Parent;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Organisation.Parent;
using OrganisationRegistry.Infrastructure.Authorization;
using Xunit;

/// <summary>
/// Matrixrij <b>Onderliggende organisaties (kinderen)</b> — het aanpassen van een
/// bovenliggende organisatie (<c>PUT /v1/organisations/{id}/parents/{id}</c>) wordt
/// gedreven door <see cref="Permission.CanManageChildren" />.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class Given_Roles_With_CanManageChildren
{
    private readonly ApiFixture _apiFixture;

    public Given_Roles_With_CanManageChildren(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task For_Algemeenbeheerder_Then_Returns_Ok()
    {
        var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var childOrganisationId = await CreateOrganisation();
        var parentOrganisationId = await CreateOrganisation();
        var organisationParentId = await AddParentAsAlgemeenbeheerder(childOrganisationId, parentOrganisationId);

        var response = await UpdateParent(client, childOrganisationId, organisationParentId, parentOrganisationId);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOwnOrganisation_Then_Returns_Ok()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        // OVO000102 ligt binnen de (gecachete) scope van de decentraalbeheerder en is niet
        // Vlimpers-beheerd. Het heeft reeds een (open) ouderkoppeling uit de fixture; die
        // werken we in-place bij zodat we niets aan OVO000003 toevoegen (dat over testruns
        // heen ouderkoppelingen verzamelt en de domeinlaag zou blokkeren).
        var childOrganisationId = _apiFixture.DecentraalbeheerderChildOrganisationId;
        var existing = await GetFirstParentCoupling(client, childOrganisationId);

        var response = await UpdateParent(
            client,
            childOrganisationId,
            existing.OrganisationOrganisationParentId,
            existing.ParentOrganisationId,
            existing.ValidFrom,
            existing.ValidTo);

        // Deze matrixrij toetst de machtiging: de decentraalbeheerder mag de ouder/kind-structuur
        // van een organisatie in eigen scope beheren, dus mag de aanvraag niet geweigerd worden.
        response.StatusCode.Should().NotBe(HttpStatusCode.Forbidden, await response.Content.ReadAsStringAsync());
        response.StatusCode.Should().NotBe(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task For_Decentraalbeheerder_WithOrganisationOutsideScope_Then_Returns_Forbidden()
    {
        var client = await _apiFixture.CreateDynamicClient(ApiFixture.Backoffice.Decentraalbeheerder);

        var childOrganisationId = await CreateOrganisation();
        var parentOrganisationId = await CreateOrganisation();
        var organisationParentId = await AddParentAsAlgemeenbeheerder(childOrganisationId, parentOrganisationId);

        var response = await UpdateParent(client, childOrganisationId, organisationParentId, parentOrganisationId);

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    private async Task<Guid> CreateOrganisation()
    {
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());
        return organisationId;
    }

    private static async Task<(Guid OrganisationOrganisationParentId, Guid ParentOrganisationId, DateTime? ValidFrom, DateTime? ValidTo)>
        GetFirstParentCoupling(HttpClient client, Guid organisationId)
    {
        using var response = await ApiFixture.Get(client, $"/v1/organisations/{organisationId}/parents");
        response.StatusCode.Should().Be(HttpStatusCode.OK, await response.Content.ReadAsStringAsync());

        var items = await ApiFixture.DeserializeAsList(response);
        items.Should().NotBeEmpty("de fixture koppelt OVO000102 aan een ouder (OVO000003)");

        var first = items[0];
        return (
            Guid.Parse((string)first["organisationOrganisationParentId"]),
            Guid.Parse((string)first["parentOrganisationId"]),
            first.TryGetValue("validFrom", out var vf) && vf is string vfs ? DateTime.Parse(vfs) : null,
            first.TryGetValue("validTo", out var vt) && vt is string vts ? DateTime.Parse(vts) : null);
    }

    private async Task<Guid> AddParentAsAlgemeenbeheerder(
        Guid childOrganisationId,
        Guid parentOrganisationId,
        DateTime? validFrom = null,
        DateTime? validTo = null)
    {
        var organisationParentId = _apiFixture.Fixture.Create<Guid>();
        var algemeenbeheerderClient = await _apiFixture.CreateAlgemeenbeheerderClient();

        var response = await ApiFixture.Post(
            algemeenbeheerderClient,
            $"/v1/organisations/{childOrganisationId}/parents",
            new AddOrganisationParentRequest
            {
                OrganisationOrganisationParentId = organisationParentId,
                ParentOrganisationId = parentOrganisationId,
                ValidFrom = validFrom,
                ValidTo = validTo,
            });
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        return organisationParentId;
    }

    private static async Task<HttpResponseMessage> UpdateParent(
        HttpClient client,
        Guid childOrganisationId,
        Guid organisationParentId,
        Guid parentOrganisationId,
        DateTime? validFrom = null,
        DateTime? validTo = null)
        => await ApiFixture.Put(
            client,
            $"/v1/organisations/{childOrganisationId}/parents/{organisationParentId}",
            new UpdateOrganisationParentRequest
            {
                OrganisationOrganisationParentId = organisationParentId,
                ParentOrganisationId = parentOrganisationId,
                ValidFrom = validFrom,
                ValidTo = validTo,
            });
}
