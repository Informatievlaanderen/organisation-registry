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

public class CapacityPolicyTests
{
    private readonly Fixture _fixture;
    private readonly Guid _regelgevingDbCapacityId;
    private readonly OrganisationRegistryConfigurationStub _configuration;

    public CapacityPolicyTests()
    {
        _fixture = new Fixture();

        _regelgevingDbCapacityId = _fixture.Create<Guid>();
        _configuration = new OrganisationRegistryConfigurationStub
        {
            Authorization = new AuthorizationConfigurationStub
            {
                CapacityIdsOwnedByRegelgevingDbBeheerder = new[] { _regelgevingDbCapacityId },
            }
        };
    }

    public CapacityPolicy CreatePolicy(string ovoNumber, Guid capacityId)
        => new(ovoNumber, _configuration, capacityId);

    [Theory]
    [InlineData(Role.RegelgevingBeheerder)]
    [InlineData(Role.AlgemeenBeheerder)]
    public void RegelgevingDbBeheerderAndAdminIsAuthorized(Role role)
    {
        var user = new UserBuilder()
            .AddRoles(role)
            .Build();

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
        var user = new UserBuilder()
            .AddRoles(role)
            .Build();

        var authorizationResult =
            CreatePolicy(_fixture.Create<string>(), _regelgevingDbCapacityId)
                .Check(user);

        authorizationResult.ShouldFailWith<InsufficientRights>();
    }

    [Fact]
    public void BeheerderIsAuthorizedForOtherCapacitiesForTheirOrganisation()
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
    public void BeheerderIsNotAuthorizedForRegelgevingDbOwnedCapacitiesForTheirOrganisation()
    {
        var ovoNumber = _fixture.Create<string>();
        var user = new UserBuilder()
            .AddRoles(Role.DecentraalBeheerder)
            .AddOrganisations(ovoNumber)
            .Build();

        var authorizationResult =
            CreatePolicy(ovoNumber, _regelgevingDbCapacityId)
                .Check(user);

        authorizationResult.ShouldFailWith<InsufficientRights>();
    }

    [Fact]
    public void BeheerderIsNotAuthorizedForOtherCapacitiesForOtherOrganisations()
    {
        var user = new UserBuilder()
            .AddRoles(Role.DecentraalBeheerder)
            .AddOrganisations(_fixture.Create<string>())
            .Build();

        var authorizationResult =
            CreatePolicy(_fixture.Create<string>(), _fixture.Create<Guid>())
                .Check(user);

        authorizationResult.ShouldFailWith<InsufficientRights>();
    }
}