namespace OrganisationRegistry.Organisation;

using System;

public class RemoveOrganisationFunction : BaseCommand<OrganisationId>
{
    public Guid OrganisationFunctionId { get; }

    public RemoveOrganisationFunction(
        OrganisationId organisationId,
        Guid organisationFunctionId)
    {
        Id = organisationId;
        OrganisationFunctionId = organisationFunctionId;
    }
}
