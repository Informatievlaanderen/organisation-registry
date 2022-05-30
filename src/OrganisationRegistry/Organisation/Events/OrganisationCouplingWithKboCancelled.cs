namespace OrganisationRegistry.Organisation.Events;

using System;
using System.Collections.Generic;

public class OrganisationCouplingWithKboCancelled : BaseEvent<OrganisationCouplingWithKboCancelled>
{
    public Guid OrganisationId => Id;

    public string PreviousKboNumber { get; }
    public string NameBeforeKboCoupling { get; }
    public string ShortNameBeforeKboCoupling { get; }
    public string NameFromKboCoupling { get; }
    public string? ShortNameFromKboCoupling { get; }
    public string OvoNumber { get; }
    public Guid? LegalFormOrganisationOrganisationClassificationIdToCancel { get; }
    public Guid? FormalNameOrganisationLabelIdToCancel { get; }
    public Guid? RegisteredOfficeOrganisationLocationIdToCancel { get; }
    public List<Guid> OrganisationBankAccountIdsToCancel { get; }

    public OrganisationCouplingWithKboCancelled(Guid organisationId,
        string previousKboNumber,
        string nameBeforeKboCoupling,
        string shortNameBeforeKboCoupling,
        string nameFromKboCoupling,
        string? shortNameFromKboCoupling,
        string ovoNumber,
        Guid? legalFormOrganisationOrganisationClassificationIdToCancel,
        Guid? formalNameOrganisationLabelIdToCancel,
        Guid? registeredOfficeOrganisationLocationIdToCancel,
        List<Guid> organisationBankAccountIdsToCancel)
    {
        Id = organisationId;

        PreviousKboNumber = previousKboNumber;
        NameBeforeKboCoupling = nameBeforeKboCoupling;
        ShortNameBeforeKboCoupling = shortNameBeforeKboCoupling;
        NameFromKboCoupling = nameFromKboCoupling;
        ShortNameFromKboCoupling = shortNameFromKboCoupling;
        OvoNumber = ovoNumber;
        LegalFormOrganisationOrganisationClassificationIdToCancel = legalFormOrganisationOrganisationClassificationIdToCancel;
        FormalNameOrganisationLabelIdToCancel = formalNameOrganisationLabelIdToCancel;
        RegisteredOfficeOrganisationLocationIdToCancel = registeredOfficeOrganisationLocationIdToCancel;
        OrganisationBankAccountIdsToCancel = organisationBankAccountIdsToCancel;
    }
}