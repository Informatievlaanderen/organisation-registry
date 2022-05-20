namespace OrganisationRegistry.UnitTests;

using FluentAssertions.Equivalency;
using OrganisationRegistry.Infrastructure.Events;

public static class EquivalencyAssertionOptionsExtentions
{
    public static EquivalencyAssertionOptions<TEvent> IgnoreEventProperties<TEvent>(
        this EquivalencyAssertionOptions<TEvent> options) where TEvent : IEvent
        => options.Excluding(e => e.Timestamp).Excluding(e => e.Version);
}
