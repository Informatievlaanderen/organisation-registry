namespace OrganisationRegistry.Infrastructure.Infrastructure;

public static class PrivateReflectionDynamicObjectExtensions
{
    public static dynamic AsDynamic(this object o)
        => PrivateReflectionDynamicObject.WrapObjectIfNeeded(o)!;
}
