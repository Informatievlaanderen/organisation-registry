namespace OrganisationRegistry.Organisation.Exceptions;

public class FunctionStillUsedInCapacity : DomainException
{
    public FunctionStillUsedInCapacity() : base("De opgegeven functie wordt nog gebruikt in één of meerdere hoedanigheden.")
    {
    }
}
