namespace OrganisationRegistry.UnitTests.Infrastructure.Tests.Extensions.TestHelpers;

using OrganisationRegistry.Infrastructure.Events;

public static class EnvelopeExtensions
{
    public static T UnwrapBody<T>(this IEnvelope source) where T : IEvent<T>
        => ((IEnvelope<T>) source).Body;
}
