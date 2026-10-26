namespace OrganisationRegistry.UnitTests.SecurityPolicy;

using AutoFixture;
using FluentAssertions;
using Handling.Authorization;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Configuration;
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared;
using Tests.Shared.Stubs;
using Xunit;

public class ContactPolicyTests
{
    private readonly Fixture _fixture;
    private readonly IOrganisationRegistryConfiguration _configuration;

    public ContactPolicyTests()
    {
        _fixture = new Fixture();
        _configuration = new OrganisationRegistryConfigurationStub();
    }

    private IUser UserWithRolesFor(string ovoNumber, params Role[] roles)
        => new UserBuilder()
            .AddRoles(roles)
            .AddOrganisations(ovoNumber)
            .WithPermissions(RolePermissionMap.For(roles, _configuration))
            .Build();

    private static ContactPolicy CreatePolicy(string ovoNumber)
        => new(ovoNumber);

    [Theory]
    [InlineData(Role.AlgemeenBeheerder)]
    [InlineData(Role.Developer)]
    public void UnrestrictedRolesAreAuthorizedForAnyOrganisation(Role role)
    {
        var user = UserWithRolesFor(_fixture.Create<string>(), role);

        CreatePolicy(_fixture.Create<string>())
            .Check(user)
            .Should().Be(AuthorizationResult.Success());
    }

    [Fact]
    public void DecentraalBeheerderIsAuthorizedForTheirOwnOrganisation()
    {
        var ovoNumber = _fixture.Create<string>();
        var user = UserWithRolesFor(ovoNumber, Role.DecentraalBeheerder);

        CreatePolicy(ovoNumber)
            .Check(user)
            .Should().Be(AuthorizationResult.Success());
    }

    [Fact]
    public void DecentraalBeheerderIsNotAuthorizedForOtherOrganisations()
    {
        var user = UserWithRolesFor(_fixture.Create<string>(), Role.DecentraalBeheerder);

        CreatePolicy(_fixture.Create<string>())
            .Check(user)
            .ShouldFailWith<InsufficientRights<ContactPolicy>>();
    }

    [Theory]
    [InlineData(Role.VoDecentraalBeheerder)]
    [InlineData(Role.VlimpersBeheerder)]
    [InlineData(Role.RegelgevingBeheerder)]
    [InlineData(Role.Orafin)]
    [InlineData(Role.OrgaanBeheerder)]
    public void OtherRolesAreNotAuthorizedEvenForTheirOwnOrganisation(Role role)
    {
        var ovoNumber = _fixture.Create<string>();
        var user = UserWithRolesFor(ovoNumber, role);

        CreatePolicy(ovoNumber)
            .Check(user)
            .ShouldFailWith<InsufficientRights<ContactPolicy>>();
    }
}
