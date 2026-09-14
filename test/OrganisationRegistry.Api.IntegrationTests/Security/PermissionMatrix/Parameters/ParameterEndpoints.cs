namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Parameters;

using System.Collections.Generic;

/// <summary>
/// Shared data for the parameter (master-data) permission matrix tests.
///
/// Every parameter screen is gated by its own fine-grained
/// <c>Parameters&lt;Thing&gt;Read/Write/Delete</c> permission. Only
/// <see cref="OrganisationRegistry.Infrastructure.Authorization.Role.AlgemeenBeheerder" />
/// (and Developer) hold them; every other interactive role must receive 403.
/// </summary>
public static class ParameterEndpoints
{
    /// <summary>The list/collection route of every in-scope parameter screen.</summary>
    public static readonly string[] ListRoutes =
    {
        "locations",
        "buildings",
        "keytypes",
        "organisationclassifications",
        "organisationclassificationtypes",
        "bodyclassifications",
        "bodyclassificationtypes",
        "organisationrelationtypes",
        "formalframeworks",
        "formalframeworkcategories",
        "lifecyclephasetypes",
        "capacities",
        "functiontypes",
        "contacttypes",
        "labeltypes",
        "purposes",
        "seattypes",
        "mandateroletypes",
        "locationtypes",
        "regulationthemes",
        "regulationsubthemes",
    };

    /// <summary>
    /// Interactive backoffice roles that must NOT have access to parameter
    /// master-data management. All of these are expected to receive 403.
    /// </summary>
    public static readonly string[] NonPrivilegedRoles =
    {
        ApiFixture.Backoffice.Vlimpersbeheerder,
        ApiFixture.Backoffice.Decentraalbeheerder,
        ApiFixture.Backoffice.Orgaanbeheerder,
        ApiFixture.Backoffice.Regelgevingbeheerder,
        ApiFixture.Backoffice.Cjmbeheerder,
    };

    public static IEnumerable<object[]> ListRouteData()
    {
        foreach (var route in ListRoutes)
            yield return new object[] { route };
    }

    public static IEnumerable<object[]> NonPrivilegedRoleData()
    {
        foreach (var role in NonPrivilegedRoles)
            yield return new object[] { role };
    }
}
