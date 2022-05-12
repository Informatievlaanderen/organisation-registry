namespace OrganisationRegistry.Organisation
{
    using System;
    using System.Collections.Generic;

    public record OrganisationTerminationSummary(
        DateTime? OrganisationNewValidTo,
        Dictionary<Guid, DateTime> Contacts,
        Dictionary<Guid, DateTime> BankAccounts,
        Dictionary<Guid, DateTime> Functions,
        Dictionary<Guid, DateTime> Locations,
        Dictionary<Guid, DateTime> Capacities,
        Dictionary<Guid, DateTime> Buildings,
        Dictionary<Guid, DateTime> Labels,
        Dictionary<Guid, DateTime> Relations,
        Dictionary<Guid, DateTime> OpeningHours,
        Dictionary<Guid, DateTime> Classifications,
        Dictionary<Guid, DateTime> FormalFrameworks,
        Dictionary<Guid, DateTime> Regulations,
        Dictionary<Guid, DateTime> Keys
    );
}
