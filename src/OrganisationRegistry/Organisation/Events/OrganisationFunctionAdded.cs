namespace OrganisationRegistry.Organisation.Events;

using System;
using System.Collections.Generic;

public class OrganisationFunctionAdded : BaseEvent<OrganisationFunctionAdded>
{
    public Guid OrganisationId => Id;

    public Guid OrganisationFunctionId { get; }
    public Guid FunctionId { get; }
    public string FunctionName { get; }
    public Guid PersonId { get; }
    public string PersonFullName { get; }
    public Dictionary<Guid, string> Contacts { get; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    public OrganisationFunctionAdded(
        Guid organisationId,
        Guid organisationFunctionId,
        Guid functionId,
        string functionName,
        Guid personId,
        string personFullName,
        Dictionary<Guid, string> contacts,
        DateTime? validFrom,
        DateTime? validTo)
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
    }
}