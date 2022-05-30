namespace OrganisationRegistry.Organisation;

using System;
using KeyTypes;

public class AddOrganisationKey : BaseCommand<OrganisationId>
{
    public OrganisationId OrganisationId => Id;

    public Guid OrganisationKeyId { get; }
    public KeyTypeId KeyTypeId { get; }
    public string KeyValue { get; }
    public ValidFrom ValidFrom { get; }
    public ValidTo ValidTo { get; }

    public AddOrganisationKey(
        Guid organisationKeyId,
        OrganisationId organisationId,
        KeyTypeId keyTypeId,
        string keyValue,
        ValidFrom validFrom,
        ValidTo validTo)
    {
        Id = organisationId;

        OrganisationKeyId = organisationKeyId;
        KeyTypeId = keyTypeId;
        KeyValue = keyValue;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}