namespace OrganisationRegistry.UnitTests.SecurityPolicy;

using System;
using System.Collections.Generic;
using FluentAssertions;
using Handling.Authorization;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Authorization.Restrictions;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared;
using Tests.Shared.Stubs;
using Xunit;

/// <summary>
/// ImportPolicy mirrors ChildPolicy: it holds no role-specific logic of its own, it
/// only evaluates Permission.CanImport (with its restriction, if any) against the
/// target organisation's OVO number and Vlimpers-management flag.
/// </summary>
public class ImportPolicyTests
{
    private static Organisation CreateOrganisation(bool underVlimpersManagement)
    {
        var organisation = Organisation.Create(
            new OrganisationId(Guid.NewGuid()),
            "Test organisatie",
            "OVO000001",
            null,
            Article.Het,
            null,
            null,
            new List<Purpose.Purpose>(),
            false,
            new Period(new ValidFrom(null), new ValidTo(null)),
            new Period(new ValidFrom(null), new ValidTo(null)),
            new DateTimeProviderStub(DateTime.Now));

        if (underVlimpersManagement)
            organisation.PlaceUnderVlimpersManagement();

        return organisation;
    }

    private static ImportPolicy CreatePolicy(Organisation organisation)
    {
        var sessionMock = new Mock<ISession>();
        sessionMock
            .Setup(session => session.Get<Organisation>(organisation.Id, null))
            .Returns(organisation);

        return new ImportPolicy(sessionMock.Object, organisation.Id);
    }

    [Theory]
    [InlineData(Role.AlgemeenBeheerder)]
    [InlineData(Role.Developer)]
    public void AdminIsAuthorized_RegardlessOfVlimpersManagement(Role role)
    {
        var organisation = CreateOrganisation(underVlimpersManagement: false);
        var user = new UserBuilder().AddRoles(role).Build();

        var authorizationResult = CreatePolicy(organisation).Check(user);

        authorizationResult.Should().Be(AuthorizationResult.Success());
    }

    [Fact]
    public void VlimpersBeheerderIsAuthorized_WhenOrganisationIsUnderVlimpersManagement()
    {
        var organisation = CreateOrganisation(underVlimpersManagement: true);
        var user = new UserBuilder()
            .AddRoles(Role.VlimpersBeheerder)
            .WithPermissions(
                PermissionSet.Of(
                    Permission.CanImport.RestrictedTo(ChildRestrictions.UnderVlimpersManagement)))
            .Build();

        var authorizationResult = CreatePolicy(organisation).Check(user);

        authorizationResult.Should().Be(AuthorizationResult.Success());
    }

    [Fact]
    public void VlimpersBeheerderIsNotAuthorized_WhenOrganisationIsNotUnderVlimpersManagement()
    {
        var organisation = CreateOrganisation(underVlimpersManagement: false);
        var user = new UserBuilder()
            .AddRoles(Role.VlimpersBeheerder)
            .WithPermissions(
                PermissionSet.Of(
                    Permission.CanImport.RestrictedTo(ChildRestrictions.UnderVlimpersManagement)))
            .Build();

        var authorizationResult = CreatePolicy(organisation).Check(user);

        authorizationResult.ShouldFailWith<InsufficientRights<ImportPolicy>>();
    }

    [Theory]
    [InlineData(Role.DecentraalBeheerder)]
    [InlineData(Role.RegelgevingBeheerder)]
    [InlineData(Role.CjmBeheerder)]
    [InlineData(Role.Orafin)]
    [InlineData(Role.OrgaanBeheerder)]
    public void NonAdminNonVlimpersIsNotAuthorized(Role role)
    {
        // None of these roles hold Permission.CanImport (unrestricted or restricted),
        // so the policy must fail regardless of the organisation's Vlimpers state.
        var organisation = CreateOrganisation(underVlimpersManagement: true);
        var user = new UserBuilder().AddRoles(role).Build();

        var authorizationResult = CreatePolicy(organisation).Check(user);

        authorizationResult.ShouldFailWith<InsufficientRights<ImportPolicy>>();
    }
}
