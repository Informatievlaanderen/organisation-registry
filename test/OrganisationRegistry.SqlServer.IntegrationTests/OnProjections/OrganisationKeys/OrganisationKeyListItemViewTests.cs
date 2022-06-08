namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.OrganisationKeys;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using OrganisationRegistry.Organisation.Events;
using TestBases;
using Xunit;

// todo: this test should talk to a real database
[Collection(SqlServerTestsCollection.Name)]
public class OrganisationKeyListItemViewTests : ListViewTestBase
{
    [Fact]
    public async Task OrganisationTerminatedV2()
    {
        var fixture = new Fixture();

        var organisationId = fixture.Create<Guid>();
        var name = fixture.Create<string>();
        var ovoNumber = fixture.Create<string>();
        var creationDate = fixture.Create<DateTime>();
        var terminationDate = creationDate.AddDays(fixture.Create<int>());

        await HandleEvents(
            new OrganisationCreated(
                organisationId,
                name,
                ovoNumber,
                null,
                null,
                null,
                new List<Purpose>(),
                fixture.Create<bool>(),
                creationDate,
                null,
                null,
                null),
            new OrganisationTerminatedV2(
                organisationId,
                name,
                ovoNumber,
                terminationDate,
                new FieldsToTerminateV2(
                    null,
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>()
                    ),
                new KboFieldsToTerminateV2(new Dictionary<Guid, DateTime>(), null, null, null),
                false));

        // Should not throw an exception
    }
}
