namespace OrganisationRegistry.Api.Infrastructure.Configuration;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Configuration.Database.Configuration;
using SqlServer.Configuration;

/// <summary>
/// Vult <c>Api:Orafin_KeyTypeId</c> eenmalig in wanneer die instelling leeg is.
/// <br />
/// Is <c>Api:Orafin_KeyTypeId</c> nog niet ingesteld, en staat er in de sleuteltypelijst precies
/// één (niet verwijderd) sleuteltype met de naam "Orafin", dan wordt het id daarvan in de
/// configuratietabel gezet. Bij geen of meerdere kandidaten gebeurt er niets, enkel een
/// waarschuwing in de log. Daarna is de instelling
/// niet langer leeg, waardoor dit vanzelf eenmalig blijft.
/// <br />
/// Moet lopen na de migraties en vóór de API-configuratie gebonden wordt; de configuratie wordt
/// daarom na het bijwerken herladen.
/// </summary>
public static class InitialiseOrafinKeyTypeConfiguration
{
    private const string SettingKey = "Api:Orafin_KeyTypeId";
    private const string OrafinKeyTypeName = "Orafin";

    public static void Run(IConfiguration configuration, ILogger logger)
    {
        if (Guid.TryParse(configuration[SettingKey], out var current) && current != Guid.Empty)
            return;

        Guid orafinKeyTypeId;
        using (var connection = new SqlConnection(configuration.GetSection(SqlServerConfiguration.Section).Get<SqlServerConfiguration>()!.ConnectionString))
        {
            connection.Open();

            using var find = new SqlCommand(
                "SELECT [Id] FROM [Backoffice].[KeyTypeList] WHERE [Name] = @Name AND [IsRemoved] = 0",
                connection);
            find.Parameters.AddWithValue("@Name", OrafinKeyTypeName);

            var ids = new List<Guid>();
            using (var reader = find.ExecuteReader())
            {
                while (reader.Read())
                    ids.Add(reader.GetGuid(0));
            }

            if (ids.Count != 1)
            {
                logger.LogWarning(
                    "{SettingKey} is not configured and {Count} key type(s) named {KeyTypeName} were found",
                    SettingKey, ids.Count, OrafinKeyTypeName);
                return;
            }

            orafinKeyTypeId = ids.Single();
        }

        using (var connection = new SqlConnection(configuration.GetSection(ConfigurationDatabaseConfiguration.Section).Get<ConfigurationDatabaseConfiguration>()!.ConnectionString))
        {
            connection.Open();

            using var upsert = new SqlCommand(
                """
                MERGE [OrganisationRegistry].[Configuration] AS target
                USING (VALUES (@Key, @Description, @Value)) AS source ([Key], [Description], [Value])
                ON target.[Key] = source.[Key]
                WHEN MATCHED THEN UPDATE SET [Value] = source.[Value]
                WHEN NOT MATCHED THEN INSERT ([Key], [Description], [Value]) VALUES (source.[Key], source.[Description], source.[Value]);
                """,
                connection);
            upsert.Parameters.AddWithValue("@Key", SettingKey);
            upsert.Parameters.AddWithValue("@Description", "KeyType GUID voor 'Orafin'");
            upsert.Parameters.AddWithValue("@Value", orafinKeyTypeId.ToString());
            upsert.ExecuteNonQuery();
        }

        if (configuration is IConfigurationRoot root)
            root.Reload();

        logger.LogInformation("{SettingKey} was not configured and has been set to {KeyTypeId}", SettingKey, orafinKeyTypeId);
    }
}
