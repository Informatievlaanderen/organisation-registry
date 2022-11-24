namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationParent;

using System;
using System.Threading.Tasks;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Domain;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared.Stubs;
using Xunit;
using Xunit.Abstractions;

public class WhenUpdatingAnOrganisationParentWithCircularDependenciesInDaughters
    : Specification<UpdateOrganisationParentCommandHandler, UpdateOrganisationParent>
{
    private readonly DateTimeProviderStub _dateTimeProviderStub;
    private readonly SequentialOvoNumberGenerator _sequentialOvoNumberGenerator;

    private readonly string _ovoNumberA;
    private readonly Guid _organisationOrganisationParentId;
    private readonly Guid _organisationAId;
    private readonly Guid _organisationBId;
    private readonly Guid _organisationCId;

    public WhenUpdatingAnOrganisationParentWithCircularDependenciesInDaughters(ITestOutputHelper helper) : base(
        helper)
    {
        _dateTimeProviderStub = new DateTimeProviderStub(new DateTime(2016, 6, 1));
        _sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();

        _organisationAId = Guid.NewGuid();
        _organisationBId = Guid.NewGuid();
        _organisationCId = Guid.NewGuid();
        _ovoNumberA = _sequentialOvoNumberGenerator.GenerateNumber();
        Guid.NewGuid();
        _organisationOrganisationParentId = Guid.NewGuid();
    }

    protected override UpdateOrganisationParentCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateOrganisationParentCommandHandler>>().Object,
            session,
            _dateTimeProviderStub);

    private IUser User
        => new UserBuilder()
            .AddOrganisations(_ovoNumberA)
            .AddRoles(Role.DecentraalBeheerder)
            .Build();

    private IEvent[] Events
        => new IEvent[]
        {
            new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator)
                .WithOvoNumber(_ovoNumberA)
                .WithId(new OrganisationId(_organisationAId))
                .Build(),
            new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator)
                .WithId(new OrganisationId(_organisationBId))
                .Build(),
            new OrganisationParentAddedBuilder(
                    _organisationAId,
                    _organisationBId)
                .WithValidity(new DateTime(2016, 1, 1), new DateTime(2016, 12, 31)).Build(),
            new OrganisationParentAddedBuilder(
                    _organisationBId,
                    _organisationCId)
                .WithValidity(new DateTime(2017, 1, 1), new DateTime(2017, 12, 31)).Build(),
            new OrganisationParentAddedBuilder(
                    _organisationAId,
                    _organisationBId)
                .WithOrganisationOrganisationParentId(_organisationOrganisationParentId)
                .WithValidity(new DateTime(2018, 1, 1), new DateTime(2018, 12, 31)).Build(),
        };

    private UpdateOrganisationParent UpdateOrganisationParentCommand
        => new(
            _organisationOrganisationParentId,
            new OrganisationId(_organisationAId),
            new OrganisationId(_organisationBId),
            new ValidFrom(new DateTime(2016, 1, 1)),
            new ValidTo(new DateTime(2016, 12, 31)));

    [Fact]
    public async Task PublishesNoEvents()
    {
        await Given(Events).When(UpdateOrganisationParentCommand, User).ThenItPublishesTheCorrectNumberOfEvents(0);
    }

    [Fact]
    public async Task ThrowsADomainException()
    {
        await Given(Events).When(UpdateOrganisationParentCommand, User)
            .ThenThrows<OrganisationAlreadyCoupledToParentInThisPeriod>();
    }
}
