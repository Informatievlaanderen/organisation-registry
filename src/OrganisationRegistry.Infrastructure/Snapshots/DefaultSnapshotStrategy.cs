namespace OrganisationRegistry.Infrastructure.Snapshots
{
    using System;
    using System.Reflection;
    using Domain;
    using System.Linq;

    public class DefaultSnapshotStrategy : ISnapshotStrategy
    {
        private const int SnapshotInterval = 100;

        public bool IsSnapshotable(Type aggregateType)
        {
            if (aggregateType.GetTypeInfo().BaseType == null)
            {
                return false;
            }
            if (aggregateType.GetTypeInfo().BaseType.GetTypeInfo().IsGenericType && aggregateType.GetTypeInfo().BaseType.GetGenericTypeDefinition() == typeof(SnapshotAggregateRoot<>))
            {
                return true;
            }
            return IsSnapshotable(aggregateType.GetTypeInfo().BaseType);
        }

        public bool ShouldMakeSnapShot(AggregateRoot aggregate)
        {
            if (!IsSnapshotable(aggregate.GetType()))
                return false;

            var i = aggregate.Version;

            for (var j = 0; j < aggregate.GetUncommittedChanges().Count(); j++)
            {
                if (++i % SnapshotInterval == 0 && i != 0)
                    return true;
            }

            return false;
        }
    }
}
