namespace OrganisationRegistry.UnitTests.SecurityPolicy;

using FluentAssertions;
using Handling.Authorization;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared;
using Xunit;

public class VlimpersManagementPolicyTests
{
    private static VlimpersManagementPolicy CreatePolicy()
        => new();

    [Theory]
    [InlineData(Role.AlgemeenBeheerder)]
    public void AdminIsAuthorized(Role role)
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
    [InlineData(Role.RegelgevingBeheerder)]
    [InlineData(Role.CjmBeheerder)]
    [InlineData(Role.Orafin)]
    [InlineData(Role.OrgaanBeheerder)]
    public void NonAdminIsNotAuthorized(Role role)
    {
        var user = new UserBuilder()
            .AddRoles(role)
            .AddOrganisations("OVO000001")
            .Build();

        var authorizationResult =
            CreatePolicy()
                .Check(user);

        authorizationResult.ShouldFailWith<InsufficientRights<VlimpersManagementPolicy>>();
    }
}
