namespace OrganisationRegistry.Projections.Reporting.Projections;

using System;
using Person;
using SqlServer.Reporting;

public class CachedPerson
{
    public Guid? PersonId { get; set; }
    public Sex? Sex { get; set; }

    private CachedPerson(
        Guid? personId,
        Sex? sex)
    {
        PersonId = personId;
        Sex = sex;
    }

    public static CachedPerson FromCache(BodySeatGenderRatioPersonListItem person)
        => new(person.PersonId, person.PersonSex);

    public static CachedPerson Empty()
        => new(null, null);
}
