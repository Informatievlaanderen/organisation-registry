namespace OrganisationRegistry.Handling;

using System;
using System.Collections.Generic;
using Authorization;
using Organisation;

public static class HandlerExtensionMethods
{
    public static Handler WithVlimpersPolicy(this Handler source, Organisation organisation)
        => source.WithPolicy(
            new VlimpersPolicy(
                organisation.State.UnderVlimpersManagement,
                organisation.State.OvoNumber));

    public static Handler RequiresAdmin(this Handler source)
        => source.WithPolicy(new AdminOnlyPolicy());

    public static Handler WithImportPolicy(this Handler source, IEnumerable<Guid> organisationIds)
        => source.WithPolicy(session => new ImportPolicy(session, organisationIds));
}
