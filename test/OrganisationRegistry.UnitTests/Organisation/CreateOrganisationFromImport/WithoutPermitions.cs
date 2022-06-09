namespace OrganisationRegistry.UnitTests.Organisation.CreateOrganisationFromImport;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Exceptions;
using OrganisationRegistry.Organisation.Import;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class
    WithoutPermitions : Specification<CreateOrganisationsFromImportCommandHandler, CreateOrganisationsFromImport>
{
    public WithoutPermitions(ITestOutputHelper helper) : base(helper)
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

    public static IEnumerable<object[]> ThrowsInsufficientRightsException_Data()
    {
        yield return new object[] { TestUser.User };
        yield return new object[] { TestUser.DecentraalBeheerder };
        yield return new object[] { TestUser.OrgaanBeheerder };
        yield return new object[] { TestUser.RegelgevingBeheerder };
        yield return new object[] { TestUser.VlimpersBeheerder };
    }

    [Theory]
    [MemberData(nameof(ThrowsInsufficientRightsException_Data))]
    public async Task ThrowsInsufficientRightsException(User user)
    {
        await Given().When(CreateOrganisationsFromImportCommand, user)
            .ThenThrows<InsufficientRights>();
    }
}
