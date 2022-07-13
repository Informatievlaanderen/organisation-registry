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

public class OrganisationClassificationTypePolicyTests
{
    private readonly Fixture _fixture;
    private readonly Guid _regelgevingDbClassificationTypeId;
    private readonly OrganisationRegistryConfigurationStub _configuration;
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

    public OrganisationClassificationTypePolicy CreatePolicy(string ovoNumber, Guid organisationClassificationTypeId)
        => new(ovoNumber, _configuration, organisationClassificationTypeId);

    [Theory]
    [InlineData(Role.RegelgevingBeheerder)]
    [InlineData(Role.AlgemeenBeheerder)]
    public void RegelgevingDbBeheerderAndAdminIsAuthorizedForRegelgeving(Role role)
    {
        var user = new UserBuilder()
            .AddRoles(role)
            .Build();

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
        var user = new UserBuilder()
            .AddRoles(role)
            .Build();

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
        var user = new UserBuilder()
            .AddRoles(role)
            .Build();

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
        var user = new UserBuilder()
            .AddRoles(role)
            .Build();

        var authorizationResult =
            CreatePolicy(_fixture.Create<string>(), _cjmClassificationTypeId)
                .Check(user);

        authorizationResult.ShouldFailWith<InsufficientRights<OrganisationClassificationTypePolicy>>();
    }

    [Fact]
    public void CjmClientIsNotAuthorizedForNonCjmClassificationTypes()
    {
        var ovoNumber = _fixture.Create<string>();
        var user = new UserBuilder()
            .AddRoles(Role.CjmBeheerder)
            .AddOrganisations(ovoNumber)
            .Build();

        var authorizationResult =
            CreatePolicy(ovoNumber, _fixture.Create<Guid>())
                .Check(user);

        authorizationResult.ShouldFailWith<InsufficientRights<OrganisationClassificationTypePolicy>>();
    }

    [Fact]
    public void BeheerderIsAuthorizedForOtherOrganisationClassificationTypesForTheirOrganisation()
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
    public void BeheerderIsNotAuthorizedForRegelgevingDbOwnedOrganisationClassificationTypesForTheirOrganisation()
    {
        var ovoNumber = _fixture.Create<string>();
        var user = new UserBuilder()
            .AddRoles(Role.DecentraalBeheerder)
            .AddOrganisations(ovoNumber)
            .Build();

        var authorizationResult =
            CreatePolicy(ovoNumber, _regelgevingDbClassificationTypeId)
                .Check(user);

        authorizationResult.ShouldFailWith<InsufficientRights<OrganisationClassificationTypePolicy>>();
    }

    [Fact]
    public void BeheerderIsNotAuthorizedForOtherOrganisationClassificationTypesForOtherOrganisations()
    {
        var user = new UserBuilder()
            .AddRoles(Role.DecentraalBeheerder)
            .AddOrganisations(_fixture.Create<string>())
            .Build();

        var authorizationResult =
            CreatePolicy(_fixture.Create<string>(), _fixture.Create<Guid>())
                .Check(user);

        authorizationResult.ShouldFailWith<InsufficientRights<OrganisationClassificationTypePolicy>>();
    }
}
