namespace OrganisationRegistry.UnitTests.EventStore
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.Options;
    using Moq;
    using Newtonsoft.Json;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Bus;
    using OrganisationRegistry.Infrastructure.Configuration;
    using OrganisationRegistry.Infrastructure.EventStore;
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

            var dataReader = new Mock<IEventDataReader>();
            dataReader.Setup(i => i.GetEvents(It.IsAny<Guid>(), It.IsAny<int>()))
                .Returns(eventData!.ToList());

            var sqlServerEventStore = new SqlServerEventStore(
                new OptionsWrapper<InfrastructureConfigurationSection>(new InfrastructureConfigurationSection()),
                new NullPublisher(),
                new NotImplementedSecurityService(),
                dataReader.Object);

            OrganisationInfoUpdatedFromKbo infoUpdatedFromKbo =
                (OrganisationInfoUpdatedFromKbo) sqlServerEventStore
                    .Get<OrganisationRegistry.Organisation.Organisation>(Guid.NewGuid(), 0)
                    .Last();

            infoUpdatedFromKbo.Name.Should().Be(" La Barraca");
        }
    }
}
