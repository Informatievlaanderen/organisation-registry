namespace OrganisationRegistry.VlaanderenBeNotifier.Schema;

using System;
using System.Data.Common;
using Autofac.Features.OwnedInstances;

public interface IContextFactory
{
    VlaanderenBeNotifierContext CreateTransactional(DbConnection connection, DbTransaction transaction);
    VlaanderenBeNotifierContext Create();
}

public class ContextFactory : IContextFactory
{
    private readonly Func<Owned<VlaanderenBeNotifierContext>> _contextFunc;

    public ContextFactory(Func<Owned<VlaanderenBeNotifierContext>> contextFunc)
    {
        _contextFunc = contextFunc;
    }

    public VlaanderenBeNotifierContext CreateTransactional(DbConnection connection, DbTransaction transaction)
    {
        return new VlaanderenBeNotifierTransactionalContext(connection, transaction);
    }

    public VlaanderenBeNotifierContext Create()
    {
        return _contextFunc().Value;
    }
}
