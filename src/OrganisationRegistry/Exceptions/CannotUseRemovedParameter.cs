namespace OrganisationRegistry.Exceptions;

public class CannotUseRemovedParameter : DomainException
{
    public CannotUseRemovedParameter(string parameterName, string name)
        : base($"{parameterName} {name} is reeds verwijderd en kan niet gebruikt worden") { }
}
