namespace OrganisationRegistry.UnitTests.Organisation.CreateOrganisationFromImport;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Import;
using Tests.Shared;
using OrganisationRegistry.UnitTests.Infrastructure.Tests.Extensions.TestHelpers;
using Xunit;
using Xunit.Abstractions;

public class WithoutRecords : Specification<CreateOrganisationsFromImportCommandHandler, CreateOrganisationsFromImport>
{
    public WithoutRecords(ITestOutputHelper helper) : base(helper)
    {
    }

    private static CreateOrganisationsFromImport CreateOrganisationsFromImportCommand
        => new(Guid.NewGuid(), Array.Empty<OutputRecord>());

    protected override CreateOrganisationsFromImportCommandHandler BuildHandler(ISession session)
        => new(
            Mock.Of<ILogger<CreateOrganisationsFromImportCommandHandler>>(),
            new SequentialOvoNumberGenerator(),
            new DateTimeProviderStub(DateTime.Now),
            session);

    [Fact]
    public async Task WhenNoRecords_ThrowsException()
    {
        await Given().When(CreateOrganisationsFromImportCommand, TestUser.AlgemeenBeheerder)
            .ThenThrows<AtLeastOneOrganisationMustHaveAnExistingOrganisationAsParent>();
    }
}
