namespace OrganisationRegistry.UnitTests.SecurityPolicy;

using System;
using AutoFixture;
using FluentAssertions;
using Handling.Authorization;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Configuration;
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared;
using Tests.Shared.Stubs;
using Xunit;

public class BodyPolicyTests
{
    private readonly Fixture _fixture;
    private readonly IOrganisationRegistryConfiguration _configuration;

    public BodyPolicyTests()
    {
        _fixture = new Fixture();
        _configuration = new OrganisationRegistryConfigurationStub();
    }

    private IUser UserWithRoles(params Role[] roles)
        => new UserBuilder()
            .AddRoles(roles)
            .WithPermissions(RolePermissionMap.For(roles, _configuration))
            .Build();

    private IUser DecentraalBeheerderForBody(Guid bodyId)
        => new UserBuilder()
            .AddRoles(Role.DecentraalBeheerder)
            .AddBodies(bodyId)
            .WithPermissions(
                RolePermissionMap.For(new[] { Role.DecentraalBeheerder }, _configuration))
            .Build();

    private static BodyPolicy CreatePolicy(Permission permission, Guid bodyId)
        => new(permission, bodyId);

    [Theory]
    [InlineData(Role.AlgemeenBeheerder)]
    [InlineData(Role.Developer)]
    [InlineData(Role.OrgaanBeheerder)]
    [InlineData(Role.CjmBeheerder)]
    public void UnrestrictedRolesAreAuthorizedForAnyBody(Role role)
    {
        var user = UserWithRoles(role);

        CreatePolicy(Permission.BodiesCanManageContacts, _fixture.Create<Guid>())
            .Check(user)
            .Should().Be(AuthorizationResult.Success());
    }

    [Fact]
    public void DecentraalBeheerderIsAuthorizedForTheirOwnBody()
    {
        var bodyId = _fixture.Create<Guid>();
        var user = DecentraalBeheerderForBody(bodyId);

        CreatePolicy(Permission.BodiesCanManageContacts, bodyId)
            .Check(user)
            .Should().Be(AuthorizationResult.Success());
    }

    [Fact]
    public void DecentraalBeheerderIsNotAuthorizedForOtherBodies()
    {
        var user = DecentraalBeheerderForBody(_fixture.Create<Guid>());

        CreatePolicy(Permission.BodiesCanManageContacts, _fixture.Create<Guid>())
            .Check(user)
            .ShouldFailWith<InsufficientRights<BodyPolicy>>();
    }

    [Fact]
    public void DecentraalBeheerderIsNeverAuthorizedForMepEvenOnTheirOwnBody()
    {
        var bodyId = _fixture.Create<Guid>();
        var user = DecentraalBeheerderForBody(bodyId);

        CreatePolicy(Permission.BodiesCanManageMep, bodyId)
            .Check(user)
            .ShouldFailWith<InsufficientRights<BodyPolicy>>();
    }

    [Theory]
    [InlineData(Role.AlgemeenBeheerder)]
    [InlineData(Role.OrgaanBeheerder)]
    public void UnrestrictedRolesAreAuthorizedForMep(Role role)
    {
        var user = UserWithRoles(role);

        CreatePolicy(Permission.BodiesCanManageMep, _fixture.Create<Guid>())
            .Check(user)
            .Should().Be(AuthorizationResult.Success());
    }

    [Theory]
    [InlineData(Role.RegelgevingBeheerder)]
    [InlineData(Role.Orafin)]
    public void RolesWithoutPermissionAreNotAuthorized(Role role)
    {
        var user = UserWithRoles(role);

        CreatePolicy(Permission.BodiesCanManageContacts, _fixture.Create<Guid>())
            .Check(user)
            .ShouldFailWith<InsufficientRights<BodyPolicy>>();
    }
}
