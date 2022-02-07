namespace OrganisationRegistry.Body.Commands
{
    using System.Collections.Generic;

    public class UpdateCurrentPersonAssignedToBodyMandate : BaseCommand<BodyId>
    {
        public BodyId BodyId => Id;

        public List<(BodySeatId bodySeatId, BodyMandateId bodyMandateId)> MandatesToUpdate { get; }

        public UpdateCurrentPersonAssignedToBodyMandate(
            BodyId bodyId,
            List<(BodySeatId bodySeatId,BodyMandateId bodyMandateId)> mandatesToUpdate)
        {
            Id = bodyId;
            MandatesToUpdate = mandatesToUpdate;
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
