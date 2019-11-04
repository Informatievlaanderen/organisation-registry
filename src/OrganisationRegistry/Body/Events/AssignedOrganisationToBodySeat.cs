namespace OrganisationRegistry.Body.Events
{
    using System;

    public class AssignedOrganisationToBodySeat : BaseEvent<AssignedOrganisationToBodySeat>
    {
        public Guid BodyId => Id;

        public Guid BodyMandateId { get; }
        public Guid BodySeatId { get; }
        public Guid OrganisationId { get; }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }

        public string BodySeatNumber { get; }
        public string BodySeatName { get; }
        public string OrganisationName { get; }

        public int? BodySeatTypeOrder { get; }

        public AssignedOrganisationToBodySeat(
            Guid bodyId,
            Guid bodyMandateId,
            Guid bodySeatId,
            string bodySeatNumber,
            string bodySeatName,
            int? bodySeatTypeOrder,
            Guid organisationId,
            string organisationName,
            DateTime? validFrom,
            DateTime? validTo)
        {
            Id = bodyId;

            BodyMandateId = bodyMandateId;

            BodySeatId = bodySeatId;
            BodySeatNumber = bodySeatNumber;
            BodySeatName = bodySeatName;

            BodySeatTypeOrder = bodySeatTypeOrder;

            OrganisationId = organisationId;
            OrganisationName = organisationName;

            ValidFrom = validFrom;
            ValidTo = validTo;
        }
    }
}
