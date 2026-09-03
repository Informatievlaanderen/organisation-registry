namespace OrganisationRegistry.UnitTests.SecurityPolicy;

using System.Linq;
using AutoFixture;
using FluentAssertions;
using Handling.Authorization;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared;
using Xunit;

public class BeheerderForOrganisationRegardlessOfVlimpersPolicyTests
{
    private readonly Fixture _fixture;

    public BeheerderForOrganisationRegardlessOfVlimpersPolicyTests()
    {
        _fixture = new Fixture();
    }

    [Fact]
    public void OrganisatieBeheerderIsAuthorizedIfNotVlimpers()
    {
        var ovoNumbers = _fixture.CreateMany<string>().ToArray();
        var user = new UserBuilder()
            .AddRoles(Role.DecentraalBeheerder)
            .AddOrganisations(ovoNumbers)
            .Build();

        var authorizationResult =
            new BeheerderForOrganisationRegardlessOfVlimpersPolicy(ovoNumber: ovoNumbers[_fixture.Create<int>() % ovoNumbers.Length])
                .Check(user);

        authorizationResult.Should().Be(AuthorizationResult.Success());
    }

    [Fact]
    public void OrganisatieBeheerderIsAuthorizedIfVlimpers()
    {
        var ovoNumbers = _fixture.CreateMany<string>().ToArray();
        var user = new UserBuilder()
            .AddRoles(Role.DecentraalBeheerder)
            .AddOrganisations(ovoNumbers)
            .Build();

        var authorizationResult =
            new BeheerderForOrganisationRegardlessOfVlimpersPolicy(ovoNumber: ovoNumbers[_fixture.Create<int>() % ovoNumbers.Length])
                .Check(user);

        authorizationResult.Should().Be(AuthorizationResult.Success());
    }

    [Theory]
    [InlineData(Role.AlgemeenBeheerder)]
    public void OrganisationRegistryBeheerderIsAlwaysAuthorized(Role role, params string[] organisationOvos)
    {
        var user = new UserBuilder()
            .AddRoles(role)
            .AddOrganisations(organisationOvos)
            .Build();

        var authorizationResult =
            new BeheerderForOrganisationRegardlessOfVlimpersPolicy(ovoNumber: _fixture.Create<string>())
                .Check(user);

        authorizationResult.Should().Be(AuthorizationResult.Success());
    }

    [Theory]
    [InlineData(Role.VlimpersBeheerder, "OVO000001")]
    [InlineData(Role.Orafin, "OVO000001")]
    public void VlimpersBeheerderAndOrafinIsNotAuthorized(Role role, params string[] ovoNumbers)
    {
        var user = new UserBuilder()
            .AddRoles(role)
            .AddOrganisations(ovoNumbers)
            .Build();

        var authorizationResult =
            new BeheerderForOrganisationRegardlessOfVlimpersPolicy(ovoNumber: ovoNumbers[_fixture.Create<int>() % ovoNumbers.Length])
                .Check(user);

        authorizationResult.ShouldFailWith<InsufficientRights<BeheerderForOrganisationRegardlessOfVlimpersPolicy>>();
    }
}
