namespace OrganisationRegistry.Organisation;

using System;
using System.Collections.Generic;
using System.Linq;

public class OrganisationList<TItem> : List<TItem>
    where TItem : IOrganisationField
{
    public OrganisationList()
    {
    }

    public OrganisationList(IEnumerable<TItem> items) : base(items)
    {
    }

    public OrganisationList(params TItem[] items) : base(items)
    {
    }

    public OrganisationList<TItem> Except(Guid itemId)
        => new(this.Where(item => item.Id != itemId));

    public OrganisationList<TItem> OverlappingWith(Period validity)
        => new(this.Where(item => item.Validity.OverlapsWith(validity)));

    public bool Exists(Guid id)
        => this.Any(item => item.Id == id);

    public TItem this[Guid id]
        => this.Single(item => item.Id == id);
}
