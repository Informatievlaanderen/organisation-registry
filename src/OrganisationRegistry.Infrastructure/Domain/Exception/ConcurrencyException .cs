namespace OrganisationRegistry.Infrastructure.Domain.Exception
{
    using System;

    public class ConcurrencyException : Exception
    {
        public ConcurrencyException(Guid id)
            : base($"A different version than expected was found in aggregate {id}") { }

        public ConcurrencyException(Guid id, int expectedVersion)
            : base($"Expected version {expectedVersion} in aggregate {id}") { }

        public ConcurrencyException(Guid id, int expectedVersion, int aggregateVersion)
            : base($"Expected version {expectedVersion} but found {aggregateVersion} in aggregate {id}") { }
    }
}
