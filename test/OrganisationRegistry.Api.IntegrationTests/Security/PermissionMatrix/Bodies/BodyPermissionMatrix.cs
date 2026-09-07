namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Bodies;

using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using AutoFixture;
using OrganisationRegistry.Api.Backoffice.Body.Detail;

/// <summary>
/// Gedeelde hulpmethodes voor de orgaan-permissiematrix.
///
/// De scope van een decentraalbeheerder op een orgaan wordt afgeleid uit de
/// ActiveBodyOrganisation-projectie. Die wordt asynchroon bijgewerkt nadat een
/// orgaan aan een organisatie toegewezen werd, dus positieve decentraalbeheerder-
/// gevallen pollen tot de bewerking niet langer Forbidden teruggeeft.
/// </summary>
internal static class BodyPermissionMatrix
{
    /// <summary>
    /// Registreert (via een bevoorrechte algemeenbeheerder-client) een orgaan dat aan
    /// <paramref name="organisationId"/> toegewezen is, zodat het in de scope van de
    /// decentraalbeheerder van die organisatie(-tak) terechtkomt.
    /// </summary>
    public static async Task<Guid> CreateBodyForOrganisation(ApiFixture fixture, Guid organisationId, DateTime? validFrom = null)
    {
        var privilegedClient = await fixture.CreateAlgemeenbeheerderClient();
        return await fixture.Create.BodyForOrganisation(organisationId, privilegedClient, validFrom);
    }

    /// <summary>
    /// Registreert (via een bevoorrechte algemeenbeheerder-client) een orgaan zonder
    /// organisatie. Zo'n orgaan valt buiten de scope van elke decentraalbeheerder.
    /// </summary>
    public static async Task<Guid> CreateBareBody(ApiFixture fixture, DateTime? validFrom = null)
    {
        var privilegedClient = await fixture.CreateAlgemeenbeheerderClient();
        await fixture.Create.DefaultLifecyclePhaseTypes();

        var bodyId = fixture.Fixture.Create<Guid>();
        using var response = await ApiFixture.Post(
            privilegedClient,
            "/v1/bodies",
            new RegisterBodyRequest
            {
                Id = bodyId,
                Name = fixture.Fixture.Create<string>(),
                ValidFrom = validFrom,
            });

        if (response.StatusCode is not HttpStatusCode.Created)
            throw new InvalidOperationException(
                $"Could not register bare body. Status: {response.StatusCode}. " +
                $"Body: {await response.Content.ReadAsStringAsync()}");

        return bodyId;
    }

    /// <summary>
    /// Voert een (herhaalbare) bewerking uit tot ze niet langer Forbidden is, zodat de
    /// test niet flaky wordt door de vertraging op de ActiveBodyOrganisation-projectie.
    /// </summary>
    public static async Task<HttpResponseMessage> WaitUntilAllowed(Guid bodyId, Func<Task<HttpResponseMessage>> action)
    {
        HttpResponseMessage? response = null;
        await ApiFixture.WaitUntil(
            async () =>
            {
                response = await action();
                return response.StatusCode != HttpStatusCode.Forbidden;
            },
            $"Het orgaan '{bodyId}' is niet tijdig bewerkbaar geworden voor de decentraalbeheerder. " +
            "Controleer of de ActiveBodyOrganisation-projectie afgewerkt is.");

        return response!;
    }
}
