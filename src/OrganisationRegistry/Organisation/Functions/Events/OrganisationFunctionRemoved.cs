namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationFunctionRemoved : BaseEvent<OrganisationFunctionRemoved>
{
    public Guid OrganisationId
        => Id;

    public Guid OrganisationFunctionId { get; }
    public Guid PersonId { get; }

    public OrganisationFunctionRemoved(
        Guid organisationId,
        Guid organisationFunctionId,
        Guid personId)
    {
        Id = organisationId;

        OrganisationFunctionId = organisationFunctionId;
        PersonId = personId;
    }
}
