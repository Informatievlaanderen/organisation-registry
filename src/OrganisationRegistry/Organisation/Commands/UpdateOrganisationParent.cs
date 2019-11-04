namespace OrganisationRegistry.Organisation.Commands
{
    using System;

    public class UpdateOrganisationParent : BaseCommand<OrganisationId>
    {
        public OrganisationId OrganisationId => Id;

        public Guid OrganisationOrganisationParentId { get; }
        public OrganisationId ParentOrganisationId { get; }
        public ValidFrom ValidFrom { get; }
        public ValidTo ValidTo { get; }

        public UpdateOrganisationParent(
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
}
