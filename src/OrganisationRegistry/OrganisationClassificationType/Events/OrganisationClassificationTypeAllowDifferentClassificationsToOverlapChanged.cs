namespace OrganisationRegistry.OrganisationClassificationType.Events;

using System;
using System.Text.Json.Serialization;

public class OrganisationClassificationTypeAllowDifferentClassificationsToOverlapChanged : BaseEvent<OrganisationClassificationTypeAllowDifferentClassificationsToOverlapChanged>
{
    public Guid OrganisationClassificationTypeId
        => Id;
    public bool IsAllowed { get; }

    [JsonConstructor]
    public OrganisationClassificationTypeAllowDifferentClassificationsToOverlapChanged(Guid organisationClassificationTypeId, bool isAllowed)
    {
        Id = organisationClassificationTypeId;
        IsAllowed = isAllowed;
    }

    public static OrganisationClassificationTypeAllowDifferentClassificationsToOverlapChanged Create(Guid organisationClassificationTypeId, bool isAllowed)
        => new(organisationClassificationTypeId, isAllowed);
}
