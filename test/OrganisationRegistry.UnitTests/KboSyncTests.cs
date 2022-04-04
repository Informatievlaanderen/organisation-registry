namespace OrganisationRegistry.UnitTests
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using Api.Backoffice.Admin.Task;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging.Abstractions;
    using Microsoft.Extensions.Options;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Infrastructure.Configuration;
    using OrganisationRegistry.Infrastructure.Domain.Exception;
    using OrganisationRegistry.Organisation.Commands;
    using SqlServer.Infrastructure;
    using SqlServer.KboSyncQueue;
    using SqlServer.Organisation;
    using Xunit;

    public class KboSyncTests
    {
        private readonly DateTimeProviderStub _dateTimeProviderStub;
        private readonly OptionsWrapper<ApiConfigurationSection> _apiConfiguration;
        private readonly OrganisationRegistryContext _context;
        private readonly ClaimsPrincipal _claimsPrincipal;

        public KboSyncTests()
        {
            _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);
            _apiConfiguration = new OptionsWrapper<ApiConfigurationSection>(new ApiConfigurationSection { SyncFromKboBatchSize = 500 });
            _context = new OrganisationRegistryContext(
                new DbContextOptionsBuilder<OrganisationRegistryContext>()
                    .UseInMemoryDatabase(
                        "kbo-sync-test-" + Guid.NewGuid(),
                        _ => { })
                    .Options);
            _claimsPrincipal = new ClaimsPrincipal();
        }

        [Fact]
        public void ProcessesSuccess()
        {
            var commandSender = Mock.Of<ICommandSender>();

            var kboSyncQueueItem = AddOrganisationToSync(_context, "0123456789", DateTimeOffset.Now.AddDays(-1));
            var kboSyncQueueItem2 = AddOrganisationToSync(_context, "0998798798", DateTimeOffset.Now.AddDays(-2));

            _context.SaveChanges();

            new KboSync(_dateTimeProviderStub, _apiConfiguration, Mock.Of<ISecurityService>(), new NullLogger<KboSync>()).SyncFromKbo(commandSender, _context, _claimsPrincipal);

            _context.KboSyncQueue.AsEnumerable().Should().BeEquivalentTo(
                new List<KboSyncQueueItem>
                {
                    new()
                    {
                        Id = kboSyncQueueItem.Id,
                        SourceOrganisationName = kboSyncQueueItem.SourceOrganisationName,
                        MutationReadAt = kboSyncQueueItem.MutationReadAt,
                        SourceFileName = kboSyncQueueItem.SourceFileName,
                        SourceOrganisationKboNumber = kboSyncQueueItem.SourceOrganisationKboNumber,
                        SourceOrganisationModifiedAt = kboSyncQueueItem.SourceOrganisationModifiedAt,
                        SyncCompletedAt = _dateTimeProviderStub.UtcNow,
                        SyncStatus = KboSync.SyncStatusSuccess,
                        SyncInfo = string.Empty,
                        SourceOrganisationStatus = kboSyncQueueItem.SourceOrganisationStatus,
                    },
                    new()
                    {
                        Id = kboSyncQueueItem2.Id,
                        SourceOrganisationName = kboSyncQueueItem2.SourceOrganisationName,
                        MutationReadAt = kboSyncQueueItem2.MutationReadAt,
                        SourceFileName = kboSyncQueueItem2.SourceFileName,
                        SourceOrganisationKboNumber = kboSyncQueueItem2.SourceOrganisationKboNumber,
                        SourceOrganisationModifiedAt = kboSyncQueueItem2.SourceOrganisationModifiedAt,
                        SyncCompletedAt = _dateTimeProviderStub.UtcNow,
                        SyncStatus = KboSync.SyncStatusSuccess,
                        SyncInfo = string.Empty,
                        SourceOrganisationStatus = kboSyncQueueItem.SourceOrganisationStatus,
                    }
                }.AsEnumerable());
        }

        [Fact]
        public void ProcessesErrors()
        {
            var commandSender = new Mock<ICommandSender>();

            var kboSyncQueueItem = AddOrganisationToSync(_context, "0123456789", DateTimeOffset.Now.AddDays(-1));

            var aggregateNotFoundException = new AggregateNotFoundException(
                typeof(OrganisationRegistry.Organisation.Organisation),
                kboSyncQueueItem.Id);

            commandSender
                .Setup(sender => sender.Send(It.IsAny<SyncOrganisationWithKbo>()))
                .Throws(aggregateNotFoundException);

            _context.SaveChanges();

            new KboSync(_dateTimeProviderStub, _apiConfiguration, Mock.Of<ISecurityService>(), new NullLogger<KboSync>()).SyncFromKbo(
                commandSender.Object,
                _context,
                _claimsPrincipal);

            _context.KboSyncQueue.AsEnumerable().Should().BeEquivalentTo(
                new List<KboSyncQueueItem>
                {
                    new()
                    {
                        Id = kboSyncQueueItem.Id,
                        SourceOrganisationName = kboSyncQueueItem.SourceOrganisationName,
                        MutationReadAt = kboSyncQueueItem.MutationReadAt,
                        SourceFileName = kboSyncQueueItem.SourceFileName,
                        SourceOrganisationKboNumber = kboSyncQueueItem.SourceOrganisationKboNumber,
                        SourceOrganisationModifiedAt = kboSyncQueueItem.SourceOrganisationModifiedAt,
                        SyncCompletedAt = null,
                        SyncStatus = KboSync.SyncStatusError,
                        SyncInfo = aggregateNotFoundException.ToString(),
                        SourceOrganisationStatus = kboSyncQueueItem.SourceOrganisationStatus,
                    }
                });
        }

        [Fact]
        public void ProcessesNotFounds()
        {
            var commandSender = Mock.Of<ICommandSender>();

            var kboSyncQueueItem = new KboSyncQueueItem
            {
                Id = Guid.NewGuid(),
                SourceOrganisationName = "test",
                MutationReadAt = DateTimeOffset.UtcNow,
                SourceFileName = "test-file.fake",
                SourceOrganisationKboNumber = "0123456789",
                SourceOrganisationModifiedAt = DateTimeOffset.Now,
                SyncCompletedAt = null,
                SyncStatus = null,
                SourceOrganisationStatus = "test",
            };

            _context.KboSyncQueue.Add(kboSyncQueueItem);

            _context.SaveChanges();

            new KboSync(_dateTimeProviderStub, _apiConfiguration, Mock.Of<ISecurityService>(), new NullLogger<KboSync>()).SyncFromKbo(
                commandSender,
                _context,
                _claimsPrincipal);

            _context.KboSyncQueue.AsEnumerable().Should().BeEquivalentTo(
                new List<KboSyncQueueItem>
                {
                    new()
                    {
                        Id = kboSyncQueueItem.Id,
                        SourceOrganisationName = kboSyncQueueItem.SourceOrganisationName,
                        MutationReadAt = kboSyncQueueItem.MutationReadAt,
                        SourceFileName = kboSyncQueueItem.SourceFileName,
                        SourceOrganisationKboNumber = kboSyncQueueItem.SourceOrganisationKboNumber,
                        SourceOrganisationModifiedAt = kboSyncQueueItem.SourceOrganisationModifiedAt,
                        SyncCompletedAt = null,
                        SyncStatus = KboSync.SyncStatusNotFound,
                        SyncInfo = KboSync.SyncInfoNotFound,
                        SourceOrganisationStatus = kboSyncQueueItem.SourceOrganisationStatus,
                    }
                });
        }

        private static KboSyncQueueItem AddOrganisationToSync(OrganisationRegistryContext context, string kboNumber, DateTimeOffset mutationReadAt)
        {
            var kboSyncQueueItem = new KboSyncQueueItem
            {
                Id = Guid.NewGuid(),
                SourceOrganisationName = "test",
                MutationReadAt = mutationReadAt,
                SourceFileName = "test-file.fake",
                SourceOrganisationKboNumber = kboNumber,
                SourceOrganisationModifiedAt = DateTimeOffset.Now,
                SyncCompletedAt = null,
                SyncStatus = null,
                SourceOrganisationStatus = "test",
            };

            context.KboSyncQueue.Add(kboSyncQueueItem);

            var organisationItem = new OrganisationDetailItem
            {
                Id = Guid.NewGuid(),
                KboNumber = kboSyncQueueItem.SourceOrganisationKboNumber,
                Name = "test",
                OvoNumber = "test",
            };

            context.OrganisationDetail.Add(organisationItem);
            return kboSyncQueueItem;
        }
    }
}
