namespace OrganisationRegistry.Infrastructure.Domain;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Authorization;

public interface IRepository
{
    Task Save<T>(T aggregate, IUser user, int? expectedVersion = null) where T : AggregateRoot;

    Task Save<T>(IEnumerable<T> aggregates, IUser user, int? expectedVersion = null) where T : AggregateRoot;

    T Get<T>(Guid aggregateId) where T : AggregateRoot;
}
