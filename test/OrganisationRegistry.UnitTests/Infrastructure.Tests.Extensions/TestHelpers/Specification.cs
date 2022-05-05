namespace OrganisationRegistry.UnitTests.Infrastructure.Tests.Extensions.TestHelpers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Infrastructure.Domain;
    using OrganisationRegistry.Infrastructure.Domain.Exception;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Infrastructure.Snapshots;
    using Xunit;
    using Xunit.Abstractions;

    public abstract class Specification<TAggregate, THandler, TCommand>
        where TAggregate: AggregateRoot
        where THandler : class, ICommandEnvelopeHandler<TCommand>
        where TCommand : ICommand
    {
        private readonly ITestOutputHelper _helper;
        protected TAggregate Aggregate { get; set; }
        protected ISession Session { get; set; }
        protected abstract IEnumerable<IEvent> Given();
        protected abstract TCommand When();
        protected abstract THandler BuildHandler();
        protected abstract IUser User { get; }

        protected abstract int ExpectedNumberOfEvents { get; }

        protected Snapshot Snapshot { get; set; }
        protected IList<IEvent> EventDescriptors { get; set; }
        protected List<IEnvelope> PublishedEvents { get; set; }

        protected Specification(ITestOutputHelper helper)
        {
            _helper = helper;
            var eventpublisher = new SpecEventPublisher();
            var eventstorage = new SpecEventStorage(eventpublisher, NumberTheEvents(Given()).ToList());
            var snapshotstorage = new SpecSnapShotStorage(Snapshot);

            var snapshotStrategy = new DefaultSnapshotStrategy();
            var repository = new SnapshotRepository(snapshotstorage, snapshotStrategy, new Repository(new Mock<ILogger<Repository>>().Object, eventstorage), eventstorage);
            Session = new Session(new Mock<ILogger<Session>>().Object, repository);

            try
            {
                //Aggregate = Session.Get<TAggregate>(Guid.Empty);
            }
            catch (AggregateNotFoundException)
            {
            }

            HandleEvents().GetAwaiter().GetResult();

            Snapshot = snapshotstorage.Snapshot;
            PublishedEvents = eventpublisher.PublishedEvents;
            EventDescriptors = eventstorage.Events;
        }

        protected virtual async Task HandleEvents()
        {
            var handler = BuildHandler();
            var command = When();

            await handler.Handle(new CommandEnvelope<TCommand>(command, User));
        }

        protected IEnumerable<IEvent> NumberTheEvents(IEnumerable<IEvent> toList)
        {
            var ids = new Dictionary<Guid, int>();
            return toList.Select(@event =>
            {
                if (!ids.ContainsKey(@event.Id))
                    ids[@event.Id] = 0;

                ids[@event.Id] = ++ids[@event.Id];
                @event.Version = @event.Version != 0 ? @event.Version : ids[@event.Id];
                return @event;
            });
        }

        [Fact]
        public void PublishesTheCorrectNumberOfEvents()
        {
            PublishedEvents.ForEach(envelope => _helper.WriteLine(envelope.Name));
            PublishedEvents.Count.Should().Be(ExpectedNumberOfEvents);
        }
    }
}
