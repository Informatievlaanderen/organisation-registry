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
    {
        return new CachedPerson(
            person.PersonId,
            person.PersonSex);
    }

    public static CachedPerson Empty()
    {
        return new CachedPerson(
            null,
            null);
    }
}