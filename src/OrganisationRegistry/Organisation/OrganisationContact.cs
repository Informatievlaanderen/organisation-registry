﻿namespace OrganisationRegistry.Organisation;

using System;

public class OrganisationContact : IOrganisationField, IValidityBuilder<OrganisationContact>
{
    public Guid Id => OrganisationContactId;
    public Guid OrganisationId { get; } // todo: remove organisationId from this (but not from event, possibly not from command)
    public Guid OrganisationContactId { get; }
    public Guid ContactTypeId { get; }
    public string ContactTypeName { get; }
    public string Value { get; }
    public Period Validity { get; }

    public OrganisationContact(
        Guid organisationContactId,
        Guid organisationId,
        Guid contactTypeId,
        string contactTypeName,
        string value,
        Period validity)
    {
        OrganisationId = organisationId;
        OrganisationContactId = organisationContactId;
        ContactTypeId = contactTypeId;
        ContactTypeName = contactTypeName;
        Value = value;
        Validity = validity;
    }

    public OrganisationContact WithValidity(Period period)
        => new(
            OrganisationContactId,
            OrganisationId,
            ContactTypeId,
            ContactTypeName,
            Value,
            period);

    public OrganisationContact WithValidFrom(ValidFrom validFrom)
        => WithValidity(new Period(validFrom, Validity.End));

    public OrganisationContact WithValidTo(ValidTo validTo)
        => WithValidity(new Period(Validity.Start, validTo));
}
