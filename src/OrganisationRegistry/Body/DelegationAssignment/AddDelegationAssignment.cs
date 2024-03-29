﻿namespace OrganisationRegistry.Body;

using System.Collections.Generic;
using ContactType;
using Organisation;
using Person;

public class AddDelegationAssignment : BaseCommand<BodyId>
{
    public BodyId BodyId => Id;

    public BodyMandateId BodyMandateId { get; }
    public BodySeatId BodySeatId { get; }
    public DelegationAssignmentId DelegationAssignmentId { get; }
    public PersonId PersonId { get; }
    public OrganisationId OrganisationId { get; }
    public Dictionary<ContactTypeId, string> Contacts { get; }
    public Period Validity { get; }

    public AddDelegationAssignment(
        BodyMandateId bodyMandateId,
        BodyId bodyId,
        BodySeatId bodySeatId,
        DelegationAssignmentId delegationAssignmentId,
        PersonId personId,
        OrganisationId organisationId,
        Dictionary<ContactTypeId, string> contacts,
        Period validity)
    {
        Id = bodyId;

        BodyMandateId = bodyMandateId;
        BodySeatId = bodySeatId;
        DelegationAssignmentId = delegationAssignmentId;
        PersonId = personId;
        OrganisationId = organisationId;
        Contacts = contacts;
        Validity = validity;
    }
}
