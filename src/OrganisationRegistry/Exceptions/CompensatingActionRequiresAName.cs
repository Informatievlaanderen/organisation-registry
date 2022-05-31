namespace OrganisationRegistry.Exceptions;

public class CompensatingActionRequiresAName : DomainException
{
    public CompensatingActionRequiresAName()
        : base("Voor het uitvoeren van een compensating action is een naam vereist.") { }
}
