namespace OrganisationRegistry.Body.Exceptions
{
    public class BodySeatNumberAlreadyAssigned : DomainException
    {
        public BodySeatNumberAlreadyAssigned()
            : base("Deze post heeft reeds een nummer.") { }
    }
}

