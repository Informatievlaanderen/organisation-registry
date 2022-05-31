namespace OrganisationRegistry.Organisation;

using System;
using OrganisationClassification;
using OrganisationClassificationType;

public class UpdateOrganisationOrganisationClassification : BaseCommand<OrganisationId>
{
    public OrganisationId OrganisationId => Id;

    public Guid OrganisationOrganisationClassificationId { get; }
    public OrganisationClassificationTypeId OrganisationClassificationTypeId { get; }
    public OrganisationClassificationId OrganisationClassificationId { get; }
    public ValidFrom ValidFrom { get; }
    public ValidTo ValidTo { get; }

    public UpdateOrganisationOrganisationClassification(
        Guid organisationOrganisationClassificationId,
        OrganisationId organisationId,
        OrganisationClassificationTypeId organisationClassificationTypeId,
        OrganisationClassificationId organisationClassificationId,
        ValidFrom validFrom,
        ValidTo validTo)
    {
        Id = organisationId;

        OrganisationOrganisationClassificationId = organisationOrganisationClassificationId;
        OrganisationClassificationTypeId = organisationClassificationTypeId;
        OrganisationClassificationId = organisationClassificationId;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}
