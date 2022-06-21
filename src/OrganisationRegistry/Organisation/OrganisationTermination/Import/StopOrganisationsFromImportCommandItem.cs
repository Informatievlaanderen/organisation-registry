namespace OrganisationRegistry.Organisation.Import;

using System;

public record StopOrganisationsFromImportCommandItem(Guid OrganisationId, DateOnly Organisation_End);
