namespace OrganisationRegistry.Body;

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Person;

public class BodySeats : IEnumerable<BodySeat>
{
    private readonly List<BodySeat> _bodySeats;

    public BodySeats()
    {
        _bodySeats = new List<BodySeat>();
    }

    public void Add(BodySeat bodySeat)
    {
        _bodySeats.Add(bodySeat);
    }

    public void Remove(BodySeat bodySeat)
    {
        _bodySeats.Remove(bodySeat);
    }

    public bool CanThisPersonBeAssignedInThisPeriod(PersonId personId, Period validity)
    {
        return _bodySeats
            .SelectMany(seat => seat.BodyMandatesInPeriod(validity))
            .OfType<PersonBodyMandate>()
            .Any(mandate => mandate.PersonId == personId);
    }

    public bool CanThisPersonBeReassignedInThisPeriod(BodyMandateId bodyMandateId, PersonId personId, Period validity)
    {
        return _bodySeats
            .SelectMany(seat => seat.BodyMandatesInPeriod(validity))
            .OfType<PersonBodyMandate>()
            .Where(mandate => mandate.BodyMandateId != bodyMandateId)
            .Any(mandate => mandate.PersonId == personId);
    }

    public IReadOnlyCollection<BodySeat> AsReadOnly() => _bodySeats.AsReadOnly();

    public IEnumerator<BodySeat> GetEnumerator() => _bodySeats.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
