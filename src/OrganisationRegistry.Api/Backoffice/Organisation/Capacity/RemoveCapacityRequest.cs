namespace OrganisationRegistry.Api.Backoffice.Organisation.Capacity;

using System;
using OrganisationRegistry.Organisation;

public class RemoveOrganisationCapacityRequest
{
    public Guid OrganisationId { get; set; }
    public Guid OrganisationCapacityId { get; set; }

    public RemoveOrganisationCapacityRequest(Guid organisationId, Guid organisationCapacityId)
    {
        OrganisationId = organisationId;
        OrganisationCapacityId = organisationCapacityId;
    }
}

public static class RemoveOrganisationCapacityRequestMapping
{
    public static RemoveOrganisationCapacity Map(RemoveOrganisationCapacityRequest message)
        => new(new OrganisationId(message.OrganisationId), new OrganisationCapacityId(message.OrganisationCapacityId));
}
