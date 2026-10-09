namespace OrganisationRegistry.Api.IntegrationTests.BackOffice;

using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using AutoFixture;
using Backoffice.Body.Detail;
using Backoffice.Body.Seat;
using Backoffice.Organisation.OrganisationClassification;
using Backoffice.Parameters.BodyClassification.Requests;
using Backoffice.Parameters.Building.Requests;
using Backoffice.Parameters.LabelType.Requests;
using Backoffice.Parameters.BodyClassificationType.Requests;
using Backoffice.Parameters.Capacity.Requests;
using Backoffice.Parameters.ContactType.Requests;
using Backoffice.Parameters.FormalFramework.Requests;
using Backoffice.Parameters.FormalFrameworkCategory.Requests;
using Backoffice.Parameters.FunctionType.Requests;
using Backoffice.Parameters.LifecyclePhaseType.Requests;
using Backoffice.Parameters.Location.Requests;
using Backoffice.Parameters.OrganisationClassification.Requests;
using Backoffice.Parameters.OrganisationClassificationType.Requests;
using Backoffice.Parameters.OrganisationRelationType.Requests;
using Backoffice.Parameters.RegulationSubTheme.Requests;
using Backoffice.Parameters.RegulationTheme.Requests;
using Backoffice.Person.Detail;
using OrganisationRegistry.Api.Backoffice.Parameters.KeyType.Requests;
using OrganisationRegistry.Api.Backoffice.Parameters.SeatType.Requests;
using Person;

public sealed class CreationHelpers(ApiFixture fixture)
{
    // Organisation:
    public async Task Organisation(Guid organisationId, string organisationName, string? ovoNumber = null, HttpClient? client = null)
    {
        using var _ = await ApiFixture.Post(client ?? fixture.HttpClient, "/v1/organisations", new { id = organisationId, name = organisationName, ovoNumber });
    }

    public async Task<Guid> CreateOrganisationClassificationType(bool allowDifferentClassificationsToOverlap)
        => await Create<Guid>(
            "/v1/organisationclassificationtypes",
            new CreateOrganisationClassificationTypeRequest
            {
                Name = fixture.Fixture.Create<string>(),
                AllowDifferentClassificationsToOverlap = allowDifferentClassificationsToOverlap,
            });

    public async Task<Guid> CreateOrganisationClassificationType(Guid organisationClassificationTypeId)
    {
        using var getResponse = await ApiFixture.Get(fixture.HttpClient, $"/v1/organisationclassificationtypes/{organisationClassificationTypeId}");
        if (getResponse.StatusCode == HttpStatusCode.OK)
            return organisationClassificationTypeId;

        using var postResponse = await ApiFixture.Post(
            fixture.HttpClient,
            "/v1/organisationclassificationtypes",
            new CreateOrganisationClassificationTypeRequest
            {
                Id = organisationClassificationTypeId,
                Name = fixture.Fixture.Create<string>(),
                AllowDifferentClassificationsToOverlap = false,
            });

        if (postResponse.StatusCode is not (HttpStatusCode.Created or HttpStatusCode.OK))
            throw new InvalidOperationException(
                $"Could not create organisationclassificationtype '{organisationClassificationTypeId}'. " +
                $"Status: {postResponse.StatusCode}. Body: {await postResponse.Content.ReadAsStringAsync()}");

        await WaitUntilCreated("/v1/organisationclassificationtypes", organisationClassificationTypeId);
        return organisationClassificationTypeId;
    }

    public async Task<Guid> OrganisationClassification(Guid organisationClassificationTypeId)
        => await Create<Guid>(
            "/v1/organisationclassifications",
            new CreateOrganisationClassificationRequest
            {
                Name = fixture.Fixture.Create<string>(),
                Order = fixture.Fixture.Create<int>(),
                OrganisationClassificationTypeId = organisationClassificationTypeId,
                Active = true,
                ExternalKey = null,
            }
        );

    public async Task<Guid> OrganisationOrganisationClassification(Guid organisationId, Guid organisationClassificationTypeId, Guid organisationClassificationId)
    {
        var id = fixture.Fixture.Create<Guid>();
        using var response = await ApiFixture.Post(
            fixture.HttpClient,
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

    public async Task<Guid> OrganisationRelationType()
        => await Create<Guid>(
            "/v1/organisationrelationtypes",
            new CreateOrganisationRelationTypeRequest
            {
                Name = fixture.Fixture.Create<string>(),
                InverseName = fixture.Fixture.Create<string>(),
            });

    // Body
    public async Task Body(Guid bodyId, string bodyName, HttpClient? client = null)
    {
        await DefaultLifecyclePhaseTypes();
        using var response = await ApiFixture.Post(
            client ?? fixture.HttpClient,
            "/v1/bodies",
            new RegisterBodyRequest
            {
                Id = bodyId,
                Name = bodyName,
            });
    }

    /// <summary>
    /// Registreert een orgaan voor een organisatie via de meegegeven client.
    /// Doordat het orgaan meteen aan de organisatie toegewezen wordt, komt het in de scope
    /// van de decentraalbeheerder van die organisatie terecht (na projectie).
    /// </summary>
    public async Task<Guid> BodyForOrganisation(Guid organisationId, HttpClient client, DateTime? validFrom = null)
    {
        await DefaultLifecyclePhaseTypes();

        var bodyId = fixture.Fixture.Create<Guid>();
        using var response = await ApiFixture.Post(
            client,
            "/v1/bodies",
            new RegisterBodyRequest
            {
                Id = bodyId,
                Name = fixture.Fixture.Create<string>(),
                OrganisationId = organisationId,
                ValidFrom = validFrom,
            });

        if (response.StatusCode is not HttpStatusCode.Created)
            throw new InvalidOperationException(
                $"Could not register body for organisation '{organisationId}'. " +
                $"Status: {response.StatusCode}. Body: {await response.Content.ReadAsStringAsync()}");

        return bodyId;
    }

    public async Task<Guid> SeatType()
        => await Create<Guid>(
            "/v1/seattypes",
            new CreateSeatTypeRequest
            {
                Name = fixture.Fixture.Create<string>(),
                Order = fixture.Fixture.Create<int>(),
                IsEffective = fixture.Fixture.Create<bool>(),
            });

    public async Task<Guid> BodyClassificationType()
        => await Create<Guid>(
            "/v1/bodyclassificationtypes",
            new CreateBodyClassificationTypeRequest
            {
                Name = fixture.Fixture.Create<string>(),
            });

    public async Task<Guid> BodyClassification(Guid bodyClassificationTypeId)
        => await Create<Guid>(
            "/v1/bodyclassifications",
            new CreateBodyClassificationRequest
            {
                Name = fixture.Fixture.Create<string>(),
                Active = true,
                Order = fixture.Fixture.Create<int>(),
                BodyClassificationTypeId = bodyClassificationTypeId,
            });

    public async Task<Guid> Location()
        => await Create<Guid>(
            "/v1/locations",
            new CreateLocationRequest
            {
                City = fixture.Fixture.Create<string>(),
                Country = fixture.Fixture.Create<string>(),
                Street = fixture.Fixture.Create<string>(),
                ZipCode = fixture.Fixture.Create<string>(),
            });

    public async Task DefaultLifecyclePhaseTypes()
    {
        await EnsureDefaultLifecyclePhaseType(representsActivePhase: true);
        await EnsureDefaultLifecyclePhaseType(representsActivePhase: false);
    }

    private async Task EnsureDefaultLifecyclePhaseType(bool representsActivePhase)
    {
        using var response = await ApiFixture.Post(
            fixture.HttpClient,
            "/v1/lifecyclephasetypes",
            new CreateLifecyclePhaseTypeRequest
            {
                Id = fixture.Fixture.Create<Guid>(),
                Name = fixture.Fixture.Create<string>(),
                IsDefaultPhase = true,
                RepresentsActivePhase = representsActivePhase,
            });

        if (response.StatusCode is HttpStatusCode.Created or HttpStatusCode.OK)
            return;

        var body = await response.Content.ReadAsStringAsync();
        if (response.StatusCode == HttpStatusCode.BadRequest &&
            body.Contains("Standaard levensloopfase is reeds gedefinieerd", StringComparison.OrdinalIgnoreCase))
            return;

        throw new InvalidOperationException(
            $"Could not ensure default lifecycle phase type. Status: {response.StatusCode}. Body: {body}");
    }

    public async Task<Guid> LifecyclePhaseType(bool? isDefaultPhase = null, bool? representsActivePhase = null)
        => await Create<Guid>(
            "/v1/lifecyclephasetypes",
            new CreateLifecyclePhaseTypeRequest()
            {
                Name = fixture.Fixture.Create<string>(),
                IsDefaultPhase = isDefaultPhase ?? false,
                RepresentsActivePhase = representsActivePhase ?? false,
            });

    // Common
    public async Task<Guid> KeyType()
        => await Create<Guid>(
            "/v1/keytypes",
            new CreateKeyTypeRequest
            {
                Name = fixture.Fixture.Create<string>(),
            });

    public async Task<Guid> KeyType(Guid keyTypeId)
    {
        using var getResponse = await ApiFixture.Get(fixture.HttpClient, $"/v1/keytypes/{keyTypeId}");
        if (getResponse.StatusCode == HttpStatusCode.OK)
            return keyTypeId;

        using var postResponse = await ApiFixture.Post(
            fixture.HttpClient,
            "/v1/keytypes",
            new CreateKeyTypeRequest
            {
                Id = keyTypeId,
                Name = fixture.Fixture.Create<string>(),
            });

        if (postResponse.StatusCode is not (HttpStatusCode.Created or HttpStatusCode.OK))
            throw new InvalidOperationException(
                $"Could not create keytype '{keyTypeId}'. " +
                $"Status: {postResponse.StatusCode}. Body: {await postResponse.Content.ReadAsStringAsync()}");

        await WaitUntilCreated("/v1/keytypes", keyTypeId);
        return keyTypeId;
    }

    public async Task<Guid> ContactType(string? contactTypeName = null)
        => await Create<Guid>(
            "/v1/contacttypes",
            new CreateContactTypeRequest
            {
                Name = contactTypeName ?? fixture.Fixture.Create<string>(),
                Example = "test",
                Regex = ".*",
            });

    public async Task<Guid> FormalFramework(Guid formalFrameworkCategoryId)
        => await FormalFramework(fixture.Fixture.Create<Guid>(), formalFrameworkCategoryId);

    public async Task<Guid> FormalFramework(Guid formalFrameworkId, Guid formalFrameworkCategoryId)
    {
        using var getResponse = await ApiFixture.Get(fixture.HttpClient, $"/v1/formalframeworks/{formalFrameworkId}");
        if (getResponse.StatusCode == HttpStatusCode.OK)
            return formalFrameworkId;

        using var postResponse = await ApiFixture.Post(
            fixture.HttpClient,
            "/v1/formalframeworks",
            new CreateFormalFrameworkRequest
            {
                Id = formalFrameworkId,
                Name = fixture.Fixture.Create<string>(),
                Code = fixture.Fixture.Create<string>(),
                FormalFrameworkCategoryId = formalFrameworkCategoryId,
            });

        if (postResponse.StatusCode is HttpStatusCode.Created or HttpStatusCode.OK)
            return formalFrameworkId;

        throw new InvalidOperationException(
            $"Could not ensure formal framework '{formalFrameworkId}'. " +
            $"Status: {postResponse.StatusCode}. Body: {await postResponse.Content.ReadAsStringAsync()}");
    }

    public async Task<Guid> FormalFrameworkCategory()
        => await Create<Guid>(
            "/v1/formalframeworkcategories",
            new CreateFormalFrameworkCategoryRequest()
            {
                Name = fixture.Fixture.Create<string>(),
            });

    public async Task<Guid> Person()
        => await Create<Guid>(
            "/v1/people",
            new CreatePersonRequest
            {
                Name = fixture.Fixture.Create<string>(),
                FirstName = fixture.Fixture.Create<string>(),
                Sex = fixture.Fixture.Create<bool>() ? Sex.Male : Sex.Female,
                DateOfBirth = fixture.Fixture.Create<DateTime>(),
            });

    public async Task<Guid> Function()
        => await Create<Guid>(
            "/v1/functiontypes",
            new CreateFunctionTypeRequest
            {
                Name = fixture.Fixture.Create<string>(),
            });

    public async Task<Guid> Capacity()
        => await Create<Guid>(
            "/v1/capacities",
            new CreateCapacityRequest
            {
                Name = fixture.Fixture.Create<string>(),
            });

    public async Task<Guid> Capacity(Guid capacityId)
    {
        using var getResponse = await ApiFixture.Get(fixture.HttpClient, $"/v1/capacities/{capacityId}");
        if (getResponse.StatusCode == HttpStatusCode.OK)
            return capacityId;

        using var postResponse = await ApiFixture.Post(
            fixture.HttpClient,
            "/v1/capacities",
            new CreateCapacityRequest
            {
                Id = capacityId,
                Name = fixture.Fixture.Create<string>(),
            });

        if (postResponse.StatusCode is not (HttpStatusCode.Created or HttpStatusCode.OK))
            throw new InvalidOperationException(
                $"Could not create capacity '{capacityId}'. " +
                $"Status: {postResponse.StatusCode}. Body: {await postResponse.Content.ReadAsStringAsync()}");

        await WaitUntilCreated("/v1/capacities", capacityId);
        return capacityId;
    }

    public async Task<Guid> Building()
        => await Create<Guid>(
            "/v1/buildings",
            new CreateBuildingRequest
            {
                Id = fixture.Fixture.Create<Guid>(),
                Name = fixture.Fixture.Create<string>(),
                VimId = null,
            });

    public async Task<Guid> LabelType()
        => await Create<Guid>(
            "/v1/labeltypes",
            new CreateLabelTypeRequest
            {
                Id = fixture.Fixture.Create<Guid>(),
                Name = fixture.Fixture.Create<string>(),
            });

    public async Task<Guid> RegulationTheme()
        => await Create<Guid>(
            "/v1/regulationthemes",
            new CreateRegulationThemeRequest
            {
                Name = fixture.Fixture.Create<string>(),
            });

    public async Task<Guid> RegulationSubTheme(Guid regulationThemeId)
        => await Create<Guid>(
            "/v1/regulationsubthemes",
            new CreateRegulationSubThemeRequest
            {
                Name = fixture.Fixture.Create<string>(),
                RegulationThemeId = regulationThemeId,
            });

    public async Task<Guid> BodySeat(Guid bodyId, Guid seatTypeId)
    {
        var id = fixture.Fixture.Create<Guid>();
        using var response = await ApiFixture.Post(
            fixture.HttpClient,
            $"/v1/bodies/{bodyId}/seats",
            new AddBodySeatRequest
            {
                // cannot use Create<> because the request uses 'BodySeatId', not 'Id'
                BodySeatId = id,
                Name = fixture.Fixture.Create<string>(),
                PaidSeat = fixture.Fixture.Create<bool>(),
                EntitledToVote = fixture.Fixture.Create<bool>(),
                SeatTypeId = seatTypeId,
            });

        if (response.StatusCode is not (HttpStatusCode.Created or HttpStatusCode.OK))
            throw new InvalidOperationException(
                $"Could not create test body seat at '/v1/bodies/{bodyId}/seats'. " +
                $"Status: {response.StatusCode}. Body: {await response.Content.ReadAsStringAsync()}");

        return id;
    }

    private async Task<TId> Create<TId>(string route, dynamic body)
        where TId : notnull
    {
        if (body.GetType().GetProperty("Id") is not { })
            throw new InvalidDataContractException("Object to create should have an 'Id' property.");

        body.Id = fixture.Fixture.Create<TId>();
        using var response = await ApiFixture.Post(fixture.HttpClient, route, body);
        if (response.StatusCode is not (HttpStatusCode.Created or HttpStatusCode.OK))
            throw new InvalidOperationException(
                $"Could not create test resource at '{route}'. " +
                $"Status: {response.StatusCode}. Body: {await response.Content.ReadAsStringAsync()}");

        await WaitUntilCreated(route, body.Id);
        return body.Id;
    }

    private async Task WaitUntilCreated(string route, object id)
    {
        var deadline = DateTime.UtcNow.AddSeconds(10);

        while (DateTime.UtcNow < deadline)
        {
            using var response = await ApiFixture.Get(fixture.HttpClient, $"{route}/{id}");
            if (response.StatusCode == HttpStatusCode.OK)
                return;

            await Task.Delay(250);
        }

        throw new InvalidOperationException($"Created test resource '{route}/{id}' did not become readable in time.");
    }
}
