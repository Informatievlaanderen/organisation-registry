namespace OrganisationRegistry.Organisation.Events;

using System;

public class OrganisationBankAccountRemoved : BaseEvent<OrganisationBankAccountRemoved>
{
    public Guid OrganisationId
        => Id;

    public Guid OrganisationBankAccountId { get; }

    public OrganisationBankAccountRemoved(
        Guid organisationId,
        Guid organisationBankAccountId)
    {
        Id = organisationId;

        OrganisationBankAccountId = organisationBankAccountId;
    }
}
