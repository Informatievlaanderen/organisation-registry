namespace OrganisationRegistry.UnitTests.Organisation.RemoveOrganisationBankAccount;

using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Handling.Authorization;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using Xunit;
using Xunit.Abstractions;

public class WhenRemovingAnOrganisationBankAccount
    : Specification<RemoveOrganisationBankAccountCommandHandler, RemoveOrganisationBankAccount>
{
    private readonly Fixture _fixture;
    private readonly Guid _organisationId;
    private readonly Guid _organisationBankAccountId;

    public WhenRemovingAnOrganisationBankAccount(ITestOutputHelper helper) : base(helper)
    {
        _fixture = new Fixture();
        _organisationId = _fixture.Create<Guid>();
        _organisationBankAccountId = _fixture.Create<Guid>();
    }

    protected override RemoveOrganisationBankAccountCommandHandler BuildHandler(ISession session)
        => new(Mock.Of<ILogger<RemoveOrganisationBankAccountCommandHandler>>(), session);

    private RemoveOrganisationBankAccount RemoveOrganisationBankAccountCommand
        => new(new OrganisationId(_organisationId), _organisationBankAccountId);

    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(
                OrganisationCreated,
                OrganisationBankAccountAdded)
            .When(RemoveOrganisationBankAccountCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(1);
    }

    [Fact]
    public async Task AnOrganisationBankAccountRemovedEventIsPublished()
    {
        await Given(
                OrganisationCreated,
                OrganisationBankAccountAdded)
            .When(RemoveOrganisationBankAccountCommand, TestUser.AlgemeenBeheerder)
            .Then();

        var evnt = PublishedEvents[0].UnwrapBody<OrganisationBankAccountRemoved>();
        evnt.OrganisationId.Should().Be(_organisationId);
        evnt.OrganisationBankAccountId.Should().Be(_organisationBankAccountId);
    }

    [Theory]
    [InlineData(Role.VlimpersBeheerder)]
    [InlineData(Role.Orafin)]
    [InlineData(Role.AutomatedTask)]
    [InlineData(Role.CjmBeheerder)]
    [InlineData(Role.DecentraalBeheerder)]
    [InlineData(Role.OrgaanBeheerder)]
    [InlineData(Role.RegelgevingBeheerder)]
    public async Task InsuficientRights_ThrowException(Role role)
    {
        await Given(
                OrganisationCreated,
                OrganisationBankAccountAdded)
            .When(RemoveOrganisationBankAccountCommand, new UserBuilder().AddRoles(role).Build())
            .ThenThrows<InsufficientRights<AdminOnlyPolicy>>();
    }

    [Theory]
    [InlineData(Role.VlimpersBeheerder)]
    [InlineData(Role.Orafin)]
    [InlineData(Role.AutomatedTask)]
    [InlineData(Role.CjmBeheerder)]
    [InlineData(Role.DecentraalBeheerder)]
    [InlineData(Role.OrgaanBeheerder)]
    [InlineData(Role.RegelgevingBeheerder)]
    public async Task InsuficientRights_PublishesNoEvents(Role role)
    {
        await Given(
                OrganisationCreated,
                OrganisationBankAccountAdded)
            .When(RemoveOrganisationBankAccountCommand, new UserBuilder().AddRoles(role).Build())
            .ThenItPublishesTheCorrectNumberOfEvents(0);
    }

    [Fact]
    public async Task DoesNotExist_ThrowsException()
    {
        await Given(
                OrganisationCreated)
            .When(RemoveOrganisationBankAccountCommand, TestUser.AlgemeenBeheerder)
            .ThenThrows<OrganisationBankAccountNotFound>();
    }

    [Fact]
    public async Task DoesNotExist_PublishesNoEvents()
    {
        await Given(
                OrganisationCreated)
            .When(RemoveOrganisationBankAccountCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(0);
    }

    private IEvent OrganisationBankAccountAdded
        => new OrganisationBankAccountAdded(
            _organisationId,
            _organisationBankAccountId,
            _fixture.Create<string>(),
            _fixture.Create<bool>(),
            _fixture.Create<string>(),
            _fixture.Create<bool>(),
            null,
            null
        );

    private OrganisationCreated OrganisationCreated
        => new OrganisationCreatedBuilder().WithId(_organisationId);
}
