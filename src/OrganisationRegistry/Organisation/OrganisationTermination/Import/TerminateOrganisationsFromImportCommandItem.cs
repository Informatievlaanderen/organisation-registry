namespace OrganisationRegistry.Organisation.Import;

using System;

public record TerminateOrganisationsFromImportCommandItem(Guid OrganisationId, string OvoNumber, DateOnly Organisation_End);
