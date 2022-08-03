namespace OrganisationRegistry.Handling;

using Authorization;
using Infrastructure.Authorization;
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

    public static Handler WithRegisterBodyPolicy(this Handler source, OrganisationId? organisationId)
        => source.WithPolicy(new RegisterBodyPolicy(organisationId));

    public static Handler RequiresOneOfRole(this Handler source, params Role[] roles)
        => source.WithPolicy(new RequiresRolesPolicy(roles));

    public static Handler WithAddBodyPolicy(this Handler source)
        => source.WithPolicy(new AddBodyPolicy());
}
