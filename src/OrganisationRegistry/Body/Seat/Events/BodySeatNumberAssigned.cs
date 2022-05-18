namespace OrganisationRegistry.Body.Events
{
    using System;

    public class BodySeatNumberAssigned : BaseEvent<BodySeatNumberAssigned>
    {
        public Guid BodyId => Id;

        public Guid BodySeatId { get; }
        public string BodySeatNumber { get; }

        public BodySeatNumberAssigned(
            Guid bodyId,
            Guid bodySeatId,
            string bodySeatNumber)
        {
            Id = bodyId;

            BodySeatId = bodySeatId;
            BodySeatNumber = bodySeatNumber;
        }
    }
}
