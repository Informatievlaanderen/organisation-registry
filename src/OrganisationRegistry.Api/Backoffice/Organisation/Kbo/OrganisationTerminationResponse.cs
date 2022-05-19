namespace OrganisationRegistry.Api.Backoffice.Organisation.Kbo
{
    using System;
    using OrganisationRegistry.SqlServer.Organisation;

    public class OrganisationTerminationResponse
    {
        public string Reason { get; }

        public string Code { get; }

        public DateTime? Date { get; }

        public Guid OrganisationId { get; }

        public TerminationStatus Status { get; }

        private OrganisationTerminationResponse(
            Guid organisationId,
            DateTime? date,
            string code,
            string reason,
            TerminationStatus status)
        {
            OrganisationId = organisationId;
            Date = date;
            Code = code;
            Reason = reason;
            Status = status;
        }

        public static OrganisationTerminationResponse FromListItem(OrganisationTerminationListItem organisationTermination)
            => new(
                organisationTermination.Id,
                organisationTermination.Date,
                organisationTermination.Code,
                organisationTermination.Reason,
                organisationTermination.Status
            );

        public static OrganisationTerminationResponse NotFound(Guid id)
            => new(
                id,
                null,
                string.Empty,
                string.Empty,
                TerminationStatus.None
            );
    }
}
