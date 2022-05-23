namespace OrganisationRegistry.UnitTests.SecurityPolicy;

using FluentAssertions;
using Handling.Authorization;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared;
using Xunit;

public class RegulationPolicyTests
{
    private static RegulationPolicy CreatePolicy()
        => new();

    [Theory]
    [InlineData(Role.RegelgevingBeheerder)]
    [InlineData(Role.AlgemeenBeheerder)]
    public void RegelgevingDbBeheerderAndAdminIsAuthorized(Role role)
    {
        var user = new UserBuilder()
            .AddRoles(role)
            .Build();

        var authorizationResult =
            CreatePolicy()
                .Check(user);

        authorizationResult.Should().Be(AuthorizationResult.Success());
    }

    [Theory]
    [InlineData(Role.DecentraalBeheerder)]
    [InlineData(Role.VlimpersBeheerder)]
    [InlineData(Role.Orafin)]
    [InlineData(Role.OrgaanBeheerder)]
    public void NonRegelgevingDbBeheerderIsNotAuthorized(Role role)
    {
        var user = new UserBuilder()
            .AddRoles(role)
            .Build();

        var authorizationResult =
            CreatePolicy()
                .Check(user);

        authorizationResult.ShouldFailWith<InsufficientRights>();
    }
}
