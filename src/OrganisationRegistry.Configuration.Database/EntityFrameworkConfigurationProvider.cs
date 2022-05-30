namespace OrganisationRegistry.Configuration.Database;

using System;
using System.Linq;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class EntityFrameworkConfigurationSource : IConfigurationSource
{
    private readonly Action<DbContextOptionsBuilder> _optionsAction;

    public EntityFrameworkConfigurationSource(Action<DbContextOptionsBuilder> optionsAction)
    {
        _optionsAction = optionsAction;
    }

    public IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        return new EntityFrameworkConfigurationProvider(_optionsAction);
    }
}

public class EntityFrameworkConfigurationProvider : ConfigurationProvider
{
    private readonly Action<DbContextOptionsBuilder> _optionsAction;

    public EntityFrameworkConfigurationProvider(Action<DbContextOptionsBuilder> optionsAction)
    {
        _optionsAction = optionsAction;
    }

    public override void Load()
    {
        var builder = new DbContextOptionsBuilder<ConfigurationContext>();
        _optionsAction(builder);

        using (var context = new ConfigurationContext(builder.Options))
        {
            context.Database.EnsureCreated();

            context.Database.ExecuteSqlRaw(@$"
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'OrganisationRegistry')
    EXEC('CREATE SCHEMA [{WellknownSchemas.OrganisationRegistrySchema}] AUTHORIZATION [dbo]');

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Configuration' AND xtype = 'U')
    CREATE TABLE [OrganisationRegistry].[Configuration] (
        [Key] nvarchar(450) NOT NULL,
        [Description] nvarchar(max),
        [Value] nvarchar(max),
        CONSTRAINT [PK_Configuration] PRIMARY KEY CLUSTERED ([Key])
    );");

            Data = context.Configuration.ToDictionary(x => x.Key, x => x.Value);
        }
    }
}