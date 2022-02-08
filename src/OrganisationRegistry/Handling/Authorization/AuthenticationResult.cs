namespace OrganisationRegistry.Handling.Authorization
{
    public class AuthenticationResult
    {
        public DomainException? Exception { get; }
        public bool IsSuccessful => Exception == null;

        private AuthenticationResult(DomainException? domainException)
        {
            Exception = domainException;
        }

        public static AuthenticationResult Fail(DomainException domainException)
        {
            return new AuthenticationResult(domainException);
        }

        public static AuthenticationResult Success()
        {
            return new AuthenticationResult(null);
        }
    }
}
