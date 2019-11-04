namespace OrganisationRegistry.Body
{
    using System;
    using System.Collections.Generic;
    using SeatType;

    public class BodySeat : IEquatable<BodySeat>
    {
        private readonly BodyMandates _bodyMandates;

        public BodySeatId BodySeatId { get; }
        public string Name { get; }
        public string BodySeatNumber { get; private set; }
        public SeatType SeatType { get; }
        public bool PaidSeat { get; }
        public bool EntitledToVote { get; set; }
        public Period Validity { get; }
        public IReadOnlyCollection<BodyMandate> BodyMandates => _bodyMandates.AsReadOnly();

        public BodySeat(
            BodySeatId bodySeatId,
            string name,
            string bodySeatNumber,
            SeatType seatType,
            bool paidSeat,
            bool entitledToVote,
            Period validity,
            IEnumerable<BodyMandate> bodyMandates = null)
        {
            _bodyMandates = bodyMandates != null ? new BodyMandates(bodyMandates) : new BodyMandates();

            BodySeatId = bodySeatId;
            Name = name;
            BodySeatNumber = bodySeatNumber;
            SeatType = seatType;
            PaidSeat = paidSeat;
            EntitledToVote = entitledToVote;
            Validity = validity;
        }

        public void AssignBodySeatNumber(string bodySeatNumber)
        {
            BodySeatNumber = bodySeatNumber;
        }

        public bool HasABodyMandateWithOverlappingValidity(Period validity)
        {
            return _bodyMandates.HasABodyMandateWithOverlappingValidity(validity);
        }

        public bool HasAnotherBodyMandateWithOverlappingValidity(Period validity, BodyMandateId bodyMandateId)
        {
            return _bodyMandates.HasAnotherBodyMandateWithOverlappingValidity(validity, bodyMandateId);
        }

        public void AssignMandate(BodyMandate bodyMandate)
        {
            _bodyMandates.Add(bodyMandate);
        }

        public void UnassignMandate(BodyMandate bodyMandate)
        {
            _bodyMandates.RemoveById(bodyMandate.BodyMandateId);
        }

        public void UnassignMandate(BodyMandateId bodyMandateId)
        {
            _bodyMandates.RemoveById(bodyMandateId);
        }

        public BodyMandates BodyMandatesInPeriod(Period validity)
        {
            return _bodyMandates.ForPeriod(validity);
        }

        public override bool Equals(object obj) => obj is BodySeat && Equals((BodySeat) obj);

        public bool Equals(BodySeat other) => BodySeatId == other.BodySeatId;

        public override int GetHashCode() => BodySeatId.GetHashCode();
    }
}
