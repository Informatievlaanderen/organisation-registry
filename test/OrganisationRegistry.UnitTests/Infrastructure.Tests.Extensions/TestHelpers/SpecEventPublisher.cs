namespace OrganisationRegistry.UnitTests.Infrastructure.Tests.Extensions.TestHelpers;

using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using OrganisationRegistry.Infrastructure.Events;

internal class SpecEventPublisher : IEventPublisher
{
    public List<IEnvelope> PublishedEvents { get; set; }

    public SpecEventPublisher()
        => PublishedEvents = new List<IEnvelope>();

    public Task Publish<T>(DbConnection? dbConnection, DbTransaction? dbTransaction, IEnvelope<T> envelope)
        where T : IEvent<T>
    {
        PublishedEvents.Add(envelope);
        return Task.CompletedTask;
    }
}
