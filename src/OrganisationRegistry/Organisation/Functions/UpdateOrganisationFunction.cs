namespace OrganisationRegistry.Organisation;

using System;
using System.Collections.Generic;
using ContactType;
using Function;
using Person;

public class UpdateOrganisationFunction : BaseCommand<OrganisationId>
{
    public OrganisationId OrganisationId => Id;

    public Guid OrganisationFunctionId { get; }
    public FunctionTypeId FunctionTypeId { get; }
    public PersonId PersonId { get; }
    public Dictionary<ContactTypeId, string> Contacts { get; }
    public ValidFrom ValidFrom { get; }
    public ValidTo ValidTo { get; }

    public UpdateOrganisationFunction(
        Guid organisationFunctionId,
        OrganisationId organisationId,
        FunctionTypeId functionTypeId,
        PersonId personId,
        Dictionary<ContactTypeId, string>? contacts,
        ValidFrom validFrom,
        ValidTo validTo)
    {
        Id = organisationId;

        OrganisationFunctionId = organisationFunctionId;
        FunctionTypeId = functionTypeId;
        PersonId = personId;
        Contacts = contacts ?? new Dictionary<ContactTypeId, string>();
        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}
