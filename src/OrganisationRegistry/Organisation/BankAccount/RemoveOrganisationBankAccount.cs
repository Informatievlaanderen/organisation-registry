namespace OrganisationRegistry.Organisation;

using System;

public class RemoveOrganisationBankAccount : BaseCommand<OrganisationId>
{
    public Guid OrganisationBankAccountId { get; }

    public RemoveOrganisationBankAccount(
        OrganisationId organisationId,
        Guid organisationBankAccountId)
    {
        Id = organisationId;
        OrganisationBankAccountId = organisationBankAccountId;
    }

}
