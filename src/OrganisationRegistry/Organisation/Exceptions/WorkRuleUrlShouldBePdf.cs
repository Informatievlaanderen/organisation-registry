namespace OrganisationRegistry.Organisation.Exceptions;

public class WorkRuleUrlShouldBePdf : DomainException
{
    public WorkRuleUrlShouldBePdf() : base("Arbeidsreglement moet een Pdf zijn.") { }
}
