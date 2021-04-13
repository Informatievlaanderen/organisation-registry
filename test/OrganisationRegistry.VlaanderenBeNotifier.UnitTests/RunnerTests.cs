namespace OrganisationRegistry.VlaanderenBeNotifier.UnitTests
{
    using System.Threading.Tasks;
    using Configuration;
    using FluentAssertions;
    using Infrastructure.Configuration;
    using Infrastructure.Events;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Moq;
    using SqlServer.ProjectionState;
    using Xunit;

    public class RunnerTests
    {
        private readonly IEventPublisher _publisher;
        private readonly IProjectionStates _projectionStates;
        private readonly IEventStore _eventStore;
        private readonly ILogger<Runner> _logger;
        private readonly OptionsWrapper<VlaanderenBeNotifierConfiguration> _vlaanderenBeNotifierConfigurationOptions;

        public RunnerTests()
        {
            _eventStore = Mock.Of<IEventStore>();
            _projectionStates = Mock.Of<IProjectionStates>();
            _publisher = Mock.Of<IEventPublisher>();
            _logger = Mock.Of<ILogger<Runner>>();
            _vlaanderenBeNotifierConfigurationOptions = new OptionsWrapper<VlaanderenBeNotifierConfiguration>(new VlaanderenBeNotifierConfigurationTestDataBuilder());
        }

        [Fact]
        public async Task ReturnsFalseWhenNotEnabled()
        {
            var togglesConfiguration =
                new OptionsWrapper<TogglesConfiguration>(new TogglesConfiguration { VlaanderenBeNotifierAvailable = false });

            var runner =
                new Runner(
                    _logger,
                    togglesConfiguration,
                    _vlaanderenBeNotifierConfigurationOptions,
                    _eventStore,
                    _projectionStates,
                    _publisher);

            var result = await runner.Run();
            result.Should().BeFalse();
        }

        [Fact]
        public async Task ReturnsTrueWhenEnabled()
        {
            var togglesConfiguration =
                new OptionsWrapper<TogglesConfiguration>(new TogglesConfiguration { VlaanderenBeNotifierAvailable = true });

            var runner = new Runner(
                _logger,
                togglesConfiguration,
                _vlaanderenBeNotifierConfigurationOptions,
                _eventStore,
                _projectionStates,
                _publisher);

            var result = await runner.Run();
            result.Should().BeTrue();
        }
    }
}
