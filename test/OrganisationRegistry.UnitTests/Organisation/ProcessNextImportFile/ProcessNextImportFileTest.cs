// namespace OrganisationRegistry.UnitTests.Organisation.ProcessNextImportFile;
//
// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using FluentAssertions;
// using Microsoft.Extensions.Logging;
// using Moq;
// using OrganisationRegistry.Infrastructure.Domain;
// using OrganisationRegistry.Infrastructure.Events;
// using OrganisationRegistry.Organisation;
// using OrganisationRegistry.Organisation.Events;
// using OrganisationRegistry.Purpose;
// using OrganisationRegistry.Tests.Shared;
// using OrganisationRegistry.UnitTests.Infrastructure.Tests.Extensions.TestHelpers;
// using Xunit;
// using Xunit.Abstractions;
//
// public class ProcessNextImportFileTest : Specification<CreateOrganisationsFromImportCommandHandler, CreateOrganisationsFromImport>
// {
//     public ProcessNextImportFileTest(ITestOutputHelper helper) : base(helper)
//     {
//     }
//
//     private static IEvent[] Events
//         => Array.Empty<IEvent>();
//
//     private static CreateOrganisationsFromImport CreateOrganisationsFromImportCommand
//         => new CreateOrganisationsFromImport(Guid.NewGuid());
//
//     protected override CreateOrganisationsFromImportCommandHandler BuildHandler(ISession session)
//         => new(
//             new Mock<ILogger<CreateOrganisationsFromImportCommandHandler>>().Object,
//             session);
//
//     [Fact]
//     public async Task PublishesOneEvent()
//     {
//         await Given(Events).When(CreateOrganisationsFromImportCommand, TestUser.AlgemeenBeheerder).ThenItPublishesTheCorrectNumberOfEvents(1);
//     }
//
//     [Fact]
//     public async Task PublishesTheEvent()
//     {
//         await Given(Events).When(CreateOrganisationsFromImportCommand, TestUser.AlgemeenBeheerder).Then();
//
//         PublishedEvents.First().Should().BeOfType<OrganisationsCreatedFromImport>();
//     }
// }
//
