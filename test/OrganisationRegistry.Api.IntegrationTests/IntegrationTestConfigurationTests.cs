namespace OrganisationRegistry.Api.IntegrationTests;

using System;
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
        configuration["EditApi:Authority"].Should().Be("http://keycloak.localhost:9080/realms/wegwijs");
        configuration["EditApi:IntrospectionEndpoint"].Should().Be("http://keycloak.localhost:9080/realms/wegwijs/protocol/openid-connect/token/introspect");
    }

    [Fact]
    public void TiltConfiguration_ShouldUseTiltEndpoints()
    {
        var configuration = BuildConfiguration();

        configuration["ApiIntegrationTests:ApiBaseUrl"].Should().Be("http://api.localhost:9080");
        configuration["Api:KboMagdaEndpoint"].Should().Be("http://mock.localhost:9080");
        configuration["Api:RepertoriumMagdaEndpoint"].Should().Be("http://mock.localhost:9080");
        configuration["Api:KboV2RegisteredOfficeLocationTypeId"].Should().Be("a7e93f04-0004-0000-0000-000000000001");
        configuration["Api:KboV2LegalFormOrganisationClassificationTypeId"].Should().Be("a7e93f06-0006-0000-0000-000000000001");
        configuration["Api:KboV2FormalNameLabelTypeId"].Should().Be("a7e93f02-0002-0000-0000-000000000004");
        configuration["Authorization:KeyIdsAllowedOnlyForOrafin"].Should().Be("1e3611a7-7914-411a-a0c9-84fcd6218e67");
    }

    private static IConfigurationRoot BuildConfiguration()
    {
        var maybeProjectDirectory = Directory
            .GetParent(typeof(IntegrationTestConfigurationTests).GetTypeInfo().Assembly.Location)?.Parent?.Parent?.Parent?.FullName;

        maybeProjectDirectory.Should().NotBeNull();

        return new ConfigurationBuilder()
            .SetBasePath(maybeProjectDirectory!)
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{Environment.MachineName}.json", optional: true)
            .AddJsonFile($"appsettings.{Environment.MachineName.ToLowerInvariant()}.json", optional: true)
            .Build();
    }
}
