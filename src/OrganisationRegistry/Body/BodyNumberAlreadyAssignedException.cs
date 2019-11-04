namespace OrganisationRegistry.Body
{
    public class BodyNumberAlreadyAssignedException : DomainException
    {
        public BodyNumberAlreadyAssignedException()
            : base("Dit orgaan heeft reeds een nummer.") { }
    }
}

