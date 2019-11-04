namespace OrganisationRegistry.Body
{
    public class CannotAssignPersonToPersonBodyMandateException : DomainException
    {
        public CannotAssignPersonToPersonBodyMandateException()
            : base("Er kan geen persoon worden toegekend aan een persoonlijk mandaat.") { }
    }
}

