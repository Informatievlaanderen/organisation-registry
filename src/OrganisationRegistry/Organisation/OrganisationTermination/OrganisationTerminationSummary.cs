namespace OrganisationRegistry.Organisation.OrganisationTermination
{
    using System;
    using System.Collections.Generic;

    public class OrganisationTerminationSummary
    {
        public DateTime? OrganisationNewValidTo { get; init; }
        public Dictionary<Guid, DateTime> Contacts { get; init; }
        public Dictionary<Guid, DateTime> BankAccounts { get; init; }
        public Dictionary<Guid, DateTime> Functions { get; init; }
        public Dictionary<Guid, DateTime> Locations { get; init; }
        public Dictionary<Guid, DateTime> Capacities { get; init; }
        public Dictionary<Guid, DateTime> Buildings { get; init; }
        public Dictionary<Guid, DateTime> Labels { get; init; }
        public Dictionary<Guid, DateTime> Relations { get; init; }
        public Dictionary<Guid, DateTime> OpeningHours { get; init; }
        public Dictionary<Guid, DateTime> Classifications { get; init; }
        public Dictionary<Guid, DateTime> FormalFrameworks { get; init; }
        public Dictionary<Guid, DateTime> Regulations { get; init; }

        public OrganisationTerminationSummary()
        {
            Contacts = new Dictionary<Guid, DateTime>();
            BankAccounts = new Dictionary<Guid, DateTime>();
            Functions = new Dictionary<Guid, DateTime>();
            Locations = new Dictionary<Guid, DateTime>();
            Capacities = new Dictionary<Guid, DateTime>();
            Buildings = new Dictionary<Guid, DateTime>();
            Labels = new Dictionary<Guid, DateTime>();
            Relations = new Dictionary<Guid, DateTime>();
            OpeningHours = new Dictionary<Guid, DateTime>();
            Classifications = new Dictionary<Guid, DateTime>();
            FormalFrameworks = new Dictionary<Guid, DateTime>();
            Regulations = new Dictionary<Guid, DateTime>();
        }
    }
}
