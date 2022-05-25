namespace OrganisationRegistry.UnitTests.Organisation.CreateOrganisation;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using Purpose;
using Tests.Shared;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Xunit;
using Xunit.Abstractions;

public class WithAnInactiveValidity : Specification<CreateOrganisationCommandHandler, CreateOrganisation>
{
    private readonly DateTimeProviderStub _dateTimeProviderStub;
    private readonly DateTime _yesterday;

    public WithAnInactiveValidity(ITestOutputHelper helper) : base(helper)
    {
        _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Today);
        _yesterday = _dateTimeProviderStub.Yesterday;
    }

    private static IEvent[] Events
        => Array.Empty<IEvent>();

    private CreateOrganisation CreateOrganisationCommand
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
            new ValidFrom(_yesterday),
            new ValidTo(_yesterday),
            new ValidFrom(),
            new ValidTo());

    protected override CreateOrganisationCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<CreateOrganisationCommandHandler>>().Object,
            session,
            new SequentialOvoNumberGenerator(),
            new UniqueOvoNumberValidatorStub(false),
            _dateTimeProviderStub);


    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(Events).When(CreateOrganisationCommand, UserBuilder.AlgemeenBeheerder())
            .ThenItPublishesTheCorrectNumberOfEvents(1);
    }

    [Fact]
    public async Task CreatesAnOrganisation()
    {
        await Given(Events).When(CreateOrganisationCommand, UserBuilder.AlgemeenBeheerder()).Then();
        PublishedEvents[0]
            .UnwrapBody<OrganisationCreated>()
            .Should().NotBeNull();
    }
}
