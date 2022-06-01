namespace OrganisationRegistry.UnitTests.SecurityPolicy;

using FluentAssertions;
using Handling.Authorization;

public static class AuthorizationResultTestExtensions{
    public static void ShouldFailWith<T>(this AuthorizationResult source)
    {
        source.IsSuccessful.Should().BeFalse();
        source.Exception.Should().BeOfType<T>();
    }
}
