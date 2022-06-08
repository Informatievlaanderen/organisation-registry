namespace OrganisationRegistry.Organisation.Events;

using System;
using System.Collections.Generic;

public class OrganisationsCreatedFromImport: BaseEvent<OrganisationsCreatedFromImport>
{
    public List<OrganisationCreatedFromImport> Organisations { get; set; }
}

public class OrganisationCreatedFromImport
{
    public Guid Id { get; set; }
}
