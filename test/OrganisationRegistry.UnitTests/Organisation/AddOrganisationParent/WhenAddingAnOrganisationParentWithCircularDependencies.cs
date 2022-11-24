namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationParent;

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

public class WhenAddingAnOrganisationParentWithCircularDependencies
    : Specification<AddOrganisationParentCommandHandler, AddOrganisationParent>
{
    private readonly DateTimeProviderStub _dateTimeProviderStub;
    private readonly SequentialOvoNumberGenerator _sequentialOvoNumberGenerator;
    private readonly string _ovoNumberB;
    private readonly Guid _organisationAId;
    private readonly Guid _organisationBId;

    public WhenAddingAnOrganisationParentWithCircularDependencies(ITestOutputHelper helper) : base(helper)
    {
        _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);
        _sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();

        _ovoNumberB = _sequentialOvoNumberGenerator.GenerateNumber();
        _organisationAId = Guid.NewGuid();
        _organisationBId = Guid.NewGuid();
    }

    protected override AddOrganisationParentCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<AddOrganisationParentCommandHandler>>().Object,
            session,
            _dateTimeProviderStub);

    private IUser User
        => new UserBuilder()
            .AddOrganisations(_ovoNumberB)
            .AddRoles(Role.DecentraalBeheerder)
            .Build();

    private IEvent[] Events
        => new IEvent[]
        {
            new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator)
                .WithId(new OrganisationId(_organisationAId))
                .Build(),
            new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator)
                .WithId(new OrganisationId(_organisationBId))
                .WithOvoNumber(_ovoNumberB)
                .Build(),
            new OrganisationParentAddedBuilder(
                    _organisationAId,
                    _organisationBId)
                .Build(),
        };

    private AddOrganisationParent AddOrganisationParentCommand
        => new(
            Guid.NewGuid(),
            new OrganisationId(_organisationBId),
            new OrganisationId(_organisationAId),
            new ValidFrom(),
            new ValidTo());

    [Fact]
    public async Task ThrowsADomainException()
    {
        await Given(Events).When(AddOrganisationParentCommand, User).ThenThrows<CircularRelationshipDetected>();
    }

    [Fact]
    public async Task PublishesNoEvents()
    {
        await Given(Events).When(AddOrganisationParentCommand, User).ThenItPublishesTheCorrectNumberOfEvents(0);
    }
}
