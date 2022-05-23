namespace OrganisationRegistry.UnitTests.SecurityPolicy
{
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

        public OrganisationClassificationTypePolicyTests()
        {
            _fixture = new Fixture();

            _regelgevingDbClassificationTypeId = _fixture.Create<Guid>();
            _configuration = new OrganisationRegistryConfigurationStub
            {
                Authorization = new AuthorizationConfigurationStub
                {
                    OrganisationClassificationTypeIdsOwnedByRegelgevingDbBeheerder = new[] { _regelgevingDbClassificationTypeId },
                }
            };
        }

        public OrganisationClassificationTypePolicy CreatePolicy(string ovoNumber, Guid organisationClassificationTypeId)
            => new(ovoNumber, _configuration, organisationClassificationTypeId);

        [Theory]
        [InlineData(Role.RegelgevingBeheerder)]
        [InlineData(Role.AlgemeenBeheerder)]
        public void RegelgevingDbBeheerderAndAdminIsAuthorized(Role role)
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
        public void NonRegelgevingDbBeheerderIsNotAuthorized(Role role)
        {
            var user = new UserBuilder()
                .AddRoles(role)
                .Build();

            var authorizationResult =
                CreatePolicy(_fixture.Create<string>(), _regelgevingDbClassificationTypeId)
                    .Check(user);

            authorizationResult.ShouldFailWith<InsufficientRights>();
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

            authorizationResult.ShouldFailWith<InsufficientRights>();
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

            authorizationResult.ShouldFailWith<InsufficientRights>();
        }
    }
}
