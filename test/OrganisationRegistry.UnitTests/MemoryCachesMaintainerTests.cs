namespace OrganisationRegistry.UnitTests;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using SqlServer;
using SqlServer.Infrastructure;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using Xunit;

public class MemoryCachesMaintainerTests
{
    private static OrganisationRegistryContext NewContext()
        => new(
            new DbContextOptionsBuilder<OrganisationRegistryContext>()
                .UseInMemoryDatabase("memcaches-" + Guid.NewGuid())
                .Options);

    private class TestContextFactory : IContextFactory
    {
        private readonly OrganisationRegistryContext _context;
        public TestContextFactory(OrganisationRegistryContext context) => _context = context;
        public OrganisationRegistryContext Create() => _context;
        public OrganisationRegistryContext CreateTransactional(System.Data.Common.DbConnection _, System.Data.Common.DbTransaction __) => _context;
    }

    [Fact]
    public async Task OrganisationCreated_AddsToOrganisationParentsCacheWithNullParent()
    {
        await using var context = NewContext();
        var factory = new TestContextFactory(context);
        var memoryCaches = new MemoryCaches(factory);
        var maintainer = new MemoryCachesMaintainer(memoryCaches, factory);

        var created = new OrganisationCreatedBuilder(new SequentialOvoNumberGenerator())
            .WithOvoNumber("OVO000123")
            .Build();

        await maintainer.Handle(null!, null!, created.ToTypedEnvelope());

        memoryCaches.OrganisationParents.Should().ContainKey(created.OrganisationId);
        memoryCaches.OrganisationParents[created.OrganisationId].Should().BeNull();
        memoryCaches.OvoNumbers[created.OrganisationId].Should().Be("OVO000123");
    }

    [Fact]
    public async Task ChildAssignedToNewlyCreatedTopLevelParent_DoesNotThrow()
    {
        // Reproduces the staging bug: a top-level parent was created after the
        // last MemoryCaches reset. When a child is later assigned to it, the
        // OrganisationTreeView must not blow up because the parent is missing
        // from the OrganisationParents cache.
        await using var context = NewContext();
        var factory = new TestContextFactory(context);
        var memoryCaches = new MemoryCaches(factory);
        var maintainer = new MemoryCachesMaintainer(memoryCaches, factory);

        var parent = new OrganisationCreatedBuilder(new SequentialOvoNumberGenerator())
            .WithOvoNumber("OVO057176")
            .Build();

        await maintainer.Handle(null!, null!, parent.ToTypedEnvelope());

        var child = new OrganisationCreatedBuilder(new SequentialOvoNumberGenerator())
            .WithOvoNumber("OVO057177")
            .Build();

        await maintainer.Handle(null!, null!, child.ToTypedEnvelope());

        var parentAssigned = new ParentAssignedToOrganisation(child.OrganisationId, parent.OrganisationId, Guid.NewGuid());
        Func<Task> act = () => maintainer.Handle(null!, null!, parentAssigned.ToTypedEnvelope());

        await act.Should().NotThrowAsync();
        memoryCaches.OrganisationParents[child.OrganisationId].Should().Be(parent.OrganisationId);
    }

    [Fact]
    public async Task OrganisationCreatedFromKbo_AddsToOrganisationParentsCacheWithNullParent()
    {
        await using var context = NewContext();
        var factory = new TestContextFactory(context);
        var memoryCaches = new MemoryCaches(factory);
        var maintainer = new MemoryCachesMaintainer(memoryCaches, factory);

        var organisationId = Guid.NewGuid();
        var created = new OrganisationCreatedFromKbo(
            organisationId,
            "0123456789",
            "Some KBO Org",
            "OVO000999",
            "shortname",
            Article.None,
            null,
            new List<Purpose>(),
            false,
            null,
            null,
            null,
            null);

        await maintainer.Handle(null!, null!, created.ToTypedEnvelope());

        memoryCaches.OrganisationParents.Should().ContainKey(organisationId);
        memoryCaches.OrganisationParents[organisationId].Should().BeNull();
    }
}
