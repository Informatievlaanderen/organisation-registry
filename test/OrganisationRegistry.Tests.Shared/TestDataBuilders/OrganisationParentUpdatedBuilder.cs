namespace OrganisationRegistry.Tests.Shared.TestDataBuilders;

using System;
using Organisation.Events;

public class OrganisationParentUpdatedBuilder
{
    public Guid OrganisationId { get; }
    public Guid OrganisationOrganisationParentId { get; }
    public Guid ParentOrganisationId { get; }
    public string ParentOrganisationName { get; }
    public DateTime? ValidFrom { get; private set; }
    public DateTime? ValidTo { get; private set; }

    public Guid PreviousParentOrganisationId { get; set; }
    public string PreviousParentOrganisationName { get; set; }

    public OrganisationParentUpdatedBuilder(Guid organisationOrganisationParentId, Guid organisationId, Guid parentOrganisationId)
    {
        OrganisationOrganisationParentId = organisationOrganisationParentId;
        OrganisationId = organisationId;
        ParentOrganisationId = parentOrganisationId;
        ParentOrganisationName = parentOrganisationId.ToString();
        PreviousParentOrganisationName = string.Empty;
        ValidFrom = null;
        ValidTo = null;
    }

    public OrganisationParentUpdatedBuilder WithValidity(DateTime? from, DateTime? to)
    {
        ValidFrom = from;
        ValidTo = to;
        return this;
    }

    public OrganisationParentUpdatedBuilder WithPreviousParent(Guid id)
    {
        PreviousParentOrganisationId = id;
        PreviousParentOrganisationName = id.ToString();
        return this;
    }

    public OrganisationParentUpdated Build()
        => new(
            OrganisationId,
            OrganisationOrganisationParentId,
            ParentOrganisationId,
            ParentOrganisationName,
            ValidFrom,
            ValidTo,
            PreviousParentOrganisationId, PreviousParentOrganisationName, ValidFrom, ValidTo);

    public static implicit operator OrganisationParentUpdated(OrganisationParentUpdatedBuilder builder)
        => builder.Build();
}
