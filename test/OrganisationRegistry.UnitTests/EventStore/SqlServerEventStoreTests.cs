namespace OrganisationRegistry.UnitTests
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.Options;
    using Moq;
    using Newtonsoft.Json;
    using OrganisationRegistry.Infrastructure.Bus;
    using OrganisationRegistry.Infrastructure.Configuration;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Infrastructure.EventStore;
    using OrganisationRegistry.Infrastructure.Infrastructure.Json;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;

    public class SqlServerEventStoreTests
    {
        [Fact]
        public async Task LoadsEventsWithSpaces()
        {
            var path = Path.Join("EventStore", $"events.json");
            var lines = await File.ReadAllTextAsync(path);
            var eventData = JsonConvert.DeserializeObject<EventData[]>(lines);

            var dataInterface = new Mock<IEventDataReader>();
            dataInterface.Setup(i => i.Get(It.IsAny<Guid>(), It.IsAny<int>()))
                .Returns(eventData.ToList());

            var sqlServerEventStore = new SqlServerEventStore(
                new OptionsWrapper<InfrastructureConfiguration>(new InfrastructureConfiguration()),
                new NullPublisher(),
                dataInterface.Object);

            OrganisationInfoUpdatedFromKbo infoUpdatedFromKbo =
                (OrganisationInfoUpdatedFromKbo) sqlServerEventStore
                    .Get<OrganisationRegistry.Organisation.Organisation>(Guid.NewGuid(), 0)
                    .Last();

            infoUpdatedFromKbo.Name.Should().Be(" La Barraca");
        }
    }
}
