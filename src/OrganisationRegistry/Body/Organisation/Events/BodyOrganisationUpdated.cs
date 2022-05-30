namespace OrganisationRegistry.Body.Events;

using System;

public class BodyOrganisationUpdated : BaseEvent<BodyOrganisationUpdated>
{
    public Guid BodyId => Id;

    public Guid BodyOrganisationId { get; }
    public Guid OrganisationId { get; }
    public Guid PreviousOrganisationId { get; }
    public string OrganisationName { get; }
    public string PreviousOrganisationName { get; }

    public DateTime? ValidFrom { get; }
    public DateTime? PreviouslyValidFrom { get; }
    public DateTime? ValidTo { get; }
    public DateTime? PreviouslyValidTo { get; }

    public BodyOrganisationUpdated(
        Guid bodyId,
        Guid bodyOrganisationId,
        Guid organisationId,
        string organisationName,
        DateTime? validFrom,
        DateTime? validTo,
        Guid previousOrganisationId,
        string previousOrganisationName,
        DateTime? previouslyValidFrom,
        DateTime? previouslyValidTo)
    {
        Id = bodyId;

        BodyOrganisationId = bodyOrganisationId;
        OrganisationId = organisationId;
        OrganisationName = organisationName;
        ValidFrom = validFrom;
        ValidTo = validTo;

        PreviousOrganisationId = previousOrganisationId;
        PreviousOrganisationName = previousOrganisationName;
        PreviouslyValidFrom = previouslyValidFrom;
        PreviouslyValidTo = previouslyValidTo;

    }
}