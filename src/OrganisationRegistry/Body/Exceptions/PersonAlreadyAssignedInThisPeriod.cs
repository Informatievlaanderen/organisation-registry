namespace OrganisationRegistry.Body.Exceptions
{
    public class PersonAlreadyAssignedInThisPeriod : DomainException
    {
        public PersonAlreadyAssignedInThisPeriod()
            : base("Deze persoon heeft in deze periode reeds een mandaat bij dit orgaan.") { }
    }
}
