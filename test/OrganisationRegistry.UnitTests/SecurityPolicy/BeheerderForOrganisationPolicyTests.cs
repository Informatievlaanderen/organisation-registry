namespace OrganisationRegistry.UnitTests.SecurityPolicy
{
    using System;
    using System.Linq;
    using AutoFixture;
    using FluentAssertions;
    using Handling.Authorization;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Organisation.Exceptions;
    using Tests.Shared.Stubs;
    using Xunit;

    public class BeheerderForOrganisationPolicyTests
    {
        private readonly Fixture _fixture;
        private readonly Guid _orafinKeyId;
        private readonly Guid _vlimpersKeyId;
        private readonly OrganisationRegistryConfigurationStub _configuration;

        public BeheerderForOrganisationPolicyTests()
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

        [Fact]
        public void OrganisatieBeheerderIsAlwaysAuthorizedIfNotVlimpers()
        {
            var ovoNumbers = _fixture.CreateMany<string>().ToArray();
            var user = new UserBuilder()
                .AddRoles(Role.OrganisatieBeheerder)
                .AddOrganisations(ovoNumbers)
                .Build();

            var authorizationResult =
                new BeheerderForOrganisationPolicy(
                        isUnderVlimpersManagement: false,
                        ovoNumber: ovoNumbers[_fixture.Create<int>() % ovoNumbers.Length],
                        allowOrganisationToBeUnderVlimpersManagement: _fixture.Create<bool>())
                    .Check(user);

            authorizationResult.Should().Be(AuthorizationResult.Success());
        }

        [Fact]
        public void OrganisatieBeheerderIsNotAuthorizedIfVlimpersAndVlimpersManagementIsNotAllowed()
        {
            var ovoNumbers = _fixture.CreateMany<string>().ToArray();
            var user = new UserBuilder()
                .AddRoles(Role.OrganisatieBeheerder)
                .AddOrganisations(ovoNumbers)
                .Build();

            var authorizationResult =
                new BeheerderForOrganisationPolicy(
                        isUnderVlimpersManagement: true,
                        ovoNumber: ovoNumbers[_fixture.Create<int>() % ovoNumbers.Length],
                        allowOrganisationToBeUnderVlimpersManagement: false)
                    .Check(user);

            authorizationResult.ShouldFailWith<InsufficientRights>();
        }

        [Fact]
        public void OrganisatieBeheerderIsAuthorizedIfVlimpersAndVlimpersManagementIsAllowed()
        {
            var ovoNumbers = _fixture.CreateMany<string>().ToArray();
            var user = new UserBuilder()
                .AddRoles(Role.OrganisatieBeheerder)
                .AddOrganisations(ovoNumbers)
                .Build();

            var authorizationResult =
                new BeheerderForOrganisationPolicy(
                        isUnderVlimpersManagement: true,
                        ovoNumber: ovoNumbers[_fixture.Create<int>() % ovoNumbers.Length],
                        allowOrganisationToBeUnderVlimpersManagement: true)
                    .Check(user);

            authorizationResult.Should().Be(AuthorizationResult.Success());
        }

        [Theory]
        [InlineData(Role.OrganisationRegistryBeheerder)]
        public void OrganisationRegistryBeheerderIsAuthorized(Role role, params string[] organisationOvos)
        {
            var user = new UserBuilder()
                .AddRoles(role)
                .AddOrganisations(organisationOvos)
                .Build();

            var authorizationResult =
                new BeheerderForOrganisationPolicy(
                        isUnderVlimpersManagement: false,
                        ovoNumber: "OVO000001",
                        allowOrganisationToBeUnderVlimpersManagement: _fixture.Create<bool>())
                    .Check(user);

            authorizationResult.Should().Be(AuthorizationResult.Success());
        }

        [Theory]
        [InlineData(Role.VlimpersBeheerder)]
        [InlineData(Role.Orafin)]
        public void VlimpersBeheerderAndOrafinIsNotAuthorizedIf(Role role, params string[] organisationOvos)
        {
            var user = new UserBuilder()
                .AddRoles(role)
                .AddOrganisations(organisationOvos)
                .Build();

            var authorizationResult =
                new BeheerderForOrganisationPolicy(
                        isUnderVlimpersManagement: false,
                        ovoNumber: "OVO000001",
                        allowOrganisationToBeUnderVlimpersManagement: _fixture.Create<bool>())
                    .Check(user);

            authorizationResult.ShouldFailWith<InsufficientRights>();
        }
    }
}
