namespace OrganisationRegistry.KboMutations.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging.Abstractions;
    using SqlServer.Infrastructure;
    using Xunit;

    public class KboMutationsPersisterTests : IDisposable
    {
        [Fact]
        public void PersistsToTerminationSyncQueue()
        {
            var dbContextOptions = new DbContextOptionsBuilder<OrganisationRegistryContext>()
                .UseInMemoryDatabase("kbopersister").Options;

            var context = new OrganisationRegistryContext(dbContextOptions);

            var kboMutationsPersister =
                new KboMutationsPersister(new NullLogger<KboMutationsPersister>());

            kboMutationsPersister.Persist(context,
                "test.csv",
                new List<MutationsLine>
                {
                    new MutationsLine
                    {
                        DatumModificatie = DateTime.Today,
                        Ondernemingsnummer = "0123456789",
                        MaatschappelijkeNaam = "test",
                        StatusCode = "AC",
                    },
                    new MutationsLine
                    {
                        DatumModificatie = DateTime.Today,
                        Ondernemingsnummer = "0123456789",
                        MaatschappelijkeNaam = "test",
                        StatusCode = "ST",
                        StopzettingsCode = "014",
                        StopzettingsDatum = DateTime.Today.AddDays(-1),
                        StopzettingsReden = "Sluiting van de vereffening"
                    }
                });

            context = new OrganisationRegistryContext(dbContextOptions);

            context.KboSyncQueue.Count().Should().Be(2);
        }

        public void Dispose()
        {
        }
    }
}
