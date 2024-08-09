namespace OrganisationRegistry.Organisation.Events;

using System;
using Infrastructure;
using Newtonsoft.Json;

public class KboLegalEntityTypeAdded : BaseEvent<KboLegalEntityTypeAdded>
{
    public KboLegalEntityTypeAdded(Guid organisationId, string legalEntityTypeCode, string legalEntityTypeDescription)
    {
        Id = organisationId;
        LegalEntityTypeCode = legalEntityTypeCode;
        LegalEntityTypeDescription = legalEntityTypeDescription;
    }

    public Guid OrganisationId => Id;

    public string LegalEntityTypeCode { get; }
    public string LegalEntityTypeDescription { get; }
}
