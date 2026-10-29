namespace OrganisationRegistry.UnitTests.FunctionTypes.RemoveFunctionType;

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Function;
using OrganisationRegistry.Function.Commands;
using OrganisationRegistry.Function.Events;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.UnitTests.Infrastructure.Tests.Extensions.TestHelpers;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class WhenRemovingAFunctionType : Specification<FunctionTypeCommandHandlers, RemoveFunctionType>
{
    private readonly Guid _functionTypeId;
    private const string FunctionTypeName = "Functie A";

    public WhenRemovingAFunctionType(ITestOutputHelper helper) : base(helper)
    {
        _functionTypeId = Guid.NewGuid();
    }

    private static IUser AlgemeenBeheerderUser
        => new UserBuilder()
            .AddRoles(Role.AlgemeenBeheerder)
            .Build();

    protected override FunctionTypeCommandHandlers BuildHandler(ISession session)
        => new(
            new Mock<ILogger<FunctionTypeCommandHandlers>>().Object,
            session,
            Mock.Of<IUniqueNameValidator<FunctionType>>()
        );

    private IEvent[] Events
        => new IEvent[]
        {
            new FunctionCreated(
                _functionTypeId,
                FunctionTypeName
            ),
        };

    private RemoveFunctionType RemoveFunctionTypeCommand
        => new(new FunctionTypeId(_functionTypeId));

    [Fact]
    public async Task AFunctionTypeRemovedEventIsPublished()
    {
        await Given(Events).When(RemoveFunctionTypeCommand, AlgemeenBeheerderUser).Then();
        PublishedEvents.First().Should().BeOfType<Envelope<FunctionTypeRemoved>>();
    }

    [Fact]
    public async Task TheEventContainsTheCorrectData()
    {
        await Given(Events).When(RemoveFunctionTypeCommand, AlgemeenBeheerderUser).Then();
        var functionTypeRemoved = PublishedEvents.First().UnwrapBody<FunctionTypeRemoved>();
        functionTypeRemoved.FunctionTypeId.Should().Be(_functionTypeId);
    }

    [Fact]
    public async Task PublishesTheCorrectNumberOfEvents()
        => await Given(Events)
            .When(RemoveFunctionTypeCommand, AlgemeenBeheerderUser)
            .ThenItPublishesTheCorrectNumberOfEvents(1);
}
