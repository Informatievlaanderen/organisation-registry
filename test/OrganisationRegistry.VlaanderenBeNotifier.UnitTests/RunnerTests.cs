namespace OrganisationRegistry.VlaanderenBeNotifier.UnitTests
{
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
        private readonly IEventStore _eventstore;
        private readonly ILogger<Runner> _logger;
        private readonly OptionsWrapper<VlaanderenBeNotifierConfiguration> _vlaanderenBeNotifierConfigurationOptions;

        public RunnerTests()
        {
            _eventstore = Mock.Of<IEventStore>();
            _projectionStates = Mock.Of<IProjectionStates>();
            _publisher = Mock.Of<IEventPublisher>();
            _logger = Mock.Of<ILogger<Runner>>();
            _vlaanderenBeNotifierConfigurationOptions = new OptionsWrapper<VlaanderenBeNotifierConfiguration>(new VlaanderenBeNotifierConfigurationTestDataBuilder());
        }

        [Fact]
        public void ReturnsFalseWhenNotEnabled()
        {
            var togglesConfiguration =
                new OptionsWrapper<TogglesConfiguration>(new TogglesConfiguration { VlaanderenBeNotifierAvailable = false });

            var runner =
                new Runner(
                    _logger,
                    togglesConfiguration,
                    _vlaanderenBeNotifierConfigurationOptions,
                    _eventstore,
                    _projectionStates,
                    _publisher);

            runner.Run().Should().BeFalse();
        }

        [Fact]
        public void ReturnsTrueWhenEnabled()
        {
            var togglesConfiguration =
                new OptionsWrapper<TogglesConfiguration>(new TogglesConfiguration { VlaanderenBeNotifierAvailable = true });

            var runner = new Runner(
                _logger,
                togglesConfiguration,
                _vlaanderenBeNotifierConfigurationOptions,
                _eventstore,
                _projectionStates,
                _publisher);

            runner.Run().Should().BeTrue();
        }
    }
}
