namespace OrganisationRegistry.Organisation.Events
{
    using System;

    public class OrganisationTerminationFoundInKbo : BaseEvent<OrganisationTerminationFoundInKbo>
    {
        public Guid OrganisationId => Id;

        public string KboNumber { get; }
        public DateTime TerminationDate { get; }
        public string TerminationCode { get; }
        public string TerminationReason { get; }

        public OrganisationTerminationFoundInKbo(Guid organisationId,
            string kboNumber,
            DateTime terminationDate,
            string terminationCode,
            string terminationReason)
        {
            KboNumber = kboNumber;
            TerminationDate = terminationDate;
            TerminationCode = terminationCode;
            TerminationReason = terminationReason;
            Id = organisationId;
        }
    }
}
