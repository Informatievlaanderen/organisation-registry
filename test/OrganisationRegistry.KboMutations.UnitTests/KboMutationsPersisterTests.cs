namespace OrganisationRegistry.KboMutations.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autofac.Features.OwnedInstances;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging.Abstractions;
    using SqlServer;
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
                new KboMutationsPersister(
                    new ContextFactory(() => new Owned<OrganisationRegistryContext>(context, this)),
                    new NullLogger<KboMutationsPersister>());

            kboMutationsPersister.Persist("test.csv",
                new List<MutationsLine>
                {
                    new MutationsLine
                    {
                        DatumModificatie = DateTime.Today,
                        Ondernemingsnummer = "0123456789",
                        MaatschappelijkeNaam = "test",
                        StatusCode = KboStatusCodes.Active
                    },
                    new MutationsLine
                    {
                        DatumModificatie = DateTime.Today,
                        Ondernemingsnummer = "0123456789",
                        MaatschappelijkeNaam = "test",
                        StatusCode = KboStatusCodes.Terminated,
                        StopzettingsCode = "014",
                        StopzettingsDatum = DateTime.Today.AddDays(-1),
                        StopzettingsReden = "Sluiting van de vereffening"
                    }
                });

            context = new OrganisationRegistryContext(dbContextOptions);

            context.KboTerminationSyncQueue.Count().Should().Be(1);
            context.KboSyncQueue.Count().Should().Be(1);
        }

        public void Dispose()
        {
        }
    }
}
