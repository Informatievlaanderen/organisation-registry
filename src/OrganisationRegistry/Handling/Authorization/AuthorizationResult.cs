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
        => new(domainException);

    public static AuthorizationResult Success()
        => new(null);

    public void ThrowOnFailure()
    {
        if (!IsSuccessful && Exception is { } exception)
            throw exception;
    }
}
