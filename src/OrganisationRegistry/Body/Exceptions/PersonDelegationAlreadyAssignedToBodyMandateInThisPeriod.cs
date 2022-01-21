namespace OrganisationRegistry.Body.Exceptions
{
    public class PersonDelegationAlreadyAssignedToBodyMandateInThisPeriod : DomainException
    {
        public PersonDelegationAlreadyAssignedToBodyMandateInThisPeriod()
            : base("Er is in deze periode reeds een persoon toegewezen aan dit mandaat.") { }
    }
}
