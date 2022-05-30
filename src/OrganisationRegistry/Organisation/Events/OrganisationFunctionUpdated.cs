namespace OrganisationRegistry.Organisation.Events;

using System;
using System.Collections.Generic;

public class OrganisationFunctionUpdated : BaseEvent<OrganisationFunctionUpdated>
{
    public Guid OrganisationId => Id;

    public Guid OrganisationFunctionId { get; }

    public Guid FunctionId { get; }
    public Guid PreviousFunctionId { get; }

    public string FunctionName { get; }
    public string PreviousFunctionName { get; }

    public Guid PersonId { get; }
    public Guid PreviousPersonId { get; }

    public string PersonFullName { get; }
    public string PreviousPersonFullName { get; }

    public Dictionary<Guid, string> Contacts { get; }
    public Dictionary<Guid, string> PreviousContacts { get; }

    public DateTime? ValidFrom { get; }
    public DateTime? PreviouslyValidFrom { get; }

    public DateTime? ValidTo { get; }
    public DateTime? PreviouslyValidTo { get; }

    public OrganisationFunctionUpdated(
        Guid organisationId,
        Guid organisationFunctionId,
        Guid functionId,
        string functionName,
        Guid personId,
        string personFullName,
        Dictionary<Guid, string> contacts,
        DateTime? validFrom,
        DateTime? validTo,

        Guid previousFunctionId,
        string previousFunctionName,
        Guid previousPersonId,
        string previousPersonFullName,
        Dictionary<Guid, string> previousContacts,
        DateTime? previouslyValidFrom,
        DateTime? previouslyValidTo)
    {
        Id = organisationId;

        OrganisationFunctionId = organisationFunctionId;
        FunctionId = functionId;
        FunctionName = functionName;
        PersonId = personId;
        PersonFullName = personFullName;
        Contacts = contacts;
        ValidFrom = validFrom;
        ValidTo = validTo;

        PreviousFunctionId = previousFunctionId;
        PreviousFunctionName = previousFunctionName;
        PreviousPersonId = previousPersonId;
        PreviousPersonFullName = previousPersonFullName;
        PreviousContacts = previousContacts;
        PreviouslyValidFrom = previouslyValidFrom;
        PreviouslyValidTo = previouslyValidTo;
    }
}