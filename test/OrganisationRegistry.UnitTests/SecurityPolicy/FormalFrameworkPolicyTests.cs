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

public class FormalFrameworkPolicyTests
{
    private readonly Fixture _fixture;
    private readonly Guid _regelgevingDbFormalFrameworkId;
    private readonly Guid _vlimpersFormalFrameworkId;
    private readonly Guid _otherFormalFrameworkId;
    private readonly IOrganisationRegistryConfiguration _configuration;

    public FormalFrameworkPolicyTests()
    {
        _fixture = new Fixture();

        _regelgevingDbFormalFrameworkId = _fixture.Create<Guid>();
        _vlimpersFormalFrameworkId = _fixture.Create<Guid>();
        _otherFormalFrameworkId = _fixture.Create<Guid>();
        _configuration = new OrganisationRegistryConfigurationStub
        {
            Authorization = new AuthorizationConfigurationStub
            {
                FormalFrameworkIdsOwnedByRegelgevingDbBeheerder = new[] { _regelgevingDbFormalFrameworkId },
                FormalFrameworkIdsOwnedByVlimpers = new[] { _vlimpersFormalFrameworkId },
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
                RolePermissionMap.For(
                    new[] { Role.DecentraalBeheerder },
                    _configuration))
            .Build();

    public FormalFrameworkPolicy CreatePolicy(string ovoNumber, Guid formalFrameworkId)
        => new(ovoNumber, formalFrameworkId);

    [Theory]
    [InlineData(Role.AlgemeenBeheerder)]
    [InlineData(Role.Developer)]
    public void UnrestrictedRolesAreAuthorizedForAnyFormalFramework(Role role)
    {
        var user = UserWithRoles(role);

        CreatePolicy(_fixture.Create<string>(), _vlimpersFormalFrameworkId)
            .Check(user)
            .Should().Be(AuthorizationResult.Success());
    }

    [Fact]
    public void CjmBeheerderIsNotAuthorizedForFormalFrameworks()
    {
        var user = UserWithRoles(Role.CjmBeheerder);

        CreatePolicy(_fixture.Create<string>(), _otherFormalFrameworkId)
            .Check(user)
            .ShouldFailWith<InsufficientRights<FormalFrameworkPolicy>>();
    }

    [Fact]
    public void RegelgevingDbBeheerderIsAuthorizedForRegelgevingDbOwnedFormalFramework()
    {
        var user = UserWithRoles(Role.RegelgevingBeheerder);

        CreatePolicy(_fixture.Create<string>(), _regelgevingDbFormalFrameworkId)
            .Check(user)
            .Should().Be(AuthorizationResult.Success());
    }

    [Fact]
    public void RegelgevingDbBeheerderIsNotAuthorizedForVlimpersOwnedFormalFramework()
    {
        var user = UserWithRoles(Role.RegelgevingBeheerder);

        CreatePolicy(_fixture.Create<string>(), _vlimpersFormalFrameworkId)
            .Check(user)
            .ShouldFailWith<InsufficientRights<FormalFrameworkPolicy>>();
    }

    [Fact]
    public void RegelgevingDbBeheerderIsNotAuthorizedForOtherFormalFramework()
    {
        var user = UserWithRoles(Role.RegelgevingBeheerder);

        CreatePolicy(_fixture.Create<string>(), _otherFormalFrameworkId)
            .Check(user)
            .ShouldFailWith<InsufficientRights<FormalFrameworkPolicy>>();
    }

    [Fact]
    public void VlimpersBeheerderIsAuthorizedForVlimpersOwnedFormalFramework()
    {
        var user = UserWithRoles(Role.VlimpersBeheerder);

        CreatePolicy(_fixture.Create<string>(), _vlimpersFormalFrameworkId)
            .Check(user)
            .Should().Be(AuthorizationResult.Success());
    }

    [Fact]
    public void VlimpersBeheerderIsNotAuthorizedForRegelgevingDbOwnedFormalFramework()
    {
        var user = UserWithRoles(Role.VlimpersBeheerder);

        CreatePolicy(_fixture.Create<string>(), _regelgevingDbFormalFrameworkId)
            .Check(user)
            .ShouldFailWith<InsufficientRights<FormalFrameworkPolicy>>();
    }

    [Fact]
    public void VlimpersBeheerderIsNotAuthorizedForOtherFormalFramework()
    {
        var user = UserWithRoles(Role.VlimpersBeheerder);

        CreatePolicy(_fixture.Create<string>(), _otherFormalFrameworkId)
            .Check(user)
            .ShouldFailWith<InsufficientRights<FormalFrameworkPolicy>>();
    }

    [Fact]
    public void DecentraalBeheerderIsAuthorizedForOtherFormalFrameworksForTheirOrganisation()
    {
        var ovoNumber = _fixture.Create<string>();
        var user = DecentraalBeheerderFor(ovoNumber);

        CreatePolicy(ovoNumber, _otherFormalFrameworkId)
            .Check(user)
            .Should().Be(AuthorizationResult.Success());
    }

    [Fact]
    public void DecentraalBeheerderIsAuthorizedForRegelgevingDbOwnedFormalFrameworksForTheirOrganisation()
    {
        var ovoNumber = _fixture.Create<string>();
        var user = DecentraalBeheerderFor(ovoNumber);

        CreatePolicy(ovoNumber, _regelgevingDbFormalFrameworkId)
            .Check(user)
            .Should().Be(AuthorizationResult.Success());
    }

    [Fact]
    public void DecentraalBeheerderIsNotAuthorizedForVlimpersOwnedFormalFrameworksForTheirOrganisation()
    {
        var ovoNumber = _fixture.Create<string>();
        var user = DecentraalBeheerderFor(ovoNumber);

        CreatePolicy(ovoNumber, _vlimpersFormalFrameworkId)
            .Check(user)
            .ShouldFailWith<InsufficientRights<FormalFrameworkPolicy>>();
    }

    [Fact]
    public void DecentraalBeheerderIsNotAuthorizedForOtherFormalFrameworksForOtherOrganisations()
    {
        var user = DecentraalBeheerderFor(_fixture.Create<string>());

        CreatePolicy(_fixture.Create<string>(), _otherFormalFrameworkId)
            .Check(user)
            .ShouldFailWith<InsufficientRights<FormalFrameworkPolicy>>();
    }
}
