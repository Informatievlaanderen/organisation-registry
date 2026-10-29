namespace OrganisationRegistry.UnitTests.FunctionTypes.RemoveFunctionType;

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Function;
using OrganisationRegistry.Function.Commands;
using OrganisationRegistry.Function.Events;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Handling.Authorization;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation.Exceptions;
using OrganisationRegistry.UnitTests.Infrastructure.Tests.Extensions.TestHelpers;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class WhenRemovingAFunctionTypeWithoutPermission : Specification<FunctionTypeCommandHandlers, RemoveFunctionType>
{
    private readonly Guid _functionTypeId;
    private const string FunctionTypeName = "Functie A";

    public WhenRemovingAFunctionTypeWithoutPermission(ITestOutputHelper helper) : base(helper)
    {
        _functionTypeId = Guid.NewGuid();
    }

    private static IUser UserWithoutPermission
        => new UserBuilder().Build();

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
    public async Task ThrowsException()
    {
        await Given(Events)
            .When(RemoveFunctionTypeCommand, UserWithoutPermission)
            .ThenThrows<InsufficientRights<RequiresPermissionPolicy>>();
    }
}
