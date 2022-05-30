namespace OrganisationRegistry.Handling.Authorization;

public readonly struct AuthorizationResult
{
    public DomainException? Exception { get; }
    public bool IsSuccessful => Exception == null;

    private AuthorizationResult(DomainException? domainException)
    {
        Exception = domainException;
    }

    public static AuthorizationResult Fail(DomainException domainException)
    {
        return new AuthorizationResult(domainException);
    }

    public static AuthorizationResult Success()
    {
        return new AuthorizationResult(null);
    }
}