namespace OrganisationRegistry.Infrastructure.Config
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Reflection;
    using Bus;
    using Commands;
    using System.Linq;
    using System.Threading.Tasks;
    using Events;
    using Microsoft.Extensions.Logging;

    public class BusRegistrar
    {
        private readonly ILogger<BusRegistrar> _logger;
        private readonly IHandlerRegistrar _bus;
        private readonly Func<IServiceProvider> _requestScopedServiceProvider;

        public BusRegistrar(
            ILogger<BusRegistrar> logger,
            IHandlerRegistrar bus,
            Func<IServiceProvider> requestScopedServiceProvider)
        {
            _logger = logger;
            _bus = bus;
            _requestScopedServiceProvider = requestScopedServiceProvider;

            _logger.LogTrace("Creating BusRegistrar");
        }

        public void RegisterCommandHandlersFromAssembly(params Type[] typesFromAssemblyContainingCommandHandlers)
            => RegisterHandlersFromAssembly(RegisterCommandHandlers, typesFromAssemblyContainingCommandHandlers);

        public void RegisterCommandHandlers(params Type[] commandHandlerTypes)
            => RegisterHandlers(typeof(ICommandHandler<>), InvokeCommandHandler, commandHandlerTypes);

        public void RegisterCommandEnvelopeHandlers(Type commandType)
        {
            var commandTypes = GetAllTypesImplementingOpenGenericType(commandType, commandType.Assembly).Distinct().ToList();

            foreach (var type in commandTypes)
            {
                try
                {
                    var commandEnvelopeHandlerWrapper = Activator.CreateInstance(typeof(CommandEnvelopeHandlerWrapper<>).MakeGenericType(type), _requestScopedServiceProvider);

                    if (commandEnvelopeHandlerWrapper is ICommandEnvelopeHandlerWrapper commandEnvelopeHandler)
                        _bus.RegisterCommandEnvelopeHandler(commandEnvelopeHandler);
                }
                catch (Exception)
                {
                    Console.WriteLine($"Cannot create instance of EnvelopeHandler for command '{type.FullName}'");
                }
            }
        }

        public static IEnumerable<Type> GetAllTypesImplementingOpenGenericType(Type openGenericType, Assembly assembly)
            => from x in assembly.GetTypes()
                from z in x.GetInterfaces()
                let y = x.BaseType
                where
                    (y is { IsGenericType: true } && openGenericType.IsAssignableFrom(y.GetGenericTypeDefinition())) ||
                    (z.IsGenericType && openGenericType.IsAssignableFrom(z.GetGenericTypeDefinition()))
                select x;

        public void RegisterEventHandlersFromAssembly(params Type[] typesFromAssemblyContainingEventHandlers)
            => RegisterHandlersFromAssembly(RegisterEventHandlers, typesFromAssemblyContainingEventHandlers);

        public void RegisterEventHandlers(params Type[] eventHandlerTypes)
            => RegisterHandlers(typeof(IEventHandler<>), InvokeEventHandler, eventHandlerTypes);

        public void RegisterReactionHandlersFromAssembly(params Type[] typesFromAssemblyContainingReactionHandlers)
            => RegisterHandlersFromAssembly(RegisterReactionHandlers, typesFromAssemblyContainingReactionHandlers);

        public void RegisterReactionHandlers(params Type[] reactionHandlerTypes)
            => RegisterHandlers(typeof(IReactionHandler<>), InvokeReactionHandler, reactionHandlerTypes);

        private void InvokeCommandHandler(Type @interface, Type executorType)
            => InvokeHandler(
                @interface,
                executorType,
                nameof(IHandlerRegistrar.RegisterCommandHandler),
                new Func<dynamic, Task>(
                    async x =>
                    {
                        LoggerExtensions.LogTrace(_logger, "Executing inner command handler for {CommandName} - {@Command} using {ExecutorType}", x.GetType(), x, executorType);

                        var serviceProvider = _requestScopedServiceProvider();
                        dynamic handler = serviceProvider.GetService(executorType);
                        await handler.Handle(x);

                        LoggerExtensions.LogTrace(_logger, "Finished executing inner command handler for {CommandName} - {@Command}", x.GetType(), x);
                    }));

        private void InvokeEventHandler(Type @interface, Type executorType)
            => InvokeHandler(
                @interface,
                executorType,
                nameof(IHandlerRegistrar.RegisterEventHandler),
                new Func<DbConnection, DbTransaction, dynamic, Task>(
                    async (dbConnection, dbTransaction, envelope) =>
                    {
                        LoggerExtensions.LogTrace(_logger, "Executing inner event handler for {EventName} - {@Event} using {ExecutorType}", envelope.GetType(), envelope, executorType);

                        var serviceProvider = _requestScopedServiceProvider();
                        dynamic handler = serviceProvider.GetService(executorType);
                        await handler.Handle(dbConnection, dbTransaction, envelope);

                        LoggerExtensions.LogTrace(_logger, "Finished executing inner event handler for {EventName} - {@Event}", envelope.GetType(), envelope);
                    }));

        private void InvokeReactionHandler(Type @interface, Type executorType)
            => InvokeHandler(
                @interface,
                executorType,
                nameof(IHandlerRegistrar.RegisterReaction),
                new Func<dynamic, Task<List<ICommand>>>(
                    async envelope =>
                    {
                        LoggerExtensions.LogTrace(_logger, "Executing inner reaction handler for {ReactionName} - {@Event} using {ExecutorType}", envelope.GetType(), envelope, executorType);

                        var serviceProvider = _requestScopedServiceProvider();
                        dynamic handler = serviceProvider.GetService(executorType);
                        var commands = await handler.Handle(envelope);

                        LoggerExtensions.LogTrace(_logger, "Finished executing inner reaction handler for {ReactionName} - {@Event}, resulting in {@Commands}", envelope.GetType(), envelope, commands);

                        return commands;
                    }));

        private static void RegisterHandlersFromAssembly(Action<Type[]> registerHandlersAction, params Type[] typesFromAssemblyContainingHandlers)
            => typesFromAssemblyContainingHandlers
                .ToList()
                .ForEach(typesFromAssemblyContainingMessage => registerHandlersAction(typesFromAssemblyContainingMessage.GetTypeInfo().Assembly.GetTypes()));

        private static void RegisterHandlers(Type handlerInterfaceType, Action<Type, Type> invokeHandlerAction, params Type[] reactionHandlerTypes)
        {
            var executorTypes = reactionHandlerTypes
                .Where(t => !t.GetTypeInfo().IsAbstract)
                .Select(t => new { Type = t, Interfaces = ResolveHandlerInterface(t, handlerInterfaceType) })
                .Where(e => e.Interfaces.Any()).ToList();

            foreach (var executorType in executorTypes)
            foreach (var @interface in executorType.Interfaces)
                invokeHandlerAction(@interface, executorType.Type);
        }

        private static IEnumerable<Type> ResolveHandlerInterface(Type type, Type handlerInterfaceType)
            => type
                .GetInterfaces()
                .Where(
                    i =>
                        i.GetTypeInfo().IsGenericType &&
                        i.GetGenericTypeDefinition() == handlerInterfaceType);

        private void InvokeHandler(Type @interface, Type executorType, string registerHandlerName, object handlerInvokerDelegate)
        {
            var messageType = @interface.GetGenericArguments()[0];

            var registerExecutorMethod = _bus
                .GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(mi => mi.Name == registerHandlerName)
                .Where(mi => mi.IsGenericMethod)
                .Where(mi => mi.GetGenericArguments().Length == 1)
                .Single(mi => mi.GetParameters().Length == 1)
                .MakeGenericMethod(messageType);

            _logger.LogTrace("Registering {ExecutorType} for {MessageType}", executorType, messageType);
            registerExecutorMethod.Invoke(_bus, new[] { handlerInvokerDelegate });
        }
    }
}
