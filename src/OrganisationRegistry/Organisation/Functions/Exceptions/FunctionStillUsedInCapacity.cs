namespace OrganisationRegistry.Organisation.Exceptions;

public class FunctionStillUsedInCapacity : DomainException
{
    public FunctionStillUsedInCapacity() : base("De opgegeven functie word nog gebruikt in één of meerdere hoedanigheden.")
    {
    }
}
