namespace OrganisationRegistry.Organisation;

using System;
using ContactType;

public class UpdateOrganisationContact : BaseCommand<OrganisationId>
{
    public OrganisationId OrganisationId => Id;

    public Guid OrganisationContactId { get; }
    public ContactTypeId ContactTypeId { get; }
    public string Value { get; }
    public ValidFrom ValidFrom { get; }
    public ValidTo ValidTo { get; }

    public UpdateOrganisationContact(
        Guid organisationContactId,
        OrganisationId organisationId,
        ContactTypeId contactTypeId,
        string value,
        ValidFrom validFrom,
        ValidTo validTo)
    {
        Id = organisationId;

        OrganisationContactId = organisationContactId;
        ContactTypeId = contactTypeId;
        Value = value;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}