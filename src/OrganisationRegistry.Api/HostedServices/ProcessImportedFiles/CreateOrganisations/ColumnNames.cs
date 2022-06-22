namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.CreateOrganisations;

using System.Collections.Immutable;

public static class ColumnNames
{
    public const string Reference = "reference";
    public const string Name = "name";
    public const string Parent = "parent";
    public const string Validity_Start = "validity_start";
    public const string ShortName = "shortname";
    public const string Article = "article";
    public const string OperationalValidity_Start = "operationalvalidity_start";

    public static readonly ImmutableList<string> Required = ImmutableList.Create(Reference, Parent, Name);
    public static readonly ImmutableList<string> Optional = ImmutableList.Create(Validity_Start, ShortName, Article, OperationalValidity_Start);
}
