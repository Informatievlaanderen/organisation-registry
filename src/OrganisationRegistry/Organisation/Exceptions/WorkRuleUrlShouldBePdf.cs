namespace OrganisationRegistry.Organisation.Exceptions
{
    using System;

    public class WorkRuleUrlShouldBePdf : DomainException
    {
        public WorkRuleUrlShouldBePdf() : base("Arbeidsreglement moet een Pdf zijn.") { }
    }
}
