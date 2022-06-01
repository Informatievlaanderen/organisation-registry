namespace OrganisationRegistry.Tests.Shared.TestDataBuilders;

using System;
using Body.Events;

public class BodyOrganisationAddedBuilder
{
    public Guid BodyId { get; }
    public Guid BodyOrganisationId { get; }
    public string BodyName { get; }
    public Guid OrganisationId { get; }
    public string OrganisationName { get; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    public BodyOrganisationAddedBuilder(Guid bodyId, Guid organisationId)
    {
        BodyOrganisationId = Guid.NewGuid();
        BodyId = bodyId;
        BodyName = bodyId.ToString();
        OrganisationId = organisationId;
        OrganisationName = organisationId.ToString();
        ValidFrom = null;
        ValidTo = null;
    }

    public BodyOrganisationAdded Build()
        => new(
            BodyId,
            BodyOrganisationId,
            BodyName,
            OrganisationId,
            OrganisationName,
            ValidFrom,
            ValidTo);

    public static implicit operator BodyOrganisationAdded(BodyOrganisationAddedBuilder builder)
        => builder.Build();
}
