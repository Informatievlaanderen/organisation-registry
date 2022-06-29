namespace OrganisationRegistry.Organisation.Exceptions;

using System;

public class InsufficientRights<T> : DomainException
    where T : class
{
    public InsufficientRights(T source)
        : base($"U heeft onvoldoende rechten voor deze actie. Bron: {source}") { }
}

[Obsolete("Use the generic variant instead.")]
public class InsufficientRights : DomainException
{
    public InsufficientRights()
        : base("U heeft onvoldoende rechten voor deze actie.") { }
}
