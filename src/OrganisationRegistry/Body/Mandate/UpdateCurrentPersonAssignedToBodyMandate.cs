namespace OrganisationRegistry.Body;

using System;
using System.Collections.Generic;
using System.Linq;

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
        => BodyId.Equals(other.BodyId)
           && MandatesToUpdate.SequenceEqual(other.MandatesToUpdate);

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((UpdateCurrentPersonAssignedToBodyMandate) obj);
    }

    public override int GetHashCode()
        => HashCode.Combine(BodyId, MandatesToUpdate);
}
