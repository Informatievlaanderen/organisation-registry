namespace OrganisationRegistry.SqlServer.IntegrationTests.TestBases
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Api;
    using Autofac;
    using Autofac.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection;
    using OnEventStore;
    using OrganisationRegistry.Infrastructure;
    using OrganisationRegistry.Infrastructure.Config;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Infrastructure.EventStore;
    using OrganisationRegistry.Magda;

    /// <summary>
    /// Use the EventStoreIntegrationTestBase if you want integration tests starting from events being published to the event store.
    /// These events will be published, persisted and reacted to (ie: fire async commands) like they would in the live application.
    /// </summary>
    /// <typeparam name="TEvent"></typeparam>
    public abstract class EventStoreIntegrationTestBase<TEvent> : IDisposable where TEvent : IEvent<TEvent>
    {
        protected readonly AutofacServiceProvider AutofacServiceProvider;

        protected abstract IEnumerable<IEvent> Given();
        protected abstract TEvent When();
        protected IDateTimeProvider DateTimeProvider { get; }

        protected EventStoreIntegrationTestBase(IDateTimeProvider dateTimeProvider, EventStoreSqlServerFixture fixture)
        {
            DateTimeProvider = dateTimeProvider;

            var builder = new ContainerBuilder();
            var services = new ServiceCollection();

            builder.RegisterModule(new InfrastructureModule(fixture.Config, ProvideScopedServiceProvider, new ServiceCollection()));
            builder.RegisterModule(new SqlServerModule(fixture.Config, services, null));
            builder.RegisterModule(new MagdaModule(fixture.Config));
            builder.RegisterModule(new ApiModule(fixture.Config, services, null));
            builder.RegisterInstance(dateTimeProvider).As<IDateTimeProvider>();

            AutofacServiceProvider = new AutofacServiceProvider(builder.Build());

            var registrar = AutofacServiceProvider.GetService<BusRegistrar>();
            registrar.RegisterCommandHandlersFromAssembly(typeof(BaseCommand));
            registrar.RegisterEventHandlersFromAssembly(typeof(OrganisationRegistrySqlServerAssemblyTokenClass));
            registrar.RegisterReactionHandlersFromAssembly(typeof(OrganisationRegistrySqlServerAssemblyTokenClass));

            var eventstore = AutofacServiceProvider.GetService<IEventStore>();
            ((SqlServerEventStore)eventstore).InitaliseEventStore(); // TODO: can we move this to the fixture?

            var allEvents = Given().Numbered().Stamped().ToList();

            eventstore.Save<IEvent>(allEvents);

            allEvents = allEvents.Append(When()).Numbered().Stamped().ToList();

            eventstore.Save<IEvent>(new List<IEvent> { allEvents.Last() });
        }

        private static Func<IServiceProvider> ProvideScopedServiceProvider(IComponentContext context)
        {
            var defaultServiceProvider = context.Resolve<IServiceProvider>();
            return () => defaultServiceProvider.CreateScope().ServiceProvider;
        }

        public void Dispose()
        {
        }
    }
}
