namespace OrganisationRegistry.Organisation;

using System;

public class AddOrganisationParent : BaseCommand<OrganisationId>
{
    public OrganisationId OrganisationId => Id;

    public Guid OrganisationOrganisationParentId { get; }
    public OrganisationId ParentOrganisationId { get; }
    public ValidFrom ValidFrom { get; }
    public ValidTo ValidTo { get; }

    public AddOrganisationParent(
        Guid organisationOrganisationParentId,
        OrganisationId organisationId,
        OrganisationId parentOrganisationId,
        ValidFrom validFrom,
        ValidTo validTo)
    {
        Id = organisationId;

        OrganisationOrganisationParentId = organisationOrganisationParentId;
        ParentOrganisationId = parentOrganisationId;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}
