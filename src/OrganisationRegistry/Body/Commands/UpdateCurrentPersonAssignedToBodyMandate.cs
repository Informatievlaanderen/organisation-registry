namespace OrganisationRegistry.Body.Commands
{
    public class UpdateCurrentPersonAssignedToBodyMandate : BaseCommand<BodyId>
    {
        public BodyId BodyId => Id;

        public BodySeatId BodySeatId { get; }
        public BodyMandateId BodyMandateId { get; }

        public UpdateCurrentPersonAssignedToBodyMandate(
            BodyId bodyId,
            BodySeatId bodySeatId,
            BodyMandateId bodyMandateId)
        {
            BodySeatId = bodySeatId;
            BodyMandateId = bodyMandateId;
            Id = bodyId;
        }

        protected bool Equals(UpdateCurrentPersonAssignedToBodyMandate other)
            => BodyId.Equals(other.BodyId);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((UpdateCurrentPersonAssignedToBodyMandate) obj);
        }

        public override int GetHashCode()
            => BodyId.GetHashCode();
    }
}
