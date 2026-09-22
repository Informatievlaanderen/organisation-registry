namespace OrganisationRegistry.UnitTests.Organisation.CreateOrganisation;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Handling.Authorization;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Domain;
using Purpose;
using Tests.Shared;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared.Stubs;
using Xunit;
using Xunit.Abstractions;

/// <summary>
/// Creating a <em>top-level</em> organisation (no parent) is gated by
/// <see cref="Permission.CanCreateOrganisations"/>, which is unrestricted-only
/// and granted solely to AlgemeenBeheerder (and Developer). Unlike adding a
/// daughter organisation (gated by <see cref="ChildPolicy"/> against the
/// parent), there is no existing organisation to scope a restriction
/// against, so CjmBeheerder/VlimpersBeheerder/DecentraalBeheerder can never
/// create a top-level organisation, regardless of Vlimpers management or
/// own-organisation membership.
/// </summary>
public class WithoutSufficientPermissions : Specification<CreateOrganisationCommandHandler, CreateOrganisation>
{
    public WithoutSufficientPermissions(ITestOutputHelper helper) : base(helper)
    {
    }

    private static IEvent[] Events
        => Array.Empty<IEvent>();

    private static CreateOrganisation CreateOrganisationCommand
        => new(
            new OrganisationId(Guid.NewGuid()),
            "Test",
            "OVO0001234",
            "",
            Article.None,
            null,
            "",
            new List<PurposeId>(),
            false,
            new ValidFrom(),
            new ValidTo(),
            new ValidFrom(),
            new ValidTo());

    protected override CreateOrganisationCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<CreateOrganisationCommandHandler>>().Object,
            session,
            new SequentialOvoNumberGenerator(),
            new UniqueOvoNumberValidatorStub(false),
            new DateTimeProviderStub(DateTime.Today));

    [Theory]
    [InlineData(Role.CjmBeheerder)]
    [InlineData(Role.VlimpersBeheerder)]
    [InlineData(Role.DecentraalBeheerder)]
    [InlineData(Role.RegelgevingBeheerder)]
    [InlineData(Role.OrgaanBeheerder)]
    public async Task ThrowsAnException(Role role)
    {
        var user = new UserBuilder().AddRoles(role).Build();

        await Given(Events).When(CreateOrganisationCommand, user)
            .ThenThrows<InsufficientRights<RequiresPermissionPolicy>>();
    }

    [Theory]
    [InlineData(Role.CjmBeheerder)]
    [InlineData(Role.VlimpersBeheerder)]
    [InlineData(Role.DecentraalBeheerder)]
    public async Task PublishesNoEvents(Role role)
    {
        var user = new UserBuilder().AddRoles(role).Build();

        await Given(Events).When(CreateOrganisationCommand, user)
            .ThenItPublishesTheCorrectNumberOfEvents(0);
    }
}
