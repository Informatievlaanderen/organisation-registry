namespace OrganisationRegistry.Tests.Shared;

using FluentAssertions.Equivalency;
using Infrastructure.Events;

public static class EquivalencyAssertionOptionsExtentions
{
    public static EquivalencyAssertionOptions<TEvent> ExcludeEventProperties<TEvent>(
        this EquivalencyAssertionOptions<TEvent> options) where TEvent : IEvent
        => options.Excluding(e => e.Timestamp).Excluding(e => e.Version);
}
