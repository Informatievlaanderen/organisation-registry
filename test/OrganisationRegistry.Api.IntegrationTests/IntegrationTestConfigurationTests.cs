namespace OrganisationRegistry.Api.IntegrationTests;

using System.IO;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

public class IntegrationTestConfigurationTests
{
    [Fact]
    public void EditApiConfiguration_ShouldUseKeycloakEndpoints()
    {
        var configuration = BuildConfiguration();

        configuration["EditApi:ClientId"].Should().Be("organisation-registry-api");
        configuration["EditApi:Authority"].Should().Be("http://localhost:8180/realms/wegwijs");
        configuration["EditApi:IntrospectionEndpoint"].Should().Be("http://localhost:8180/realms/wegwijs/protocol/openid-connect/token/introspect");
    }

    [Fact]
    public void MagdaConfiguration_ShouldUseWiremockEndpoints()
    {
        var configuration = BuildConfiguration();

        configuration["Api:KboMagdaEndpoint"].Should().Be("http://localhost:8080");
        configuration["Api:RepertoriumMagdaEndpoint"].Should().Be("http://localhost:8080");
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var maybeProjectDirectory = Directory
            .GetParent(typeof(IntegrationTestConfigurationTests).GetTypeInfo().Assembly.Location)?.Parent?.Parent?.Parent?.FullName;

        maybeProjectDirectory.Should().NotBeNull();

        return new ConfigurationBuilder()
            .SetBasePath(maybeProjectDirectory!)
            .AddJsonFile("appsettings.json")
            .Build();
    }
}
