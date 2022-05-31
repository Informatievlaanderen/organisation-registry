namespace OrganisationRegistry;

using System.Collections.Generic;
using Be.Vlaanderen.Basisregisters.AggregateSource;

public class IdName<TId, TName> : ValueObject<IdName<TId, TName>>
    where TId : ValueObject<TId>
    where TName : ValueObject<TName> 
{
    public TId Id { get; }

    public TName Name { get; }

    public IdName(
        TId id,
        TName name)
    {
        Id = id;
        Name = name;
    }

    protected override IEnumerable<object> Reflect()
    {
        yield return Id;
        yield return Name;
    }
}
