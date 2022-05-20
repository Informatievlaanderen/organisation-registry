namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationInfo;

using System;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.Organisation.Exceptions;
using Xunit;
using Xunit.Abstractions;

public class WhenTryingToUpdateATerminatedNonVlimpersOrgAsVlimpersUser : ExceptionOldSpecification2<UpdateOrganisationInfoLimitedToVlimpersCommandHandler, UpdateOrganisationInfoLimitedToVlimpers>
{
    private Guid _organisationId;

    public WhenTryingToUpdateATerminatedNonVlimpersOrgAsVlimpersUser(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override UpdateOrganisationInfoLimitedToVlimpersCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<UpdateOrganisationInfoLimitedToVlimpersCommandHandler>>().Object,
            Session,
            new DateTimeProviderStub(DateTime.Today));

    protected override IUser User
        => new UserBuilder()
            .AddRoles(Role.VlimpersBeheerder)
            .Build();

    protected override IEnumerable<IEvent> Given()
    {
        var fixture = new Fixture();
        var organisationCreatedBuilder = new OrganisationCreatedBuilder(new SequentialOvoNumberGenerator());

        _organisationId = organisationCreatedBuilder.Id;

        return new List<IEvent>
        {
            organisationCreatedBuilder
                .WithValidity(null, null)
                .Build(),
            new OrganisationBecameActive(organisationCreatedBuilder.Id),
            new OrganisationTerminatedV2(
                organisationCreatedBuilder.Id,
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<DateTime>(),
                new FieldsToTerminateV2(
                    null,
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>()),
                new KboFieldsToTerminateV2(
                    new Dictionary<Guid, DateTime>(),
                    new KeyValuePair<Guid, DateTime>?(),
                    new KeyValuePair<Guid, DateTime>?(),
                    new KeyValuePair<Guid, DateTime>?()
                ),
                fixture.Create<bool>(),
                fixture.Create<DateTime?>()
            ),
        };
    }

    protected override UpdateOrganisationInfoLimitedToVlimpers When()
        => new(
            new OrganisationId(_organisationId),
            "Test",
            Article.None,
            "testing",
            new ValidFrom(),
            new ValidTo(),
            new ValidFrom(),
            new ValidTo());

    protected override int ExpectedNumberOfEvents
        => 0;

    [Fact]
    public void UpdatesOrganisationName()
    {
        Exception.Should().BeOfType<InsufficientRights>();
    }
}
