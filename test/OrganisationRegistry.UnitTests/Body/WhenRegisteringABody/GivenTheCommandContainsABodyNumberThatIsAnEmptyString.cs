namespace OrganisationRegistry.UnitTests.Body.WhenRegisteringABody;

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using LifecyclePhaseType;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using OrganisationRegistry.Body;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.Infrastructure.Domain;
using Xunit;

public class GivenTheCommandContainsABodyNumberThatIsAnEmptyString
{
    [Fact]
    public async Task ThenANewBodyNumberIsGenerated()
    {
        var fixture = new Fixture();
        var activeLifecyclePhaseTypeId = new LifecyclePhaseTypeId(fixture.Create<Guid>());
        var activeLifecyclePhaseType = new LifecyclePhaseType(
            activeLifecyclePhaseTypeId,
            new LifecyclePhaseTypeName(fixture.Create<string>()),
            LifecyclePhaseTypeIsRepresentativeFor.ActivePhase,
            LifecyclePhaseTypeStatus.Default);
        var inActiveLifecyclePhaseTypeId = new LifecyclePhaseTypeId(fixture.Create<Guid>());
        var inActiveLifecyclePhaseType = new LifecyclePhaseType(
            inActiveLifecyclePhaseTypeId,
            new LifecyclePhaseTypeName(fixture.Create<string>()),
            LifecyclePhaseTypeIsRepresentativeFor.InactivePhase,
            LifecyclePhaseTypeStatus.Default);

        var command = new RegisterBody(
            new BodyId(fixture.Create<Guid>()),
            fixture.Create<string>(),
            string.Empty,
            null,
            null,
            null,
            new Period(),
            new Period(),
            activeLifecyclePhaseTypeId,
            inActiveLifecyclePhaseTypeId);

        var fixedBodyNumberGenerator = new FixedBodyNumberGenerator(fixture.Create<string>());
        var mockSession = new MockSession(activeLifecyclePhaseType, inActiveLifecyclePhaseType);

        var handler = new RegisterBodyCommandHandler(
            NullLogger<RegisterBodyCommandHandler>.Instance,
            mockSession,
            fixedBodyNumberGenerator,
            new FakeUniqueBodyNumberValidator());

        await handler.Handle(new CommandEnvelope<RegisterBody>(command, Mock.Of<IUser>()));

        mockSession.AddedAggregate.Should().NotBeNull();
        var aggregate = mockSession.AddedAggregate!;
        aggregate.Should().BeOfType<Body>();
        var body = (Body)aggregate;
        var bodyType = typeof(Body);
        var fieldInfo = bodyType.GetField("_bodyNumber", BindingFlags.Instance | BindingFlags.NonPublic);
        var fieldValue = fieldInfo!.GetValue(body);

        fieldValue.Should().Be(fixedBodyNumberGenerator.BodyNumber);
    }

    private class MockSession : ISession
    {
        private readonly Dictionary<Guid, LifecyclePhaseType> _lifecyclePhaseTypes = new();

        public MockSession(LifecyclePhaseType activeLifecyclePhaseType, LifecyclePhaseType inActiveLifecyclePhaseType)
        {
            _lifecyclePhaseTypes.Add(activeLifecyclePhaseType.Id, activeLifecyclePhaseType);
            _lifecyclePhaseTypes.Add(inActiveLifecyclePhaseType.Id, inActiveLifecyclePhaseType);
        }

        public AggregateRoot? AddedAggregate { get; private set; }

        public void Add<T>(T aggregate) where T : AggregateRoot
        {
            AddedAggregate = aggregate;
        }

        public T Get<T>(Guid id, int? expectedVersion = null) where T : AggregateRoot
        {
            return typeof(T).Name switch
            {
                nameof(LifecyclePhaseType) => (_lifecyclePhaseTypes[id] as T)!,
                nameof(Body) => (AddedAggregate as T)!,
                _ => null!
            };
        }

        public Task Commit(IUser user)
            => Task.CompletedTask;
    }

    private class FixedBodyNumberGenerator : IBodyNumberGenerator
    {
        public string BodyNumber { get; }

        public FixedBodyNumberGenerator(string bodyNumber)
            => BodyNumber = bodyNumber;

        public string GenerateNumber()
            => BodyNumber;
    }

    private class FakeUniqueBodyNumberValidator : IUniqueBodyNumberValidator
    {
        private readonly bool _isTaken;

        public FakeUniqueBodyNumberValidator(bool isTaken)
            => _isTaken = isTaken;

        public FakeUniqueBodyNumberValidator() : this(false)
        {
        }

        public bool IsBodyNumberTaken(string ovoNumber)
            => _isTaken;

        public bool IsBodyNumberTaken(Guid id, string ovoNumber)
            => _isTaken;
    }
}
