namespace OrganisationRegistry.UnitTests.SecurityPolicy;

using System;
using AutoFixture;
using FluentAssertions;
using Handling.Authorization;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared;
using Tests.Shared.Stubs;
using Xunit;

public class FormalFrameworkPolicyTests
{
    private readonly Fixture _fixture;
    private readonly Guid _regelgevingDbFormalFrameworkId;
    private readonly Guid _vlimpersFormalFrameworkId;
    private readonly OrganisationRegistryConfigurationStub _configuration;

    public FormalFrameworkPolicyTests()
    {
        _fixture = new Fixture();

        _regelgevingDbFormalFrameworkId = _fixture.Create<Guid>();
        _vlimpersFormalFrameworkId = _fixture.Create<Guid>();
        _configuration = new OrganisationRegistryConfigurationStub
        {
            Authorization = new AuthorizationConfigurationStub
            {
                FormalFrameworkIdsOwnedByRegelgevingDbBeheerder = new[] { _regelgevingDbFormalFrameworkId },
                FormalFrameworkIdsOwnedByVlimpers = new[] { _vlimpersFormalFrameworkId },
            },
        };
    }

    public FormalFrameworkPolicy CreatePolicy(string ovoNumber, Guid formalFrameworkId)
        => new(ovoNumber, formalFrameworkId, _configuration);

    [Theory]
    [InlineData(Role.RegelgevingBeheerder)]
    [InlineData(Role.AlgemeenBeheerder)]
    public void RegelgevingDbBeheerderAndAdminIsAuthorized(Role role)
    {
        var user = new UserBuilder()
            .AddRoles(role)
            .Build();

        var authorizationResult =
            CreatePolicy(_fixture.Create<string>(), _regelgevingDbFormalFrameworkId)
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
            CreatePolicy(_fixture.Create<string>(), _regelgevingDbFormalFrameworkId)
                .Check(user);

        authorizationResult.ShouldFailWith<InsufficientRights<FormalFrameworkPolicy>>();
    }

    [Theory]
    [InlineData(Role.VlimpersBeheerder)]
    [InlineData(Role.AlgemeenBeheerder)]
    public void VlimpersBeheerderAndAdminIsAuthorizedForVlimpersFormalFramework(Role role)
    {
        var user = new UserBuilder()
            .AddRoles(role)
            .Build();

        var authorizationResult =
            CreatePolicy(_fixture.Create<string>(), _vlimpersFormalFrameworkId)
                .Check(user);

        authorizationResult.Should().Be(AuthorizationResult.Success());
    }

    [Theory]
    [InlineData(Role.Orafin)]
    [InlineData(Role.DecentraalBeheerder)]
    [InlineData(Role.OrgaanBeheerder)]
    [InlineData(Role.RegelgevingBeheerder)]
    public void NonVlimpersBeheerderIsNotAuthorizedForVlimpersFormalFramework(Role role)
    {
        var user = new UserBuilder()
            .AddRoles(role)
            .Build();

        var authorizationResult =
            CreatePolicy(_fixture.Create<string>(), _vlimpersFormalFrameworkId)
                .Check(user);

        authorizationResult.ShouldFailWith<InsufficientRights<FormalFrameworkPolicy>>();
    }

    [Fact]
    public void BeheerderIsAuthorizedForOtherFormalFrameworksForTheirOrganisation()
    {
        var ovoNumber = _fixture.Create<string>();
        var user = new UserBuilder()
            .AddRoles(Role.DecentraalBeheerder)
            .AddOrganisations(ovoNumber)
            .Build();

        var authorizationResult =
            CreatePolicy(ovoNumber, _fixture.Create<Guid>())
                .Check(user);

        authorizationResult.Should().Be(AuthorizationResult.Success());
    }

    [Fact]
    public void BeheerderIsNotAuthorizedForVlimpersOwnedFormalFrameworksForTheirOrganisation()
    {
        var ovoNumber = _fixture.Create<string>();
        var user = new UserBuilder()
            .AddRoles(Role.DecentraalBeheerder)
            .AddOrganisations(ovoNumber)
            .Build();

        var authorizationResult =
            CreatePolicy(ovoNumber, _vlimpersFormalFrameworkId)
                .Check(user);

        authorizationResult.ShouldFailWith<InsufficientRights<FormalFrameworkPolicy>>();
    }

    [Fact]
    public void BeheerderIsNotAuthorizedForRegelgevingDbOwnedFormalFrameworksForTheirOrganisation()
    {
        var ovoNumber = _fixture.Create<string>();
        var user = new UserBuilder()
            .AddRoles(Role.DecentraalBeheerder)
            .AddOrganisations(ovoNumber)
            .Build();

        var authorizationResult =
            CreatePolicy(ovoNumber, _regelgevingDbFormalFrameworkId)
                .Check(user);

        authorizationResult.ShouldFailWith<InsufficientRights<FormalFrameworkPolicy>>();
    }

    [Fact]
    public void BeheerderIsNotAuthorizedForOtherFormalFrameworksForOtherOrganisations()
    {
        var user = new UserBuilder()
            .AddRoles(Role.DecentraalBeheerder)
            .AddOrganisations(_fixture.Create<string>())
            .Build();

        var authorizationResult =
            CreatePolicy(_fixture.Create<string>(), _fixture.Create<Guid>())
                .Check(user);

        authorizationResult.ShouldFailWith<InsufficientRights<FormalFrameworkPolicy>>();
    }
}
