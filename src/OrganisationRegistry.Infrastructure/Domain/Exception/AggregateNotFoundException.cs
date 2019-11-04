namespace OrganisationRegistry.Infrastructure.Domain.Exception
{
    using System;

    public class AggregateNotFoundException : Exception
    {
        public Type T { get; }
        public Guid Id { get; }

        public AggregateNotFoundException(Type t, Guid id)
            : base($"Aggregate {id} of type {t.FullName} was not found")
        {
            T = t;
            Id = id;
        }
    }
}
