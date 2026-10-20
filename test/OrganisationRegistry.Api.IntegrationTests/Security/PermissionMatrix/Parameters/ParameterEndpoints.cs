namespace OrganisationRegistry.Api.IntegrationTests.Security.PermissionMatrix.Parameters;

using System.Collections.Generic;

/// <summary>
/// Shared data for the parameter (master-data) permission matrix tests.
///
/// Every parameter screen is gated by its own fine-grained
/// <c>Parameters&lt;Thing&gt;Write/Delete</c> permission for writes/deletes. Only
/// <see cref="OrganisationRegistry.Infrastructure.Authorization.Role.AlgemeenBeheerder" />
/// (and Developer) hold them; every other interactive role must receive 403
/// when writing or deleting. Reading these lists, however, carries no
/// dedicated permission and is open to any authenticated backoffice user.
///
/// Most list screens (see <see cref="PubliclyReadableListRoutes"/>) are, in
/// addition, open to anonymous (not logged in, "Publiek") callers: there is
/// no restriction on reading master-data lists for the public. Only
/// <c>keytypes</c> and <c>organisationrelationtypes</c> remain restricted to
/// authenticated backoffice users (not part of this public-access set).
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
    /// List/collection routes whose controller carries <c>[AllowAnonymous]</c>,
    /// i.e. readable without being logged in at all. This is every route in
    /// <see cref="ListRoutes"/> except <c>keytypes</c> and
    /// <c>organisationrelationtypes</c>.
    /// </summary>
    public static readonly string[] PubliclyReadableListRoutes =
    {
        "locations",
        "buildings",
        "organisationclassifications",
        "organisationclassificationtypes",
        "bodyclassifications",
        "bodyclassificationtypes",
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
    /// master-data management (writing/deleting). All of these are expected
    /// to receive 403 on write/delete, but 200 on reads (see
    /// <see cref="Given_Any_Role_Reading_Parameters"/>).
    /// </summary>
    public static readonly string[] NonPrivilegedRoles =
    {
        ApiFixture.Backoffice.Vlimpersbeheerder,
        ApiFixture.Backoffice.Decentraalbeheerder,
        ApiFixture.Backoffice.Orgaanbeheerder,
        ApiFixture.Backoffice.Regelgevingbeheerder,
        ApiFixture.Backoffice.VoMedewerker,
    };

    public static IEnumerable<object[]> ListRouteData()
    {
        foreach (var route in ListRoutes)
            yield return new object[] { route };
    }

    public static IEnumerable<object[]> PubliclyReadableListRouteData()
    {
        foreach (var route in PubliclyReadableListRoutes)
            yield return new object[] { route };
    }

    public static IEnumerable<object[]> NonPrivilegedRoleData()
    {
        foreach (var role in NonPrivilegedRoles)
            yield return new object[] { role };
    }
}
