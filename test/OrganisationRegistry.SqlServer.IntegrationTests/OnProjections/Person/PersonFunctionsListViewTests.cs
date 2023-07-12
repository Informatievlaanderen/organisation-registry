namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.Person;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using OrganisationRegistry.Organisation.Events;
using TestBases;
using Xunit;

[Collection(SqlServerTestsCollection.Name)]
public class PersonFunctionsListViewTests : ListViewTestBase
{
    [Fact]
    public async Task OrganisationTeminated()
    {
        var personId = Guid.NewGuid();
        var organisationId = Guid.NewGuid();
        var organisationFunctionId = Guid.NewGuid();
        var dateOfTermination = DateTime.Now.AddYears(1);
        await HandleEvents(
            new OrganisationCreated(
                organisationId,
                Guid.NewGuid()
                    .ToString(),
                Guid.NewGuid()
                    .ToString(),
                null,
                null,
                null,
                new List<Purpose>()
                {
                },
                true,
                null,
                null,
                null,
                null),
            new OrganisationFunctionAdded(
                organisationId: organisationId,
                organisationFunctionId: organisationFunctionId,
                functionId: Guid.NewGuid(),
                functionName: Guid.NewGuid().ToString(),
                personId: personId,
                personFullName: Guid.NewGuid().ToString(),
                contacts: new Dictionary<Guid, string>(),
                validFrom: DateTime.Now.AddDays(-10),
                validTo: DateTime.Now.AddDays(10)),
            new OrganisationTerminated(
                organisationId,
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                dateOfTermination,
                new FieldsToTerminate(
                    organisationValidity: dateOfTermination,
                    buildings: new Dictionary<Guid, DateTime>(),
                    bankAccounts: new Dictionary<Guid, DateTime>(),
                    capacities: new Dictionary<Guid, DateTime>(),
                    contacts: new Dictionary<Guid, DateTime>(),
                    classifications: new Dictionary<Guid, DateTime>(),
                    functions: new Dictionary<Guid, DateTime>() { { organisationFunctionId, dateOfTermination } },
                    labels: new Dictionary<Guid, DateTime>(),
                    locations: new Dictionary<Guid, DateTime>(),
                    parents: new Dictionary<Guid, DateTime>(),
                    relations: new Dictionary<Guid, DateTime>(),
                    formalFrameworks: new Dictionary<Guid, DateTime>(),
                    openingHours: new Dictionary<Guid, DateTime>()),
                new KboFieldsToTerminate(
                    new Dictionary<Guid, DateTime>(),
                    null,
                    null,
                    null),
                false,
                dateOfTermination));

        Context.PersonFunctionList.Single(item => item.PersonId == personId).ValidTo.Should().Be(dateOfTermination);
    }

    [Fact]
    public async Task OrganisationTeminatedV2()
    {
        var personId = Guid.NewGuid();
        var organisationId = Guid.NewGuid();
        var organisationFunctionId = Guid.NewGuid();
        var dateOfTermination = DateTime.Now.AddYears(1);
        await HandleEvents(
            new OrganisationCreated(
                organisationId,
                Guid.NewGuid()
                    .ToString(),
                Guid.NewGuid()
                    .ToString(),
                null,
                null,
                null,
                new List<Purpose>()
                {
                },
                true,
                null,
                null,
                null,
                null),
            new OrganisationFunctionAdded(
                organisationId: organisationId,
                organisationFunctionId: organisationFunctionId,
                functionId: Guid.NewGuid(),
                functionName: Guid.NewGuid().ToString(),
                personId: personId,
                personFullName: Guid.NewGuid().ToString(),
                contacts: new Dictionary<Guid, string>(),
                validFrom: DateTime.Now.AddDays(-10),
                validTo: DateTime.Now.AddDays(10)),
            new OrganisationTerminatedV2(
                organisationId,
                Guid.NewGuid().ToString(),
                Guid.NewGuid().ToString(),
                dateOfTermination,
                new FieldsToTerminateV2(
                    organisationValidity: dateOfTermination,
                    buildings: new Dictionary<Guid, DateTime>(),
                    bankAccounts: new Dictionary<Guid, DateTime>(),
                    capacities: new Dictionary<Guid, DateTime>(),
                    contacts: new Dictionary<Guid, DateTime>(),
                    classifications: new Dictionary<Guid, DateTime>(),
                    functions: new Dictionary<Guid, DateTime>() { { organisationFunctionId, dateOfTermination } },
                    labels: new Dictionary<Guid, DateTime>(),
                    locations: new Dictionary<Guid, DateTime>(),
                    relations: new Dictionary<Guid, DateTime>(),
                    formalFrameworks: new Dictionary<Guid, DateTime>(),
                    openingHours: new Dictionary<Guid, DateTime>(),
                    regulations: new Dictionary<Guid, DateTime>(),
                    keys: new Dictionary<Guid, DateTime>()),
                new KboFieldsToTerminateV2(
                    new Dictionary<Guid, DateTime>(),
                    null,
                    null,
                    null),
                false,
                dateOfTermination));

        Context.PersonFunctionList.Single(item => item.PersonId == personId).ValidTo.Should().Be(dateOfTermination);
    }
}
