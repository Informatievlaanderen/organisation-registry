namespace OrganisationRegistry.UnitTests.Import.Organisations;

using System.Collections.Generic;
using Api.HostedServices;
using SqlServer.Organisation;

public class FakeImportCache : ImportCache
{
    private FakeImportCache(IEnumerable<OrganisationListItem> organisations) : base(organisations)
    {
    }

    public static ImportCache Create()
        => new FakeImportCache(new List<OrganisationListItem>());

    public static ImportCache Create(IEnumerable<OrganisationListItem> organisations)
        => new FakeImportCache(organisations);
}
