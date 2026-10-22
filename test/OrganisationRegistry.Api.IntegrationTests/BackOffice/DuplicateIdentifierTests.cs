namespace OrganisationRegistry.Api.IntegrationTests.BackOffice;

using System;
using System.Net;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using OrganisationRegistry.Api.Backoffice.Body.Detail;
using OrganisationRegistry.Api.Backoffice.Organisation.Detail;
using OrganisationRegistry.Api.Backoffice.Organisation.Label;
using Xunit;

/// <summary>
/// Resending a request with a client-supplied identifier that already exists
/// is a client input error, not a server fault. These tests guard against a
/// regression where such a collision bubbled up as an unhandled exception
/// (<see cref="OrganisationRegistry.Infrastructure.Domain.Exception.ConcurrencyException"/>
/// for brand new aggregates, or a SQL unique/primary key violation for
/// sub-entities of an existing aggregate) and was reported to the integrator
/// as a generic 500 Internal Server Error instead of a 409 Conflict.
/// </summary>
[Collection(ApiTestsCollection.Name)]
public class DuplicateIdentifierTests
{
    private readonly ApiFixture _apiFixture;

    public DuplicateIdentifierTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;
    }

    [Fact]
    public async Task When_Creating_Organisation_With_AlreadyExisting_Id_Then_Returns_Conflict()
    {
        using var client = await _apiFixture.CreateAlgemeenbeheerderClient();
        var organisationId = _apiFixture.Fixture.Create<Guid>();
        var request = new CreateOrganisationRequest
        {
            Id = organisationId,
            Name = _apiFixture.Fixture.Create<string>(),
            ParentOrganisationId = null,
        };

        using var firstResponse = await ApiFixture.Post(client, "/v1/organisations", request);
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        using var secondResponse = await ApiFixture.Post(
            client,
            "/v1/organisations",
            new CreateOrganisationRequest
            {
                Id = organisationId,
                Name = _apiFixture.Fixture.Create<string>(),
                ParentOrganisationId = null,
            });

        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task When_Registering_Body_With_AlreadyExisting_Id_Then_Returns_Conflict()
    {
        using var client = await _apiFixture.CreateAlgemeenbeheerderClient();
        var bodyId = _apiFixture.Fixture.Create<Guid>();

        using var firstResponse = await ApiFixture.Post(
            client,
            "/v1/bodies",
            new RegisterBodyRequest
            {
                Id = bodyId,
                Name = _apiFixture.Fixture.Create<string>(),
            });
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        using var secondResponse = await ApiFixture.Post(
            client,
            "/v1/bodies",
            new RegisterBodyRequest
            {
                Id = bodyId,
                Name = _apiFixture.Fixture.Create<string>(),
            });

        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task When_Adding_Organisation_Label_With_AlreadyExisting_Id_Then_Returns_Conflict()
    {
        using var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        var organisationId = _apiFixture.Fixture.Create<Guid>();
        await _apiFixture.Create.Organisation(organisationId, _apiFixture.Fixture.Create<string>());

        var labelTypeId = await _apiFixture.Create.LabelType();
        var organisationLabelId = _apiFixture.Fixture.Create<Guid>();

        using var firstResponse = await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/labels",
            new AddOrganisationLabelRequest
            {
                OrganisationLabelId = organisationLabelId,
                LabelTypeId = labelTypeId,
                LabelValue = _apiFixture.Fixture.Create<string>(),
                ValidFrom = null,
                ValidTo = null,
            });
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var otherLabelTypeId = await _apiFixture.Create.LabelType();
        using var secondResponse = await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/labels",
            new AddOrganisationLabelRequest
            {
                // Different label type, but the same OrganisationLabelId:
                // the domain's overlap check only rejects a duplicate label
                // *per type*, so this collision instead surfaces at the SQL
                // projection layer (duplicate primary key) when it isn't
                // also guarded for by an identifier check.
                OrganisationLabelId = organisationLabelId,
                LabelTypeId = otherLabelTypeId,
                LabelValue = _apiFixture.Fixture.Create<string>(),
                ValidFrom = null,
                ValidTo = null,
            });

        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}
