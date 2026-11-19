namespace OrganisationRegistry.Api.Infrastructure.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.Infrastructure.Events;
using SqlServer.Configuration;
using SqlServer.Infrastructure;
using RebuildProjection = OrganisationRegistry.Infrastructure.Commands.RebuildProjection;
using RebuildProjectionEvent = OrganisationRegistry.Infrastructure.Events.RebuildProjection;

/// <summary>
/// Bouwt de projecties uit de whitelist <c>Projections:InitialiseWhenEmpty</c> eenmalig op bij het
/// opstarten, voordat de API verzoeken behandelt.
/// <br />
/// Een nieuwe projectie verwerkt enkel events die na de deploy binnenkomen; zonder rebuild blijft
/// haar read model leeg of onvolledig. Een projectie uit de whitelist wordt herbouwd wanneer al
/// haar tabellen leeg zijn terwijl de event store dat niet is. Na de rebuild is dat niet langer zo,
/// waardoor dit vanzelf eenmalig blijft.
/// </summary>
public static class InitialiseEmptyProjectionsExtension
{
    public const string WhitelistConfigurationKey = "Projections:InitialiseWhenEmpty";

    public static IApplicationBuilder InitialiseEmptyProjections(this IApplicationBuilder app)
    {
        var services = app.ApplicationServices;
        var logger = services.GetRequiredService<ILoggerFactory>().CreateLogger(typeof(InitialiseEmptyProjectionsExtension));

        var whitelist = services.GetRequiredService<IConfiguration>()
            .GetSection(WhitelistConfigurationKey)
            .Get<string[]>() ?? Array.Empty<string>();

        if (!whitelist.Any())
            return app;

        var projections = services
            .GetServices<IEventHandler<RebuildProjectionEvent>>()
            .OfType<IProjectionMarker>()
            .ToDictionary(projection => projection.GetType().FullName!);

        var connectionString = services
            .GetRequiredService<IConfiguration>()
            .GetSection(SqlServerConfiguration.Section)
            .Get<SqlServerConfiguration>()!
            .ConnectionString;

        using var connection = new SqlConnection(connectionString);
        connection.Open();

        if (!HasRows(connection, "[OrganisationRegistry].[Events]"))
            return app;

        var commandSender = services.GetRequiredService<ICommandSender>();

        foreach (var projectionName in whitelist)
        {
            if (!projections.TryGetValue(projectionName, out var projection))
            {
                logger.LogWarning("Projection {ProjectionName} from the startup whitelist does not exist", projectionName);
                continue;
            }

            if (projection.TableNames.Any(table => HasRows(connection, $"[{projection.Schema}].[{table}]")))
                continue;

            logger.LogInformation("Read model of {ProjectionName} is empty, rebuilding before startup", projectionName);

            commandSender.Send(new RebuildProjection(projectionName), WellknownUsers.ProjectionInitialiser).GetAwaiter().GetResult();

            logger.LogInformation("Rebuild of {ProjectionName} finished", projectionName);
        }

        return app;
    }

    private static bool HasRows(SqlConnection connection, string table)
    {
        using var command = new SqlCommand($"SELECT TOP 1 1 FROM {table}", connection);
        return command.ExecuteScalar() is not null;
    }
}
