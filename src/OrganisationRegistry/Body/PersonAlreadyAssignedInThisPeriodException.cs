namespace OrganisationRegistry.Body
{
    public class PersonAlreadyAssignedInThisPeriodException : DomainException
    {
        public PersonAlreadyAssignedInThisPeriodException()
            : base("Deze persoon heeft in deze periode reeds een mandaat bij dit orgaan.") { }
    }
}
