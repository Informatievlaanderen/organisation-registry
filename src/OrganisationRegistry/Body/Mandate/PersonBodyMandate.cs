namespace OrganisationRegistry.Body;

using System;
using System.Collections.Generic;
using Exceptions;
using Person;

public class PersonBodyMandate : BodyMandate
{
    public PersonId PersonId { get; }
    public string PersonName { get; }
    public string PersonFirstName { get; }
    public Dictionary<Guid, string> Contacts { get; }

    public PersonBodyMandate(
        BodyMandateId bodyMandateId,
        PersonId personId,
        string personName,
        string personFirstName,
        Dictionary<Guid, string> contacts,
        Period validity)
        : base(bodyMandateId, validity)
    {
        PersonId = personId;
        PersonName = personName;
        PersonFirstName = personFirstName;
        Contacts = contacts;
    }

    public override void AssignPerson(
        DelegationAssignmentId delegationAssignmentId,
        PersonId personId,
        string personFullName,
        Dictionary<Guid, string> contacts,
        Period period)
    {
        throw new CannotAssignPersonToPersonBodyMandate();
    }
}