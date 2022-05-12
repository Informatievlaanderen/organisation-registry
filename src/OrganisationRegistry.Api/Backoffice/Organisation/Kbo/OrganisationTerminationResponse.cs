namespace OrganisationRegistry.Api.Backoffice.Organisation.Kbo
{
    using System;
    using OrganisationRegistry.SqlServer.Organisation;

    public class OrganisationTerminationResponse
    {
        public string Reason { get; private set; }

        public string Code { get; private set;}

        public DateTime? Date { get; private set;}

        public Guid OrganisationId { get; private set;}

        public TerminationStatus Status { get; private set; }

        private OrganisationTerminationResponse() { }

        public static OrganisationTerminationResponse FromListItem(OrganisationTerminationListItem organisationTermination)
        {
            return new OrganisationTerminationResponse
            {
                OrganisationId = organisationTermination.Id,
                Date = organisationTermination.Date,
                Code = organisationTermination.Code,
                Reason = organisationTermination.Reason,
                Status = organisationTermination.Status,
            };
        }

        public static OrganisationTerminationResponse NotFound(Guid id)
        {
            return new OrganisationTerminationResponse
            {
                OrganisationId = id,
                Date = null,
                Code = string.Empty,
                Reason = string.Empty,
                Status = TerminationStatus.None,
            };
        }
    }
}
