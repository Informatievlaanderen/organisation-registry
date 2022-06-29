namespace OrganisationRegistry.UnitTests.SecurityPolicy;

using System;
using AutoFixture;
using FluentAssertions;
using Handling.Authorization;
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared;
using Tests.Shared.Stubs;
using Xunit;

public class LabelPolicyTests
{
    private const string VlimpersLabelIdValue = "7FE6F62E-3EC1-4F30-A1E2-72731E53E5A1";

    private readonly Fixture _fixture;
    private readonly OrganisationRegistryConfigurationStub _configuration;
    private readonly Guid _vlimpersLabelId;

    public LabelPolicyTests()
    {
        _fixture = new Fixture();
        _vlimpersLabelId = new Guid(VlimpersLabelIdValue);
        _configuration = new OrganisationRegistryConfigurationStub
        {
            Authorization = new AuthorizationConfigurationStub
            {
                LabelIdsAllowedForVlimpers = new[] { _vlimpersLabelId },
            },
        };
    }

    [Fact]
    public void AlgemeenBeheerderCanUpdateAllLabels()
    {
        var policy = LabelPolicy.ForUpdate(
            _fixture.Create<string>(),
            _fixture.Create<bool>(),
            _configuration,
            _vlimpersLabelId,
            Guid.NewGuid()
        );

        policy.Check(TestUser.AlgemeenBeheerder)
            .Should().BeEquivalentTo(AuthorizationResult.Success());
    }

    [Fact]
    public void VlimpersBeheerderCanUpdateVlimpersLabel()
    {
        var policy = LabelPolicy.ForUpdate(
            _fixture.Create<string>(),
            _fixture.Create<bool>(),
            _configuration,
            _vlimpersLabelId,
            _vlimpersLabelId
        );

        policy.Check(TestUser.VlimpersBeheerder)
            .Should().BeEquivalentTo(AuthorizationResult.Success());
    }

    [Theory]
    [InlineData("9D534B91-E3C5-4D45-8A88-A36C65B35A3D", "82E8DCBB-F78E-4D45-BE57-D8E92783B0AD")]
    [InlineData(VlimpersLabelIdValue, "1F79C899-4848-499D-AA21-4BB00132F810")]
    [InlineData("D842F234-8DFF-4FBD-8FBA-D589C095D782", VlimpersLabelIdValue)]
    public void VlimpersBeheerderCannotUpdateOtherLabel(string oldLabelTypeIdValue, string newLabelTypeIdValue)
    {
        var policy = LabelPolicy.ForUpdate(
            _fixture.Create<string>(),
            _fixture.Create<bool>(),
            _configuration,
            new Guid(oldLabelTypeIdValue),
            new Guid(newLabelTypeIdValue)
        );

        policy.Check(TestUser.VlimpersBeheerder)
            .ShouldFailWith<InsufficientRights<LabelPolicy>>();
    }

    [Fact]
    public void DecentraalBeheerderCannotUpdateLabelsOfOtherOrganisation()
    {
        var ovoNumber = _fixture.Create<string>();
        var otherOvoNumber = _fixture.Create<string>();

        var user = new UserBuilder().AsDecentraalBeheerder().AddOrganisations(ovoNumber).Build();
        var policy = LabelPolicy.ForUpdate(
            otherOvoNumber,
            _fixture.Create<bool>(),
            _configuration,
            _fixture.Create<Guid>(),
            _fixture.Create<Guid>());

        policy.Check(user).ShouldFailWith<InsufficientRights<LabelPolicy>>();
    }

    [Theory]
    [InlineData(VlimpersLabelIdValue, VlimpersLabelIdValue)]
    [InlineData(VlimpersLabelIdValue, "1F79C899-4848-499D-AA21-4BB00132F810")]
    [InlineData("D842F234-8DFF-4FBD-8FBA-D589C095D782", VlimpersLabelIdValue)]
    public void DecentraalBeheerderCannotUpdateVlimpersLabelsForVlimpersOrganisation(string oldLabelTypeIdValue, string newLabelTypeIdValue)
    {
        var ovoNumber = _fixture.Create<string>();
        var user = new UserBuilder().AsDecentraalBeheerder().AddOrganisations(ovoNumber).Build();
        var policy = LabelPolicy.ForUpdate(
            ovoNumber,
            isUnderVlimpersManagement: true,
            _configuration,
            new Guid(oldLabelTypeIdValue),
            new Guid(newLabelTypeIdValue));

        policy.Check(user).ShouldFailWith<InsufficientRights<LabelPolicy>>();
    }

    [Fact]
    public void DecentraalBeheerderCanUpdateLabelsForNotVlimpersOrganisation()
    {
        var ovoNumber = _fixture.Create<string>();
        var user = new UserBuilder().AsDecentraalBeheerder().AddOrganisations(ovoNumber).Build();
        var policy = LabelPolicy.ForUpdate(
            ovoNumber,
            isUnderVlimpersManagement: false,
            _configuration,
            _fixture.Create<Guid>(),
            _fixture.Create<Guid>());

        policy.Check(user).Should().BeEquivalentTo(AuthorizationResult.Success());
    }

    [Fact]
    public void DecentraalBeheerderCanUpdateNonVlimperLabelsForVlimperOrganisation()
    {
        var ovoNumber = _fixture.Create<string>();
        var user = new UserBuilder().AsDecentraalBeheerder().AddOrganisations(ovoNumber).Build();
        var policy = LabelPolicy.ForUpdate(
            ovoNumber,
            isUnderVlimpersManagement: true,
            _configuration,
            _fixture.Create<Guid>(),
            _fixture.Create<Guid>());

        policy.Check(user).Should().BeEquivalentTo(AuthorizationResult.Success());
    }
}
