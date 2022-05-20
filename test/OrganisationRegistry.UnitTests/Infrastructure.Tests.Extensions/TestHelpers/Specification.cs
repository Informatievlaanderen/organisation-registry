namespace OrganisationRegistry.UnitTests.Infrastructure.Tests.Extensions.TestHelpers;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Specialized;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Infrastructure.Events;
using Xunit.Abstractions;

public abstract class Specification<THandler, TCommand>
    where THandler : class, ICommandEnvelopeHandler<TCommand>
    where TCommand : ICommand
{
    private readonly ITestOutputHelper _helper;
    private readonly Func<ISession, THandler> _buildHandler;
    private readonly SpecEventStorage _eventstorage;
    private IUser _user = null!;
    private TCommand _command = default!;
    protected ISession Session { get; }
    protected IList<IEvent> EventDescriptors { get; }
    protected List<IEnvelope> PublishedEvents { get; }

    protected Specification(
        ITestOutputHelper helper,
        Func<ISession, THandler> buildHandler
    )
    {
        _helper = helper;
        _buildHandler = buildHandler;

        var eventpublisher = new SpecEventPublisher();
        _eventstorage = new SpecEventStorage(eventpublisher);

        var repository = new Repository(new Mock<ILogger<Repository>>().Object, _eventstorage);
        Session = new Session(new Mock<ILogger<Session>>().Object, repository);

        PublishedEvents = eventpublisher.PublishedEvents;
        EventDescriptors = _eventstorage.Events;
    }

    protected Specification<THandler, TCommand> Given(params IEvent[] events)
    {
        _eventstorage.Events = NumberTheEvents(events).ToList();
        return this;
    }

    public Specification<THandler, TCommand> When(TCommand command, IUser user)
    {
        _command = command;
        _user = user;
        return this;
    }

    public async Task Then()
    {
        var handler = _buildHandler(Session);
        await handler.Handle(new CommandEnvelope<TCommand>(_command, _user));
    }

    private async Task ThenTry()
    {
        try
        {
            var handler = _buildHandler(Session);
            await handler.Handle(new CommandEnvelope<TCommand>(_command, _user));
        }
        catch
        {
            // ignored
        }
    }

    public async Task<ExceptionAssertions<TException>> ThenThrows<TException>() where TException : Exception
        => await FluentActions.Invoking(async () => await Then()).Should()
            .ThrowAsync<TException>();

    public async Task ThenItPublishesTheCorrectNumberOfEvents(int numberOfEvents)
    {
        await ThenTry();
        PublishedEvents.ForEach(envelope => _helper.WriteLine(envelope.Name));
        PublishedEvents.Count.Should().Be(numberOfEvents);
    }

    private static IEnumerable<IEvent> NumberTheEvents(IEnumerable<IEvent> toList)
    {
        var ids = new Dictionary<Guid, int>();
        return toList.Select(
            @event =>
            {
                if (!ids.ContainsKey(@event.Id))
                    ids[@event.Id] = 0;

                ids[@event.Id] = ++ids[@event.Id];
                @event.Version = @event.Version != 0 ? @event.Version : ids[@event.Id];
                return @event;
            });
    }
}
