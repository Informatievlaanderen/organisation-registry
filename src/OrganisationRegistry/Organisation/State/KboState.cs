namespace OrganisationRegistry.Organisation.State;

using System.Collections.Generic;

public class KboState
{
    public OrganisationLocation? KboRegisteredOffice { get; set; }
    public OrganisationLabel? KboFormalNameLabel { get; set; }
    public OrganisationOrganisationClassification? KboLegalFormOrganisationClassification { get; set; }
    public List<OrganisationBankAccount> KboBankAccounts { get; }
    public string? NameBeforeKboCoupling { get; set; }
    public string? ShortNameBeforeKboCoupling { get; set; }
    public KboTermination? TerminationInKbo { get; set; }
    public KboNumber? KboNumber { get; set; }

    public void Clear()
    {
        KboNumber = null;
        TerminationInKbo = null;
        KboBankAccounts.Clear();
        KboRegisteredOffice = null;
        KboFormalNameLabel = null;
        KboLegalFormOrganisationClassification = null;
    }

    public KboState()
    {
        KboBankAccounts = new List<OrganisationBankAccount>();
    }
}