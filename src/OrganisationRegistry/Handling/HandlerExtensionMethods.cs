namespace OrganisationRegistry.Handling;

using Authorization;
using Infrastructure.Authorization;
using Organisation;

public static class HandlerExtensionMethods
{
    public static Handler WithChildPolicy(this Handler source, Organisation organisation)
        => source.WithPolicy(
            new ChildPolicy(
                organisation.State.OvoNumber,
                organisation.State.UnderVlimpersManagement));


    public static Handler WithRegisterBodyPolicy(this Handler source, OrganisationId? organisationId)
        => source.WithPolicy(new RegisterBodyPolicy(organisationId));

    public static Handler RequiresOneOfRole(this Handler source, params Role[] roles)
        => source.WithPolicy(new RequiresRolesPolicy(roles));

    public static Handler RequiresPermission(this Handler source, Permission permission)
        => source.WithPolicy(new RequiresPermissionPolicy(permission));

    public static Handler WithBodyPolicy(this Handler source, Permission permission, System.Guid bodyId)
        => source.WithPolicy(new BodyPolicy(permission, bodyId));

    public static Handler WithPeoplePolicy(this Handler source)
        => source.WithPolicy(new PeoplePolicy());
}
