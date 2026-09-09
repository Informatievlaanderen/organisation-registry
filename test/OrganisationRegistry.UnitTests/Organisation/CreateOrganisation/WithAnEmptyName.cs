namespace OrganisationRegistry.UnitTests.Organisation.CreateOrganisation;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Exceptions;
using Purpose;
using Tests.Shared;
using Tests.Shared.Stubs;
using Xunit;
using Xunit.Abstractions;

public class WithAnEmptyName : Specification<CreateOrganisationCommandHandler, CreateOrganisation>
{
    public WithAnEmptyName(ITestOutputHelper helper) : base(helper)
    {
    }

    private static IEvent[] Events
        => Array.Empty<IEvent>();

    private static CreateOrganisation CreateOrganisationCommandWithName(string name)
        => new(
            new OrganisationId(Guid.NewGuid()),
            name,
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

    [Fact]
    public async Task ThrowsWhenNameIsEmpty()
    {
        await Given(Events)
            .When(CreateOrganisationCommandWithName(string.Empty), TestUser.AlgemeenBeheerder)
            .ThenThrows<OrganisationNameIsRequired>();
    }

    [Fact]
    public async Task ThrowsWhenNameIsWhitespace()
    {
        await Given(Events)
            .When(CreateOrganisationCommandWithName("   "), TestUser.AlgemeenBeheerder)
            .ThenThrows<OrganisationNameIsRequired>();
    }
}
