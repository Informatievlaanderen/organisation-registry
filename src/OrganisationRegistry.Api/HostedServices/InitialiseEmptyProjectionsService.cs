namespace OrganisationRegistry.Api.HostedServices;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.Infrastructure.Events;
using SqlServer.Configuration;
using SqlServer.Infrastructure;
using RebuildProjection = OrganisationRegistry.Infrastructure.Commands.RebuildProjection;
using RebuildProjectionEvent = OrganisationRegistry.Infrastructure.Events.RebuildProjection;

/// <summary>
/// Bouwt de projecties uit de whitelist <c>Projections:InitialiseWhenEmpty</c> eenmalig op, op de
/// achtergrond nadat de API opgestart is.
/// <br />
/// Een nieuwe projectie verwerkt enkel events die na de deploy binnenkomen; zonder rebuild blijft
/// haar read model leeg of onvolledig. Een projectie uit de whitelist wordt herbouwd wanneer al
/// haar tabellen leeg zijn terwijl de event store dat niet is. Na de rebuild is dat niet langer zo,
/// waardoor dit vanzelf eenmalig blijft.
/// <br />
/// De rebuild loopt in één transactie: zolang die bezig is, wachten schrijfacties die dezelfde
/// read model-tabellen raken.
/// </summary>
public class InitialiseEmptyProjectionsService : BackgroundService
{
    public const string WhitelistConfigurationKey = "Projections:InitialiseWhenEmpty";

    private readonly IConfiguration _configuration;
    private readonly IHostApplicationLifetime _lifetime;
    private readonly ICommandSender _commandSender;
    private readonly IEnumerable<IEventHandler<RebuildProjectionEvent>> _rebuildHandlers;

    public InitialiseEmptyProjectionsService(
        ILogger<InitialiseEmptyProjectionsService> logger,
        IConfiguration configuration,
        IHostApplicationLifetime lifetime,
        ICommandSender commandSender,
        IEnumerable<IEventHandler<RebuildProjectionEvent>> rebuildHandlers) : base(logger)
    {
        _configuration = configuration;
        _lifetime = lifetime;
        _commandSender = commandSender;
        _rebuildHandlers = rebuildHandlers;
    }

    protected override async Task Process(CancellationToken cancellationToken)
    {
        var whitelist = _configuration
            .GetSection(WhitelistConfigurationKey)
            .Get<string[]>() ?? Array.Empty<string>();

        if (!whitelist.Any())
            return;

        // Pas starten wanneer de API volledig opgestart is: dan zijn de event handlers
        // geregistreerd en antwoordt de API al op health checks.
        await WaitForApplicationStarted(cancellationToken);

        if (cancellationToken.IsCancellationRequested)
            return;

        var projections = _rebuildHandlers
            .OfType<IProjectionMarker>()
            .ToDictionary(projection => projection.GetType().FullName!);

        var connectionString = _configuration
            .GetSection(SqlServerConfiguration.Section)
            .Get<SqlServerConfiguration>()!
            .ConnectionString;

        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        if (!await HasRows(connection, "[OrganisationRegistry].[Events]", cancellationToken))
            return;

        foreach (var projectionName in whitelist)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            if (!projections.TryGetValue(projectionName, out var projection))
            {
                Logger.LogWarning("Projection {ProjectionName} from the startup whitelist does not exist", projectionName);
                continue;
            }

            if (await AnyTableHasRows(connection, projection, cancellationToken))
                continue;

            Logger.LogInformation("Read model of {ProjectionName} is empty, rebuilding in the background", projectionName);

            await _commandSender.Send(new RebuildProjection(projectionName), WellknownUsers.ProjectionInitialiser);

            Logger.LogInformation("Rebuild of {ProjectionName} finished", projectionName);
        }
    }

    private Task WaitForApplicationStarted(CancellationToken cancellationToken)
    {
        var started = new TaskCompletionSource();
        _lifetime.ApplicationStarted.Register(() => started.TrySetResult());
        cancellationToken.Register(() => started.TrySetResult());
        return started.Task;
    }

    private static async Task<bool> AnyTableHasRows(SqlConnection connection, IProjectionMarker projection, CancellationToken cancellationToken)
    {
        foreach (var table in projection.TableNames)
        {
            if (await HasRows(connection, $"[{projection.Schema}].[{table}]", cancellationToken))
                return true;
        }

        return false;
    }

    private static async Task<bool> HasRows(SqlConnection connection, string table, CancellationToken cancellationToken)
    {
        await using var command = new SqlCommand($"SELECT TOP 1 1 FROM {table}", connection);
        return await command.ExecuteScalarAsync(cancellationToken) is not null;
    }
}
