namespace OrganisationRegistry.UnitTests.Organisation.CreateOrganisationFromImport;

using System;
using OrganisationRegistry.Organisation;

public class OutputRecordStub : IOutputRecord
{
    public Guid ParentOrganisationId { get; init; }
    public string Name { get; init; } = null!;
    public DateOnly? Validity_Start { get; init; }
    public string? ShortName { get; init; }
    public Article? Article { get; init; }
    public DateOnly? OperationalValidity_Start { get; init; }
}
