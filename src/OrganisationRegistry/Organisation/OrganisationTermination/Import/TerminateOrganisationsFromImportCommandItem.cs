namespace OrganisationRegistry.Organisation.Import;

using System;

public record TerminateOrganisationsFromImportCommandItem(Guid OrganisationId, DateOnly Organisation_End);
