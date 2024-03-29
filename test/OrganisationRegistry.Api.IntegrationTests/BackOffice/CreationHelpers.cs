﻿namespace OrganisationRegistry.Api.IntegrationTests.BackOffice;

using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using AutoFixture;
using Backoffice.Body.Detail;
using Backoffice.Body.Seat;
using Backoffice.Organisation.OrganisationClassification;
using Backoffice.Parameters.BodyClassification.Requests;
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
using Person;

public class CreationHelpers
{
    private readonly ApiFixture _fixture;

    public CreationHelpers(ApiFixture fixture)
    {
        _fixture = fixture;
    }

    // Organisation:
    public async Task Organisation(Guid organisationId, string organisationName)
        => await ApiFixture.Post(_fixture.HttpClient, "/v1/organisations", new { id = organisationId, name = organisationName });

    public async Task<Guid> CreateOrganisationClassificationType(bool allowDifferentClassificationsToOverlap)
        => await Create<Guid>(
            "/v1/organisationclassificationtypes",
            new CreateOrganisationClassificationTypeRequest
            {
                Name = _fixture.Fixture.Create<string>(),
                AllowDifferentClassificationsToOverlap = allowDifferentClassificationsToOverlap,
            });

    public async Task<Guid> OrganisationClassification(Guid organisationClassificationTypeId)
        => await Create<Guid>(
            "/v1/organisationclassifications",
            new CreateOrganisationClassificationRequest
            {
                Name = _fixture.Fixture.Create<string>(),
                Order = _fixture.Fixture.Create<int>(),
                OrganisationClassificationTypeId = organisationClassificationTypeId,
                Active = true,
                ExternalKey = null,
            }
        );

    public async Task<Guid> OrganisationOrganisationClassification(Guid organisationId, Guid organisationClassificationTypeId, Guid organisationClassificationId)
    {
        var id = _fixture.Fixture.Create<Guid>();
        await ApiFixture.Post(
            _fixture.HttpClient,
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
                Name = _fixture.Fixture.Create<string>(),
                InverseName = _fixture.Fixture.Create<string>(),
            });

    // Body
    public async Task Body(Guid bodyId, string bodyName)
    {
        await DefaultLifecyclePhaseTypes();
        await ApiFixture.Post(
            _fixture.HttpClient,
            "/v1/bodies",
            new RegisterBodyRequest
            {
                Id = bodyId,
                Name = bodyName,
            });
    }

    public async Task<Guid> BodyClassificationType()
        => await Create<Guid>(
            "/v1/bodyclassificationtypes",
            new CreateBodyClassificationTypeRequest
            {
                Name = _fixture.Fixture.Create<string>(),
            });

    public async Task<Guid> BodyClassification(Guid bodyClassificationTypeId)
        => await Create<Guid>(
            "/v1/bodyclassifications",
            new CreateBodyClassificationRequest
            {
                Name = _fixture.Fixture.Create<string>(),
                Active = true,
                Order = _fixture.Fixture.Create<int>(),
                BodyClassificationTypeId = bodyClassificationTypeId,
            });

    public async Task<Guid> Location()
        => await Create<Guid>(
            "/v1/locations",
            new CreateLocationRequest
            {
                City = _fixture.Fixture.Create<string>(),
                Country = _fixture.Fixture.Create<string>(),
                Street = _fixture.Fixture.Create<string>(),
                ZipCode = _fixture.Fixture.Create<string>(),
            });

    private async Task DefaultLifecyclePhaseTypes()
    {
        await LifecyclePhaseType(isDefaultPhase: true, representsActivePhase: true);
        await LifecyclePhaseType(isDefaultPhase: true, representsActivePhase: false);
    }

    public async Task<Guid> LifecyclePhaseType(bool? isDefaultPhase = null, bool? representsActivePhase = null)
        => await Create<Guid>(
            "/v1/lifecyclephasetypes",
            new CreateLifecyclePhaseTypeRequest()
            {
                Name = _fixture.Fixture.Create<string>(),
                IsDefaultPhase = isDefaultPhase ?? false,
                RepresentsActivePhase = representsActivePhase ?? false,
            });

    // Common
    public async Task<Guid> KeyType()
        => await Create<Guid>(
            "/v1/keytypes",
            new CreateKeyTypeRequest
            {
                Name = _fixture.Fixture.Create<string>(),
            });

    public async Task<Guid> ContactType(string? contactTypeName = null)
        => await Create<Guid>(
            "/v1/contacttypes",
            new CreateContactTypeRequest
            {
                Name = contactTypeName ?? _fixture.Fixture.Create<string>(),
                Example = "test",
                Regex = ".*",
            });

    public async Task<Guid> FormalFramework(Guid formalFrameworkCategoryId)
        => await Create<Guid>(
            "/v1/formalframeworks",
            new CreateFormalFrameworkRequest
            {
                Name = _fixture.Fixture.Create<string>(),
                Code = _fixture.Fixture.Create<string>(),
                FormalFrameworkCategoryId = formalFrameworkCategoryId,
            });

    public async Task<Guid> FormalFrameworkCategory()
        => await Create<Guid>(
            "/v1/formalframeworkcategories",
            new CreateFormalFrameworkCategoryRequest()
            {
                Name = _fixture.Fixture.Create<string>(),
            });

    public async Task<Guid> Person()
        => await Create<Guid>(
            "/v1/people",
            new CreatePersonRequest
            {
                Name = _fixture.Fixture.Create<string>(),
                FirstName = _fixture.Fixture.Create<string>(),
                Sex = _fixture.Fixture.Create<bool>() ? Sex.Male : Sex.Female,
                DateOfBirth = _fixture.Fixture.Create<DateTime>(),
            });

    public async Task<Guid> Function()
        => await Create<Guid>(
            "/v1/functiontypes",
            new CreateFunctionTypeRequest
            {
                Name = _fixture.Fixture.Create<string>(),
            });

    public async Task<Guid> Capacity()
        => await Create<Guid>(
            "/v1/capacities",
            new CreateCapacityRequest
            {
                Name = _fixture.Fixture.Create<string>(),
            });

    public async Task<Guid> RegulationTheme()
        => await Create<Guid>(
            "/v1/regulationthemes",
            new CreateRegulationThemeRequest
            {
                Name = _fixture.Fixture.Create<string>(),
            });

    public async Task<Guid> RegulationSubTheme(Guid regulationThemeId)
        => await Create<Guid>(
            "/v1/regulationsubthemes",
            new CreateRegulationSubThemeRequest
            {
                Name = _fixture.Fixture.Create<string>(),
                RegulationThemeId = regulationThemeId,
            });

    public async Task<Guid> BodySeat(Guid bodyId, Guid seatTypeId)
    {
        var id = _fixture.Fixture.Create<Guid>();
        return await Create<Guid>(
            $"/v1/bodies/{bodyId}/seats",
            new AddBodySeatRequest
            {
                // cannot use _fixture.Create<> because no 'Id' property
                BodySeatId = id,
                Name = _fixture.Fixture.Create<string>(),
                PaidSeat = _fixture.Fixture.Create<bool>(),
                EntitledToVote = _fixture.Fixture.Create<bool>(),
                SeatTypeId = seatTypeId,
            });
    }

    private async Task<TId> Create<TId>(string route, dynamic body)
        where TId : notnull
    {
        if (body.GetType().GetProperty("Id") is not { })
            throw new InvalidDataContractException("Object to create should have an 'Id' property.");

        body.Id = _fixture.Fixture.Create<TId>();
        await ApiFixture.Post(_fixture.HttpClient, route, body);
        return body.Id;
    }
}
