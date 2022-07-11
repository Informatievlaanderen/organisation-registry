namespace OrganisationRegistry.OrganisationClassificationType;

using Events;
using Infrastructure.Domain;

public class OrganisationClassificationType : AggregateRoot
{
    public OrganisationClassificationTypeName Name { get; private set; }
    public bool AllowDifferentClassificationsToOverlap { get; private set; }

    private OrganisationClassificationType()
    {
        Name = new OrganisationClassificationTypeName(string.Empty);
        AllowDifferentClassificationsToOverlap = false;
    }

    private OrganisationClassificationType(
        OrganisationClassificationTypeId id,
        OrganisationClassificationTypeName name) : this()
    {
        ApplyChange(new OrganisationClassificationTypeCreated(id, name));
    }

    public void Update(OrganisationClassificationTypeName name)
    {
        var @event = new OrganisationClassificationTypeUpdated(Id, name, Name);
        ApplyChange(@event);
    }

    public void ChangeAllowDifferentClassificationsToOverlap(bool isAllowed)
    {
        if (AllowDifferentClassificationsToOverlap == isAllowed) return;
        ApplyChange(OrganisationClassificationTypeAllowDifferentClassificationsToOverlapChanged.Create(Id, isAllowed));
    }

    private void Apply(OrganisationClassificationTypeCreated @event)
    {
        Id = @event.OrganisationClassificationTypeId;
        Name = new OrganisationClassificationTypeName(@event.Name);
    }

    private void Apply(OrganisationClassificationTypeUpdated @event)
    {
        Name = new OrganisationClassificationTypeName(@event.Name);
    }

    private void Apply(OrganisationClassificationTypeAllowDifferentClassificationsToOverlapChanged @event)
    {
        AllowDifferentClassificationsToOverlap = @event.IsAllowed;
    }

    public static OrganisationClassificationType Create(OrganisationClassificationTypeId id, OrganisationClassificationTypeName name, bool allowDifferentClassificationsToOverlap)
    {
        var organisationClassificationType = new OrganisationClassificationType(id, name);
        organisationClassificationType.ChangeAllowDifferentClassificationsToOverlap(allowDifferentClassificationsToOverlap);
        return organisationClassificationType;
    }
}
