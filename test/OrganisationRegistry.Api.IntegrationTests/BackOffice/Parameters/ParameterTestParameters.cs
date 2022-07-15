namespace OrganisationRegistry.Api.IntegrationTests.BackOffice.Parameters;

using System;
using System.Collections.Immutable;

public record ParameterTestParameters(
    Type CreateParameterRequestType,
    bool SupportsRemoval,
    ImmutableList<string> DependencyRoutes);
