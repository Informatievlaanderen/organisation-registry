namespace OrganisationRegistry.ElasticSearch.Tests.Scenario;

using System;
using System.Collections.Generic;
using Body.Events;
using Projections.Body;
using Specimen;

/// <summary>
/// Sets up a fixture which uses the same bodyId and LifecyclePhaseTypeId for all events
/// </summary>
public class BodyScenario : ScenarioBase<BodyHandler>
{
    public BodyScenario(Guid bodyId) :
        base(
            new ParameterNameArg<Guid>("bodyId", bodyId),
            new ParameterNameArg<Guid>("lifecyclePhaseTypeId", Guid.NewGuid()),
            new ParameterNameArg<Guid>("bodySeatId", Guid.NewGuid()),
            new ParameterNameArg<Guid>("bodyMandateId", Guid.NewGuid()),
            new ParameterNameArg<Guid>("delegationAssignmentId", Guid.NewGuid()),
            new ParameterNameArg<Guid>("personId", Guid.NewGuid()),
            new ParameterNameArg<Guid>("organisationId", Guid.NewGuid()))
    {
        var functionTypeId = Guid.NewGuid();
        AddCustomization(new ParameterNameArg<Guid>("functionId", functionTypeId));
        AddCustomization(new ParameterNameArg<Guid>("functionTypeId", functionTypeId));
    }

    public PersonAssignedToDelegation CreatePersonAssignedToDelegation(Guid bodyId, Guid bodySeatId, Guid bodyMandateId, string? personFullName = null)
        => new(
            bodyId,
            bodySeatId,
            bodyMandateId,
            Create<Guid>(),
            Create<Guid>(),
            personFullName ?? Create<string>(),
            new Dictionary<Guid, string>(),
            Create<DateTime?>(),
            Create<DateTime?>()
        );

    public PersonAssignedToDelegationUpdated CreatePersonAssignedToDelegationUpdated(Guid bodyId, Guid bodySeatId, Guid bodyMandateId, Guid delegationAssignmentId)
        => new(
            bodyId,
            bodySeatId,
            bodyMandateId,
            delegationAssignmentId,
            Create<Guid>(),
            Create<string>(),
            new Dictionary<Guid, string>(),
            Create<DateTime?>(),
            Create<DateTime?>(),
            Create<Guid>(),
            Create<string>(),
            new Dictionary<Guid, string>(),
            Create<DateTime?>(),
            Create<DateTime?>()
        );

    public PersonAssignedToDelegationRemoved CreatePersonAssignedToDelegationRemoved(Guid bodyId, Guid bodySeatId, Guid bodyMandateId, Guid delegationAssignmentId)
        => new(
            bodyId,
            bodySeatId,
            bodyMandateId,
            delegationAssignmentId,
            Create<Guid>(),
            Create<string>(),
            new Dictionary<Guid, string>(),
            Create<DateTime?>(),
            Create<DateTime?>());
}
