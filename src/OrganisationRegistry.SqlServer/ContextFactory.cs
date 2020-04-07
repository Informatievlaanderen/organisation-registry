namespace OrganisationRegistry.SqlServer
{
    using System;
    using System.Data.Common;
    using Autofac.Features.OwnedInstances;
    using Infrastructure;

    public interface IContextFactory
    {
        OrganisationRegistryContext CreateTransactional(DbConnection connection, DbTransaction transaction);
        OrganisationRegistryContext Create();
    }

    public class ContextFactory : IContextFactory
    {
        private readonly Func<Owned<OrganisationRegistryContext>> _contextFunc;

        public ContextFactory(Func<Owned<OrganisationRegistryContext>> contextFunc)
        {
            _contextFunc = contextFunc;
        }

        public OrganisationRegistryContext CreateTransactional(DbConnection connection, DbTransaction transaction)
        {
            return new OrganisationRegistryTransactionalContext(connection, transaction);
        }

        public OrganisationRegistryContext Create()
        {
            return _contextFunc().Value;
        }
    }
}
