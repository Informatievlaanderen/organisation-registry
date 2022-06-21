namespace OrganisationRegistry.UnitTests.Import.Organisations;

using System;
using System.Collections.Generic;
using Api.HostedServices.ProcessImportedFiles.Strategy.CreateOrganisations;
using SqlServer.Organisation;

public class FakeImportCache : ImportCache
{
    private FakeImportCache(
        IEnumerable<OrganisationListItem> organisations,
        Dictionary<string, (Guid id, string name)> labelTypes) : base(organisations, labelTypes)
    {
    }

    public static ImportCache Create()
        => new FakeImportCache(new List<OrganisationListItem>(), new Dictionary<string, (Guid id, string name)>());

    public static ImportCache Create(IEnumerable<OrganisationListItem> organisations)
        => new FakeImportCache(organisations, new Dictionary<string, (Guid id, string name)>());
}
