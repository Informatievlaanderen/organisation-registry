namespace OrganisationRegistry.Organisation;

using System;
using System.Collections.Generic;
using System.Linq;

public class OrganisationContacts : List<OrganisationContact>
{
    public OrganisationContacts()
    {
    }

    public OrganisationContacts(IEnumerable<OrganisationContact> organisationBuildings) : base(organisationBuildings)
    {
    }

    public OrganisationContacts(params OrganisationContact[] organisationBuildings) : base(organisationBuildings)
    {
    }

    public OrganisationContact this[Guid id]
        => this.Single(oc => oc.OrganisationContactId == id);

    public bool HasDuplicatContactOverlappingWith(OrganisationContact contact)
        => Except(contact.Id)
            .WithContactType(contact.ContactTypeId)
            .WithValue(contact.Value)
            .OverlappingWith(contact.Validity)
            .Any();

    private OrganisationContacts OverlappingWith(Period validity)
        => new(
            this.Where(ob => ob.Validity.OverlapsWith(validity)));


    private OrganisationContacts WithValue(string contactValue)
        => new(this.Where(oc => oc.Value == contactValue));

    private OrganisationContacts WithContactType(Guid contactTypeId)
        => new(this.Where(oc => oc.ContactTypeId == contactTypeId));

    private OrganisationContacts Except(Guid organisationContactId)
        => new(this.Where(oc => oc.OrganisationContactId != organisationContactId));
}
