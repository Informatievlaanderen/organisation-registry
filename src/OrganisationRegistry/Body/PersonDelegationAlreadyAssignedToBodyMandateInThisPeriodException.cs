namespace OrganisationRegistry.Body
{
    public class PersonDelegationAlreadyAssignedToBodyMandateInThisPeriodException : DomainException
    {
        public PersonDelegationAlreadyAssignedToBodyMandateInThisPeriodException()
            : base("Er is in deze periode reeds een persoon toegewezen aan dit mandaat.") { }
    }
}
