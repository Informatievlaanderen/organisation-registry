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

public class OrganisationClassificationTypePolicyTests
{
    private readonly Fixture _fixture;
    private readonly Guid _regelgevingDbClassificationTypeId;
    private readonly IOrganisationRegistryConfiguration _configuration;
    private readonly Guid _cjmClassificationTypeId;

    public OrganisationClassificationTypePolicyTests()
    {
        _fixture = new Fixture();

        _regelgevingDbClassificationTypeId = _fixture.Create<Guid>();
        _cjmClassificationTypeId = _fixture.Create<Guid>();
        _configuration = new OrganisationRegistryConfigurationStub
        {
            Authorization = new AuthorizationConfigurationStub
            {
                OrganisationClassificationTypeIdsOwnedByRegelgevingDbBeheerder = new[] { _regelgevingDbClassificationTypeId },
                OrganisationClassificationTypeIdsOwnedByCjm = new[] { _cjmClassificationTypeId },
            },
        };
    }

    private IUser UserWithRoles(params Role[] roles)
        => new UserBuilder()
            .AddRoles(roles)
            .WithPermissions(RolePermissionMap.For(roles, _configuration))
            .Build();

    private IUser UserWithRolesFor(string ovoNumber, params Role[] roles)
        => new UserBuilder()
            .AddRoles(roles)
            .AddOrganisations(ovoNumber)
            .WithPermissions(RolePermissionMap.For(roles, _configuration))
            .Build();

    private static OrganisationClassificationTypePolicy CreatePolicy(string ovoNumber, Guid organisationClassificationTypeId)
        => new(ovoNumber, organisationClassificationTypeId);

    [Theory]
    [InlineData(Role.RegelgevingBeheerder)]
    [InlineData(Role.AlgemeenBeheerder)]
    public void RegelgevingDbBeheerderAndAdminIsAuthorizedForRegelgeving(Role role)
    {
        var user = UserWithRoles(role);

        var authorizationResult =
            CreatePolicy(_fixture.Create<string>(), _regelgevingDbClassificationTypeId)
                .Check(user);

        authorizationResult.Should().Be(AuthorizationResult.Success());
    }

    [Theory]
    [InlineData(Role.DecentraalBeheerder)]
    [InlineData(Role.VlimpersBeheerder)]
    [InlineData(Role.Orafin)]
    [InlineData(Role.OrgaanBeheerder)]
    [InlineData(Role.CjmBeheerder)]
    public void NonRegelgevingDbBeheerderIsNotAuthorizedForRegelgeving(Role role)
    {
        var user = UserWithRoles(role);

        var authorizationResult =
            CreatePolicy(_fixture.Create<string>(), _regelgevingDbClassificationTypeId)
                .Check(user);

        authorizationResult.ShouldFailWith<InsufficientRights<OrganisationClassificationTypePolicy>>();
    }

    [Theory]
    [InlineData(Role.CjmBeheerder)]
    [InlineData(Role.AlgemeenBeheerder)]
    public void CjmClientAndAdminIsAuthorizedForCjm(Role role)
    {
        var user = UserWithRoles(role);

        var authorizationResult =
            CreatePolicy(_fixture.Create<string>(), _cjmClassificationTypeId)
                .Check(user);

        authorizationResult.Should().Be(AuthorizationResult.Success());
    }

    [Theory]
    [InlineData(Role.DecentraalBeheerder)]
    [InlineData(Role.VlimpersBeheerder)]
    [InlineData(Role.Orafin)]
    [InlineData(Role.OrgaanBeheerder)]
    [InlineData(Role.RegelgevingBeheerder)]
    public void NonCjmClientIsNotAuthorizedForCjm(Role role)
    {
        var user = UserWithRoles(role);

        var authorizationResult =
            CreatePolicy(_fixture.Create<string>(), _cjmClassificationTypeId)
                .Check(user);

        authorizationResult.ShouldFailWith<InsufficientRights<OrganisationClassificationTypePolicy>>();
    }

    [Fact]
    public void CjmClientIsNotAuthorizedForNonCjmClassificationTypes()
    {
        var ovoNumber = _fixture.Create<string>();
        var user = UserWithRolesFor(ovoNumber, Role.CjmBeheerder);

        var authorizationResult =
            CreatePolicy(ovoNumber, _fixture.Create<Guid>())
                .Check(user);

        authorizationResult.ShouldFailWith<InsufficientRights<OrganisationClassificationTypePolicy>>();
    }

    [Fact]
    public void BeheerderIsAuthorizedForOtherOrganisationClassificationTypesForTheirOrganisation()
    {
        var ovoNumber = _fixture.Create<string>();
        var user = UserWithRolesFor(ovoNumber, Role.DecentraalBeheerder);

        var authorizationResult =
            CreatePolicy(ovoNumber, _fixture.Create<Guid>())
                .Check(user);

        authorizationResult.Should().Be(AuthorizationResult.Success());
    }

    [Fact]
    public void BeheerderIsNotAuthorizedForRegelgevingDbOwnedOrganisationClassificationTypesForTheirOrganisation()
    {
        var ovoNumber = _fixture.Create<string>();
        var user = UserWithRolesFor(ovoNumber, Role.DecentraalBeheerder);

        var authorizationResult =
            CreatePolicy(ovoNumber, _regelgevingDbClassificationTypeId)
                .Check(user);

        authorizationResult.ShouldFailWith<InsufficientRights<OrganisationClassificationTypePolicy>>();
    }

    [Fact]
    public void BeheerderIsNotAuthorizedForOtherOrganisationClassificationTypesForOtherOrganisations()
    {
        var user = UserWithRolesFor(_fixture.Create<string>(), Role.DecentraalBeheerder);

        var authorizationResult =
            CreatePolicy(_fixture.Create<string>(), _fixture.Create<Guid>())
                .Check(user);

        authorizationResult.ShouldFailWith<InsufficientRights<OrganisationClassificationTypePolicy>>();
    }
}
