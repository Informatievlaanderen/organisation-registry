namespace OrganisationRegistry.Body;

using System;
using System.Collections.Generic;
using System.Linq;
using Person;

public abstract class BodyMandate : IEquatable<BodyMandate>
{
    public BodyMandateId BodyMandateId { get; }
    public Period Validity { get; }
    public List<Assignment> Assignments { get; private set; }
    public Assignment? CurrentAssignment { get; private set; }

    protected BodyMandate(BodyMandateId bodyMandateId, Period validity)
    {
        BodyMandateId = bodyMandateId;
        Validity = validity;
        Assignments = new List<Assignment>();
    }

    public override bool Equals(object? obj)
        => obj is BodyMandate mandate && Equals(mandate);

    public bool Equals(BodyMandate? other)
        => other is { } bodyMandate && BodyMandateId == bodyMandate.BodyMandateId;

    public override int GetHashCode()
        => BodyMandateId.GetHashCode();

    public abstract void AssignPerson(DelegationAssignmentId delegationAssignmentId, PersonId personId, string personFullName, Dictionary<Guid, string> contacts, Period period);

    protected void AddAssignment(Assignment assignment)
    {
        Assignments.Add(assignment);
    }

    public bool HasPersonDelegationWithOverlappingValidity(Period validity)
    {
        return Assignments
            .Any(assignment => assignment.Validity.OverlapsWith(validity));
    }

    public bool HasAnotherPersonDelegationWithOverlappingValidity(DelegationAssignmentId delegationAssignmentId, Period validity)
    {
        return Assignments
            .Where(assignment => assignment.Id != delegationAssignmentId)
            .Any(assignment => assignment.Validity.OverlapsWith(validity));
    }

    public void RemoveAssignment(DelegationAssignmentId delegationAssignmentId)
    {
        var assignmentToRemove = Assignments.SingleOrDefault(assignment => assignment.Id == delegationAssignmentId);
        if (assignmentToRemove == null)
            return;

        Assignments.Remove(assignmentToRemove);
    }

    public void SetAssignments(List<Assignment> assignments)
    {
        Assignments = assignments;
    }

    public void SetCurrentAssignment(Guid delegationAssignmentId)
    {
        CurrentAssignment = Assignments.Single(assignment => assignment.Id == delegationAssignmentId);
    }

    public void SetCurrentAssignment(Assignment? assignment)
    {
        CurrentAssignment = assignment;
    }

    public void ClearCurrentAssignment()
    {
        CurrentAssignment = null;
    }
}

public class Assignment
{
    public DelegationAssignmentId Id { get; }
    public PersonId PersonId { get; }
    public string PersonFullName { get; }
    public Dictionary<Guid, string> Contacts { get; }
    public Period Validity { get; }

    public Assignment(DelegationAssignmentId id, PersonId personId, string personFullName, Dictionary<Guid, string> contacts, Period validity)
    {
        Id = id;
        PersonId = personId;
        PersonFullName = personFullName;
        Contacts = contacts;
        Validity = validity;
    }
}
