namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.StopOrganisations;

using System.Collections.Immutable;

public static class ColumnNames
{
    public const string OvoNumber = "ovonumber";
    public const string Name = "name";
    public const string Organisation_End = "organisation_end";

    public static readonly ImmutableList<string> Required = ImmutableList.Create(OvoNumber, Name, Organisation_End);
    public static readonly ImmutableList<string> Optional = ImmutableList<string>.Empty;
}
