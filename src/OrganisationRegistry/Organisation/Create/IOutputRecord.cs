namespace OrganisationRegistry.Organisation;

using System;

public interface IOutputRecord
{
    Guid ParentOrganisationId { get; }
    string Name { get; }
    DateOnly? Validity_Start { get; }
    string? ShortName { get; }
    Article? Article { get; }
    DateOnly? OperationalValidity_Start { get; }
}
