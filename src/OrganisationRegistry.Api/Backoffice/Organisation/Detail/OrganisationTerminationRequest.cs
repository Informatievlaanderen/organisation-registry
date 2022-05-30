namespace OrganisationRegistry.Api.Backoffice.Organisation.Detail;

using System;

public class OrganisationTerminationRequest
{
    public DateTime DateOfTermination { get; set; }
    public bool ForceKboTermination { get; set; }
}