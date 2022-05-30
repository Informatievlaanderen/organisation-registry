namespace OrganisationRegistry.Organisation.Exceptions;

public class WorkRuleUrlShouldBeValidUrl : DomainException
{
    public WorkRuleUrlShouldBeValidUrl() : base("Arbeidsreglement moet een geldige Url zijn.") { }
}