namespace OrganisationRegistry
{
    using System;
    using Autofac;
    using System.Reflection;
    using Infrastructure.Commands;

    public class OrganisationRegistryModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(OrganisationRegistryAssemblyTokenClass).GetTypeInfo().Assembly)
                .Where(type => !type.IsClosedTypeOf(typeof(IEquatable<>)))
                .AsImplementedInterfaces();

            builder.RegisterAssemblyTypes(typeof(BaseCommand).GetTypeInfo().Assembly)
                .AsClosedTypesOf(typeof(ICommandHandler<>))
                .InstancePerLifetimeScope();
        }
    }
}
