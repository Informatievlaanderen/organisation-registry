namespace OrganisationRegistry.Organisation;

using System;
using KeyTypes;

public class OrganisationKey : IOrganisationField, IValidityBuilder<OrganisationKey>
{
    public Guid Id => OrganisationKeyId;
    public Guid OrganisationId { get; } // todo: remove organisationId from this (but not from event, possibly not from command) // Why ?
    public Guid OrganisationKeyId { get; }
    public Guid KeyTypeId { get; }
    public string KeyTypeName { get; }
    public string Value { get; }
    public Period Validity { get; }

    public OrganisationKey(
        Guid organisationKeyId,
        Guid organisationId,
        Guid keyTypeId,
        string keyTypeName,
        string value,
        Period validity)
    {
        OrganisationId = organisationId;
        OrganisationKeyId = organisationKeyId;
        KeyTypeId = keyTypeId;
        Value = value;
        Validity = validity;
        KeyTypeName = keyTypeName;
    }

    public OrganisationKey WithValidity(Period period)
        => new(
            OrganisationKeyId,
            OrganisationId,
            KeyTypeId,
            KeyTypeName,
            Value,
            period);

    public OrganisationKey WithKeyType(KeyType keyType)
        => new(
            OrganisationKeyId,
            OrganisationId,
            keyType.Id,
            keyType.Name,
            Value,
            Validity);

    public OrganisationKey WithValidFrom(ValidFrom validFrom)
        => WithValidity(new Period(validFrom, Validity.End));

    public OrganisationKey WithValidTo(ValidTo validTo)
        => WithValidity(new Period(Validity.Start, validTo));
}
