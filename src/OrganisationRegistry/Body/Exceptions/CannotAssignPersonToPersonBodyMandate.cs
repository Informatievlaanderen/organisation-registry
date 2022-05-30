namespace OrganisationRegistry.Body.Exceptions;

public class CannotAssignPersonToPersonBodyMandate : DomainException
{
    public CannotAssignPersonToPersonBodyMandate()
        : base("Er kan geen persoon worden toegekend aan een persoonlijk mandaat.") { }
}