namespace OrganisationRegistry.ElasticSearch.Projections.Infrastructure;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Change;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public class ElasticBusRegistrar
{
    private readonly ILogger<ElasticBusRegistrar> _logger;
    private readonly ElasticBus _bus;
    private readonly Func<IServiceProvider> _requestScopedServiceProvider;

    public ElasticBusRegistrar(
        ILogger<ElasticBusRegistrar> logger,
        ElasticBus bus,
        Func<IServiceProvider> requestScopedServiceProvider)
    {
        _logger = logger;
        _bus = bus;
        _requestScopedServiceProvider = requestScopedServiceProvider;

        _logger.LogTrace("Creating BusRegistrar");
    }

    public void RegisterEventHandlers(params Type[] eventHandlerTypes)
        => RegisterHandlers(typeof(IElasticEventHandler<>), InvokeEventHandler, eventHandlerTypes);

    private void InvokeEventHandler(Type @interface, Type executorType)
        => InvokeHandler(
            @interface,
            executorType,
            nameof(ElasticBus.RegisterEventHandler),
            new Func<DbConnection, DbTransaction, dynamic, Task<IElasticChange>>(async (dbConnection, dbTransaction, envelope) =>
            {
                LoggerExtensions.LogTrace(_logger, "Executing inner event handler for {EventName} - {@Event} using {ExecutorType}", envelope.GetType(), envelope, executorType);

                try
                {
                    var serviceProvider = _requestScopedServiceProvider();
                    dynamic handler = serviceProvider.GetRequiredService(executorType);
                    return await handler.Handle(dbConnection, dbTransaction, envelope);
                }
                finally
                {
                    LoggerExtensions.LogTrace(_logger, "Finished executing inner event handler for {EventName} - {@Event}", envelope.GetType(), envelope);

                }
            }));


    private static void RegisterHandlers(Type handlerInterfaceType, Action<Type, Type> invokeHandlerAction, params Type[] reactionHandlerTypes)
    {
        var executorTypes = reactionHandlerTypes
            .Where(t => !t.GetTypeInfo().IsAbstract)
            .Select(t => new { Type = t, Interfaces = ResolveHandlerInterface(t, handlerInterfaceType) })
            .Where(e => e.Interfaces.Any());

        foreach (var executorType in executorTypes)
        foreach (var @interface in executorType.Interfaces)
            invokeHandlerAction(@interface, executorType.Type);
    }

    private static IEnumerable<Type> ResolveHandlerInterface(Type type, Type handlerInterfaceType)
        => type
            .GetInterfaces()
            .Where(i =>
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
