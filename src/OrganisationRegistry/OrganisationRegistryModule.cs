namespace OrganisationRegistry;

using System;
using System.Linq;
using Autofac;
using System.Reflection;
using Handling.Authorization;
using Infrastructure.Commands;

public class OrganisationRegistryModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        builder.RegisterAssemblyTypes(typeof(OrganisationRegistryAssemblyTokenClass).GetTypeInfo().Assembly)
            .Where(type => !type.IsClosedTypeOf(typeof(IEquatable<>)))
            .Where(t => !t.GetInterfaces().Any(i => i == typeof(ISecurityPolicy)))
            .AsImplementedInterfaces();

        builder.RegisterAssemblyTypes(typeof(BaseCommand).GetTypeInfo().Assembly)
            .AsClosedTypesOf(typeof(ICommandEnvelopeHandler<>))
            .InstancePerLifetimeScope();
    }
}
