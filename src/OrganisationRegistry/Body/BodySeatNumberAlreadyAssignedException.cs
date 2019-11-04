namespace OrganisationRegistry.Body
{
    public class BodySeatNumberAlreadyAssignedException : DomainException
    {
        public BodySeatNumberAlreadyAssignedException()
            : base("Deze post heeft reeds een nummer.") { }
    }
}

