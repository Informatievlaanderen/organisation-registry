namespace OrganisationRegistry.UnitTests
{
    using System;
    using System.Security.Claims;
    using Api.Configuration;
    using Api.Task;
    using FluentAssertions;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Options;
    using Moq;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Infrastructure.Domain.Exception;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using SqlServer.Infrastructure;
    using SqlServer.KboSyncQueue;
    using SqlServer.Organisation;
    using Xunit;

    public class KboSyncTests
    {
        private readonly DateTimeProviderStub _dateTimeProviderStub;
        private readonly OptionsWrapper<ApiConfiguration> _apiConfiguration;
        private readonly OrganisationRegistryContext _context;
        private readonly ClaimsPrincipal _claimsPrincipal;

        public KboSyncTests()
        {
            _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);
            _apiConfiguration = new OptionsWrapper<ApiConfiguration>(new ApiConfiguration {SyncFromKboBatchSize = 500});
            _context = new OrganisationRegistryContext(
                new DbContextOptionsBuilder<OrganisationRegistryContext>()
                    .UseInMemoryDatabase(
                        "kbo-sync-test-" + Guid.NewGuid(),
                        builder => { }).Options);
            _claimsPrincipal = new ClaimsPrincipal();
        }

        [Fact]
        public void ProcessesSuccess()
        {
            var commandSender = Mock.Of<ICommandSender>();

            var kboSyncQueueItem = AddOrganisationToSync(_context, "0123456789", DateTimeOffset.Now.AddDays(-1));
            var kboSyncQueueItem2 = AddOrganisationToSync(_context, "0998798798", DateTimeOffset.Now.AddDays(-2));

            _context.SaveChanges();

            new KboSync(_dateTimeProviderStub, _apiConfiguration).SyncFromKbo(commandSender, _context, _claimsPrincipal);

            _context.KboSyncQueue.Should().BeEquivalentTo(
                new KboSyncQueueItem
                {
                    Id = kboSyncQueueItem.Id,
                    SourceName = kboSyncQueueItem.SourceName,
                    MutationReadAt = kboSyncQueueItem.MutationReadAt,
                    SourceAddressModifiedAt = kboSyncQueueItem.SourceAddressModifiedAt,
                    SourceFileName = kboSyncQueueItem.SourceFileName,
                    SourceKboNumber = kboSyncQueueItem.SourceKboNumber,
                    SourceModifiedAt = kboSyncQueueItem.SourceModifiedAt,
                    SyncCompletedAt = _dateTimeProviderStub.UtcNow,
                    SyncStatus = KboSync.SyncStatusSuccess
                },
                new KboSyncQueueItem
                {
                    Id = kboSyncQueueItem2.Id,
                    SourceName = kboSyncQueueItem2.SourceName,
                    MutationReadAt = kboSyncQueueItem2.MutationReadAt,
                    SourceAddressModifiedAt = kboSyncQueueItem2.SourceAddressModifiedAt,
                    SourceFileName = kboSyncQueueItem2.SourceFileName,
                    SourceKboNumber = kboSyncQueueItem2.SourceKboNumber,
                    SourceModifiedAt = kboSyncQueueItem2.SourceModifiedAt,
                    SyncCompletedAt = _dateTimeProviderStub.UtcNow,
                    SyncStatus = KboSync.SyncStatusSuccess
                });
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
                .Setup(sender => sender.Send(It.IsAny<UpdateFromKbo>()))
                .Throws(aggregateNotFoundException);

            _context.SaveChanges();

            new KboSync(_dateTimeProviderStub, _apiConfiguration).SyncFromKbo(commandSender.Object, _context,
                _claimsPrincipal);

            _context.KboSyncQueue.Should().BeEquivalentTo(
                new KboSyncQueueItem
                {
                    Id = kboSyncQueueItem.Id,
                    SourceName = kboSyncQueueItem.SourceName,
                    MutationReadAt = kboSyncQueueItem.MutationReadAt,
                    SourceAddressModifiedAt = kboSyncQueueItem.SourceAddressModifiedAt,
                    SourceFileName = kboSyncQueueItem.SourceFileName,
                    SourceKboNumber = kboSyncQueueItem.SourceKboNumber,
                    SourceModifiedAt = kboSyncQueueItem.SourceModifiedAt,
                    SyncCompletedAt = null,
                    SyncStatus = KboSync.SyncStatusError,
                    SyncInfo = aggregateNotFoundException.ToString()
                });
        }

        [Fact]
        public void ProcessesNotFounds()
        {
            var commandSender = Mock.Of<ICommandSender>();

            var kboSyncQueueItem = new KboSyncQueueItem
            {
                Id = Guid.NewGuid(),
                SourceName = "test",
                MutationReadAt = DateTimeOffset.UtcNow,
                SourceAddressModifiedAt = DateTimeOffset.Now,
                SourceFileName = "test-file.fake",
                SourceKboNumber = "0123456789",
                SourceModifiedAt = DateTimeOffset.Now,
                SyncCompletedAt = null,
                SyncStatus = null,
            };

            _context.KboSyncQueue.Add(kboSyncQueueItem);

            _context.SaveChanges();

            new KboSync(_dateTimeProviderStub, _apiConfiguration).SyncFromKbo(commandSender, _context,
                _claimsPrincipal);

            _context.KboSyncQueue.Should().BeEquivalentTo(
                new KboSyncQueueItem
                {
                    Id = kboSyncQueueItem.Id,
                    SourceName = kboSyncQueueItem.SourceName,
                    MutationReadAt = kboSyncQueueItem.MutationReadAt,
                    SourceAddressModifiedAt = kboSyncQueueItem.SourceAddressModifiedAt,
                    SourceFileName = kboSyncQueueItem.SourceFileName,
                    SourceKboNumber = kboSyncQueueItem.SourceKboNumber,
                    SourceModifiedAt = kboSyncQueueItem.SourceModifiedAt,
                    SyncCompletedAt = null,
                    SyncStatus = KboSync.SyncStatusNotFound,
                    SyncInfo = KboSync.SyncInfoNotFound
                });
        }

        private static KboSyncQueueItem AddOrganisationToSync(OrganisationRegistryContext context, string kboNumber, DateTimeOffset mutationReadAt)
        {
            var kboSyncQueueItem = new KboSyncQueueItem
            {
                Id = Guid.NewGuid(),
                SourceName = "test",
                MutationReadAt = mutationReadAt,
                SourceAddressModifiedAt = DateTimeOffset.Now,
                SourceFileName = "test-file.fake",
                SourceKboNumber = kboNumber,
                SourceModifiedAt = DateTimeOffset.Now,
                SyncCompletedAt = null,
                SyncStatus = null,
            };

            context.KboSyncQueue.Add(kboSyncQueueItem);

            var organisationItem = new OrganisationDetailItem
            {
                Id = Guid.NewGuid(),
                KboNumber = kboSyncQueueItem.SourceKboNumber,
            };

            context.OrganisationDetail.Add(organisationItem);
            return kboSyncQueueItem;
        }
    }
}
