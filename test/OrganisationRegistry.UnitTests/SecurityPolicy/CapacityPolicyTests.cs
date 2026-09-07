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

public class CapacityPolicyTests
{
    private readonly Fixture _fixture;
    private readonly Guid _regelgevingDbCapacityId;
    private readonly IOrganisationRegistryConfiguration _configuration;

    public CapacityPolicyTests()
    {
        _fixture = new Fixture();

        _regelgevingDbCapacityId = _fixture.Create<Guid>();
        _configuration = new OrganisationRegistryConfigurationStub
        {
            Authorization = new AuthorizationConfigurationStub
            {
                CapacityIdsOwnedByRegelgevingDbBeheerder = new[] { _regelgevingDbCapacityId },
            },
        };
    }

    private IUser UserWithRoles(params Role[] roles)
        => new UserBuilder()
            .AddRoles(roles)
            .WithPermissions(RolePermissionMap.For(roles, _configuration))
            .Build();

    private IUser DecentraalBeheerderFor(string ovoNumber)
        => new UserBuilder()
            .AddRoles(Role.DecentraalBeheerder)
            .AddOrganisations(ovoNumber)
            .WithPermissions(
                RolePermissionMap.For(new[] { Role.DecentraalBeheerder }, _configuration))
            .Build();

    private static CapacityPolicy CreatePolicy(string ovoNumber, Guid capacityId)
        => new(ovoNumber, capacityId);

    [Theory]
    [InlineData(Role.RegelgevingBeheerder)]
    [InlineData(Role.AlgemeenBeheerder)]
    public void RegelgevingDbBeheerderAndAdminIsAuthorized(Role role)
    {
        var user = UserWithRoles(role);

        var authorizationResult =
            CreatePolicy(_fixture.Create<string>(), _regelgevingDbCapacityId)
                .Check(user);

        authorizationResult.Should().Be(AuthorizationResult.Success());
    }

    [Fact]
    public void AutomatedTaskIsAuthorized()
    {
        var user = UserWithRoles(Role.AutomatedTask);

        var authorizationResult =
            CreatePolicy(_fixture.Create<string>(), _regelgevingDbCapacityId)
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
        var user = UserWithRoles(role);

        var authorizationResult =
            CreatePolicy(_fixture.Create<string>(), _regelgevingDbCapacityId)
                .Check(user);

        authorizationResult.ShouldFailWith<InsufficientRights<CapacityPolicy>>();
    }

    [Fact]
    public void BeheerderIsAuthorizedForOtherCapacitiesForTheirOrganisation()
    {
        var ovoNumber = _fixture.Create<string>();
        var user = DecentraalBeheerderFor(ovoNumber);

        var authorizationResult =
            CreatePolicy(ovoNumber, _fixture.Create<Guid>())
                .Check(user);

        authorizationResult.Should().Be(AuthorizationResult.Success());
    }

    [Fact]
    public void BeheerderIsNotAuthorizedForRegelgevingDbOwnedCapacitiesForTheirOrganisation()
    {
        var ovoNumber = _fixture.Create<string>();
        var user = DecentraalBeheerderFor(ovoNumber);

        var authorizationResult =
            CreatePolicy(ovoNumber, _regelgevingDbCapacityId)
                .Check(user);

        authorizationResult.ShouldFailWith<InsufficientRights<CapacityPolicy>>();
    }

    [Fact]
    public void BeheerderIsNotAuthorizedForOtherCapacitiesForOtherOrganisations()
    {
        var user = DecentraalBeheerderFor(_fixture.Create<string>());

        var authorizationResult =
            CreatePolicy(_fixture.Create<string>(), _fixture.Create<Guid>())
                .Check(user);

        authorizationResult.ShouldFailWith<InsufficientRights<CapacityPolicy>>();
    }
}
