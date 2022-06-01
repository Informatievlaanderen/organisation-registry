namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections;

using System;
using System.IO;
using System.Reflection;
using Microsoft.Extensions.Configuration;

public class SqlServerFixture
{
    public IConfigurationRoot Configuration { get; }

    public SqlServerFixture()
    {
        Directory.SetCurrentDirectory(Directory.GetParent(typeof(SqlServerFixture).GetTypeInfo().Assembly.Location)!.Parent!.Parent!.Parent!.FullName);

        Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true)
            .Build();
    }
}
