// namespace OrganisationRegistry.VlaanderenBeNotifier.UnitTests
// {
//     using System;
//     using System.Data;
//     using System.Data.Common;
//     using System.Threading;
//     using System.Threading.Tasks;
//     using Configuration;
//     using Infrastructure.Configuration;
//     using Infrastructure.Events;
//     using Microsoft.Data.SqlClient;
//     using Microsoft.EntityFrameworkCore;
//     using Microsoft.Extensions.Logging;
//     using Microsoft.Extensions.Options;
//     using Moq;
//     using Schema;
//     using SqlServer.Configuration;
//     using SqlServer.ProjectionState;
//     using Tests.Shared.TestDataBuilders;
//     using Xunit;
//
//     public class MailProcessorTests
//     {
//         private readonly OptionsWrapper<VlaanderenBeNotifierConfiguration> _vlaanderenBeNotifierConfigurationOptions;
//         private readonly IEventPublisher _publisher;
//         private readonly ILogger<Runner> _logger;
//         private readonly OptionsWrapper<TogglesConfiguration> _togglesConfigurationOptions;
//         private readonly DbContextOptions<VlaanderenBeNotifierContext> _dbContextOptions;
//
//         public MailProcessorTests()
//         {
//             _publisher = Mock.Of<IEventPublisher>();
//             _logger = Mock.Of<ILogger<Runner>>();
//             _vlaanderenBeNotifierConfigurationOptions = new OptionsWrapper<VlaanderenBeNotifierConfiguration>(new VlaanderenBeNotifierConfigurationTestDataBuilder());
//             _togglesConfigurationOptions = new OptionsWrapper<TogglesConfiguration>(new TogglesConfiguration() { VlaanderenBeNotifierAvailable = true });
//
//             _dbContextOptions = new DbContextOptionsBuilder<VlaanderenBeNotifierContext>()
//                 .UseInMemoryDatabase($"vlaanderenbe-notifier-mailprocessor-{Guid.NewGuid()}")
//                 .Options;
//         }
//
//         [Fact]
//         public void UpdatesProjectionStateWithLastEnvelopeIfAllSucceed()
//         {
//             const int lastEventNumber = 1;
//
//             var projectionStates = new Mock<IProjectionStates>();
//             projectionStates
//                 .Setup(states => states.GetLastProcessedEventNumber(Runner.VlaanderenbeNotifierProjectionName))
//                 .Returns(Task.FromResult(lastEventNumber));
//
//             var eventStore = new Mock<IEventStore>();
//             eventStore
//                 .Setup(store => store.GetEventEnvelopesAfter(lastEventNumber, It.IsAny<int>()))
//                 .Returns(() => new[]
//                 {
//                     new EnvelopeTestDataBuilder().WithNumber(2).Build(),
//                     new EnvelopeTestDataBuilder().WithNumber(3).Build()
//                 });
//
//             var connection = new Mock<DbConnection>();
//             connection.Setup(dbConnection => dbConnection.BeginTransactionAsync(It.IsAny<IsolationLevel>(), It.IsAny<CancellationToken>()))
//                 .ReturnsAsync(() => null);
//
//             var runner = new Runner(
//                 _logger,
//                 _togglesConfigurationOptions,
//                 _vlaanderenBeNotifierConfigurationOptions,
//                 eventStore.Object,
//                 projectionStates.Object,
//                 _publisher,
//                 Mock.Of<IOptions<SqlServerConfiguration>>(),
//                 () => new VlaanderenBeNotifierContext(new DbContextOptions<VlaanderenBeNotifierContext>()));
//
//             runner.Run();
//
//             projectionStates.Verify(states => states.UpdateProjectionState(Runner.VlaanderenbeNotifierProjectionName, 3, It.IsAny<DbConnection>(), It.IsAny<DbTransaction>()));
//         }
//
//         [Fact]
//         public void DoesNotUpdateProjectionStateWhenNoEventsAreSuccessfullyPublished()
//         {
//             const int lastEventNumber = 1;
//
//             var projectionStates = new Mock<IProjectionStates>();
//             projectionStates
//                 .Setup(states => states.GetLastProcessedEventNumber(Runner.VlaanderenbeNotifierProjectionName))
//                 .Returns(Task.FromResult(lastEventNumber));
//
//             var eventStore = new Mock<IEventStore>();
//             eventStore
//                 .Setup(store => store.GetEventEnvelopesAfter(lastEventNumber))
//                 .Returns(() => new[]
//                 {
//                     new EnvelopeTestDataBuilder().WithNumber(2).Build(),
//                     new EnvelopeTestDataBuilder().WithNumber(3).Build()
//                 });
//
//
//             var eventPublisher = new Mock<IEventPublisher>();
//             eventPublisher
//                 .Setup(publisher =>
//                     publisher.Publish(It.IsAny<DbConnection>(), It.IsAny<DbTransaction>(), It.IsAny<IEnvelope<IEvent<IEvent>>>()))
//                 .Throws<Exception>();
//
//             var runner = new Runner(
//                 _logger,
//                 _togglesConfigurationOptions,
//                 _vlaanderenBeNotifierConfigurationOptions,
//                 eventStore.Object,
//                 projectionStates.Object,
//                 eventPublisher.Object,
//                 Mock.Of<IOptions<SqlServerConfiguration>>(),
//                 () => new VlaanderenBeNotifierContext(_dbContextOptions));
//
//             Assert.ThrowsAsync<Exception>(() => runner.Run());
//
//             projectionStates.Verify(states => states.UpdateProjectionState(Runner.VlaanderenbeNotifierProjectionName, It.IsAny<int>(), null, null), Times.Never);
//         }
//
//         [Fact]
//         public void SavesTheLastSucceededEventNumberOnFail()
//         {
//             const int lastEventNumber = 1;
//
//             var projectionStates = new Mock<IProjectionStates>();
//             projectionStates
//                 .Setup(states => states.GetLastProcessedEventNumber(Runner.VlaanderenbeNotifierProjectionName))
//                 .Returns(Task.FromResult(lastEventNumber));
//
//             var eventStore = new Mock<IEventStore>();
//             eventStore
//                 .Setup(store => store.GetEventEnvelopesAfter(lastEventNumber))
//                 .Returns(() => new[]
//                 {
//                     new EnvelopeTestDataBuilder().WithNumber(2).Build(),
//                     new EnvelopeTestDataBuilder().WithNumber(3).Build()
//                 });
//
//
//             var eventPublisher = new Mock<IEventPublisher>();
//             eventPublisher
//                 .Setup(publisher =>
//                     publisher.Publish(It.IsAny<DbConnection>(), It.IsAny<DbTransaction>(),
//                         It.IsAny<IEnvelope<IEvent<IEvent>>>()))
//                 .Callback(() =>
//                     eventPublisher.Setup(publisher =>
//                         publisher.Publish(It.IsAny<DbConnection>(), It.IsAny<DbTransaction>(), It.IsAny<IEnvelope<IEvent<IEvent>>>()))
//                         .Throws<Exception>());
//
//             var runner = new Runner(
//                 _logger,
//                 _togglesConfigurationOptions,
//                 _vlaanderenBeNotifierConfigurationOptions,
//                 eventStore.Object,
//                 projectionStates.Object,
//                 eventPublisher.Object,
//                 Mock.Of<IOptions<SqlServerConfiguration>>(),
//                 () => new VlaanderenBeNotifierContext(_dbContextOptions));
//
//             Assert.ThrowsAsync<Exception>(() => runner.Run());
//
//             projectionStates.Verify(states => states.UpdateProjectionState(Runner.VlaanderenbeNotifierProjectionName, 2, null, null));
//         }
//     }
// }
