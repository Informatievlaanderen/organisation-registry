namespace OrganisationRegistry.Organisation.OrganisationTermination
{
    using System;
    using System.Collections.Generic;

    public class OrganisationTerminationKboSummary
    {
        public Dictionary<Guid, DateTime> KboBankAccounts { get; init; }
        public KeyValuePair<Guid, DateTime>? KboRegisteredOfficeLocation { get; init; }
        public KeyValuePair<Guid, DateTime>? KboFormalNameLabel { get; init; }
        public KeyValuePair<Guid, DateTime>? KboLegalForm { get; init; }

        public OrganisationTerminationKboSummary()
        {
            KboBankAccounts = new Dictionary<Guid, DateTime>();
        }

    }
}
