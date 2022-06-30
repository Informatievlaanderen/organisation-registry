namespace OrganisationRegistry.Organisation.Exceptions;

public class InsufficientRights<T> : DomainException
    where T : class
{
    public InsufficientRights(T source)
        : base($"U heeft onvoldoende rechten voor deze actie. Reden: {source}.")
    {
    }
}

public static class InsufficientRights
{
    public static InsufficientRights<T> CreateFor<T>(T source) where T : class
        => new(source);
}
