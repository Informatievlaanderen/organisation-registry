namespace OrganisationRegistry.Api.IntegrationTests.BackOffice.Parameters;

using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Backoffice.Body.Mandate;
using Backoffice.Body.Seat;
using Backoffice.Organisation.Capacity;
using Backoffice.Organisation.Function;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OrganisationRegistry.Body;
using SqlServer.Body;
using SqlServer.Configuration;
using SqlServer.Infrastructure;
using SqlServer.Organisation;
using Xunit;

[Collection(ApiTestsCollection.Name)]
public class FunctionTypeRemovedScheduledForRemovalTests
{
    private readonly ApiFixture _apiFixture;
    private readonly OrganisationRegistryContext _context;

    public FunctionTypeRemovedScheduledForRemovalTests(ApiFixture apiFixture)
    {
        _apiFixture = apiFixture;

        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.development.json", optional: true)
            .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetSection(SqlServerConfiguration.Section).Get<SqlServerConfiguration>()?.ConnectionString
                               ?? throw new InvalidOperationException("SqlServer:ConnectionString is missing.");

        var options = new DbContextOptionsBuilder<OrganisationRegistryContext>()
            .UseSqlServer(connectionString)
            .Options;

        _context = new OrganisationRegistryContext(options);
    }

    [Fact]
    public async Task Given_A_Removed_FunctionType_Related_Items_Are_Scheduled_For_Removal()
    {
        var organisationId = Guid.NewGuid();
        var personId = await _apiFixture.Create.Person();
        var functionTypeId = await _apiFixture.Create.Function();

        using var client = await _apiFixture.CreateAlgemeenbeheerderClient();

        await _apiFixture.Create.Organisation(organisationId, Guid.NewGuid().ToString("N"));

        var organisationFunctionId = Guid.NewGuid();
        var organisationFunctionResponse = await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/functions",
            new AddOrganisationFunctionRequest
            {
                OrganisationFunctionId = organisationFunctionId,
                FunctionId = functionTypeId,
                PersonId = personId,
                ValidFrom = null,
                ValidTo = null,
                Contacts = new Dictionary<Guid, string>(),
            });
        await ApiFixture.VerifyStatusCode(organisationFunctionResponse, HttpStatusCode.Created);

        var organisationCapacityId = Guid.NewGuid();
        var organisationCapacityResponse = await ApiFixture.Post(
            client,
            $"/v1/organisations/{organisationId}/capacities",
            new AddOrganisationCapacityRequest
            {
                OrganisationCapacityId = organisationCapacityId,
                CapacityId = await _apiFixture.Create.Capacity(),
                PersonId = personId,
                FunctionId = functionTypeId,
                LocationId = null,
                ValidFrom = null,
                ValidTo = null,
                Contacts = new Dictionary<Guid, string>(),
            });
        await ApiFixture.VerifyStatusCode(organisationCapacityResponse, HttpStatusCode.Created);

        await _apiFixture.Create.DefaultLifecyclePhaseTypes();
        var bodyId = Guid.NewGuid();
        var bodyResponse = await ApiFixture.Post(
            client,
            "/v1/bodies",
            new Backoffice.Body.Detail.RegisterBodyRequest
            {
                Id = bodyId,
                Name = Guid.NewGuid().ToString("N"),
            });
        await ApiFixture.VerifyStatusCode(bodyResponse, HttpStatusCode.Created);

        var seatTypeId = await _apiFixture.Create.SeatType();
        var bodySeatId = Guid.NewGuid();
        var bodySeatResponse = await ApiFixture.Post(
            client,
            $"/v1/bodies/{bodyId}/seats",
            new AddBodySeatRequest
            {
                BodySeatId = bodySeatId,
                Name = Guid.NewGuid().ToString("N"),
                PaidSeat = false,
                EntitledToVote = false,
                SeatTypeId = seatTypeId,
            });
        await ApiFixture.VerifyStatusCode(bodySeatResponse, HttpStatusCode.Created);

        var bodyMandateId = Guid.NewGuid();
        var bodyMandateResponse = await ApiFixture.Post(
            client,
            $"/v1/bodies/{bodyId}/mandates",
            new AddBodyMandateRequest
            {
                BodyMandateId = bodyMandateId,
                BodySeatId = bodySeatId,
                BodyMandateType = BodyMandateType.FunctionType,
                DelegatorId = organisationId,
                DelegatedId = functionTypeId,
                ValidFrom = null,
                ValidTo = null,
                Contacts = new Dictionary<Guid, string>(),
            });
        await ApiFixture.VerifyStatusCode(bodyMandateResponse, HttpStatusCode.Created);

        var deleteResponse = await ApiFixture.Delete(client, $"/v1/functiontypes/{functionTypeId}");
        await ApiFixture.VerifyStatusCode(deleteResponse, HttpStatusCode.NoContent);

        await WaitUntilAsync(
            async () =>
            {
                // fresh, untracked reads on every poll: Find() would serve the tracked
                // (stale) entities loaded by the previous poll.
                var organisationFunction = await _context.OrganisationFunctionList
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.OrganisationFunctionId == organisationFunctionId);
                var organisationCapacity = await _context.OrganisationCapacityList
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.OrganisationCapacityId == organisationCapacityId);
                var bodyMandate = await _context.BodyMandateList
                    .AsNoTracking()
                    .SingleOrDefaultAsync(x => x.BodyMandateId == bodyMandateId);

                return organisationFunction is { ScheduledForRemoval: true } &&
                       organisationCapacity is { ScheduledForRemoval: true } &&
                       bodyMandate is { ScheduledForRemoval: true };
            },
            TimeSpan.FromSeconds(30),
            "Niet alle gekoppelde items zijn gemarkeerd als ScheduledForRemoval.");
    }

    private static async Task WaitUntilAsync(Func<Task<bool>> predicate, TimeSpan timeout, string timeoutMessage)
    {
        var deadline = DateTime.UtcNow.Add(timeout);
        while (DateTime.UtcNow < deadline)
        {
            if (await predicate())
                return;

            await Task.Delay(TimeSpan.FromMilliseconds(500));
        }

        throw new InvalidOperationException(timeoutMessage);
    }
}
