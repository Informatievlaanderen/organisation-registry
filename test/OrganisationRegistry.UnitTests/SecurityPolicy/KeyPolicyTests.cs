namespace OrganisationRegistry.UnitTests.SecurityPolicy
{
    using System;
    using AutoFixture;
    using FluentAssertions;
    using Handling.Authorization;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Organisation.Exceptions;
    using Tests.Shared.Stubs;
    using Xunit;

    public class KeyPolicyTests
    {
        private readonly Fixture _fixture;
        private readonly Guid _orafinKeyId;
        private readonly Guid _vlimpersKeyId;
        private readonly OrganisationRegistryConfigurationStub _configuration;

        public KeyPolicyTests()
        {
            _fixture = new Fixture();

            _vlimpersKeyId = _fixture.Create<Guid>();
            _orafinKeyId = _fixture.Create<Guid>();
            _configuration = new OrganisationRegistryConfigurationStub
            {
                Authorization = new AuthorizationConfigurationStub
                {
                    KeyIdsAllowedForVlimpers = new[] { _vlimpersKeyId },
                    KeyIdsAllowedOnlyForOrafin = new[] { _orafinKeyId }
                }
            };
        }

        [Theory]
        [InlineData(Role.Orafin)]
        [InlineData(Role.OrganisationRegistryBeheerder)]
        public void OrafinBeheerderAndAdminIsAuthorizedForOrafinKey(Role role)
        {
            var user = new UserBuilder()
                .AddRoles(role)
                .Build();

            var authorizationResult =
                new KeyPolicy(
                        _fixture.Create<string>(),
                        _configuration,
                        _orafinKeyId)
                    .Check(user);

            authorizationResult.Should().Be(AuthorizationResult.Success());
        }

        [Theory]
        [InlineData(Role.OrganisatieBeheerder)]
        [InlineData(Role.VlimpersBeheerder)]
        public void NonOrafinBeheerderIsNotAuthorizedForOrafinKey(Role role)
        {
            var user = new UserBuilder()
                .AddRoles(role)
                .Build();

            var authorizationResult =
                new KeyPolicy(
                        _fixture.Create<string>(),
                        _configuration,
                        _orafinKeyId)
                    .Check(user);

            authorizationResult.ShouldFailWith<InsufficientRights>();
        }

        [Theory]
        [InlineData(Role.VlimpersBeheerder)]
        [InlineData(Role.OrganisationRegistryBeheerder)]
        public void VlimpersBeheerderAndAdminIsAuthorizedForVlimpersKey(Role role)
        {
            var user = new UserBuilder()
                .AddRoles(role)
                .Build();

            var authorizationResult =
                new KeyPolicy(
                        _fixture.Create<string>(),
                        _configuration,
                        _vlimpersKeyId)
                    .Check(user);

            authorizationResult.Should().Be(AuthorizationResult.Success());
        }

        [Theory]
        [InlineData(Role.Orafin)]
        [InlineData(Role.OrganisatieBeheerder)]
        [InlineData(Role.OrgaanBeheerder)]
        public void NonVlimpersBeheerderIsNotAuthorizedForVlimpersKey(Role role)
        {
            var user = new UserBuilder()
                .AddRoles(role)
                .Build();

            var authorizationResult =
                new KeyPolicy(
                        _fixture.Create<string>(),
                        _configuration,
                        _vlimpersKeyId)
                    .Check(user);

            authorizationResult.ShouldFailWith<InsufficientRights>();
        }

        [Fact]
        public void BeheerderIsAuthorizedForOtherKeysForTheirOrganisation()
        {
            var ovoNumber = _fixture.Create<string>();
            var user = new UserBuilder()
                .AddRoles(Role.OrganisatieBeheerder)
                .AddOrganisations(ovoNumber)
                .Build();

            var authorizationResult =
                new KeyPolicy(
                        ovoNumber,
                        _configuration,
                        _fixture.Create<Guid>())
                    .Check(user);

            authorizationResult.Should().Be(AuthorizationResult.Success());
        }

        [Fact]
        public void BeheerderIsNotAuthorizedForOtherKeysForOtherOrganisation()
        {
            var user = new UserBuilder()
                .AddRoles(Role.OrganisatieBeheerder)
                .AddOrganisations(_fixture.Create<string>())
                .Build();

            var authorizationResult =
                new KeyPolicy(
                        _fixture.Create<string>(),
                        _configuration,
                        _fixture.Create<Guid>())
                    .Check(user);

            authorizationResult.ShouldFailWith<InsufficientRights>();
        }

        [Fact]
        public void OrgaanBeheerderIsNotAuthorizedForOtherKeys()
        {
            var user = new UserBuilder()
                .AddRoles(Role.OrgaanBeheerder)
                .Build();

            var authorizationResult =
                new KeyPolicy(
                        _fixture.Create<string>(),
                        _configuration,
                        _fixture.Create<Guid>())
                    .Check(user);

            authorizationResult.ShouldFailWith<InsufficientRights>();
        }
    }
}
