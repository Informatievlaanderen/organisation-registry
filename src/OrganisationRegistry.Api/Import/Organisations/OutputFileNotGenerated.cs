namespace OrganisationRegistry.Api.Import.Organisations;

using System;

public class OutputFileNotGenerated : Exception
{
    public OutputFileNotGenerated() : base("Something went wrong while generating an output file. Please try again.")
    {
    }
}
