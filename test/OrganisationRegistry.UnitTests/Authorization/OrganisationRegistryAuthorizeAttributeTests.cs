namespace OrganisationRegistry.UnitTests.Authorization;

using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using OrganisationRegistry.Api.Infrastructure.Security;
using OrganisationRegistry.Infrastructure.Authorization;
using Xunit;

public class OrganisationRegistryAuthorizeAttributeTests
{
    private static AuthorizationFilterContext BuildContext(IUser user)
    {
        var securityService = new Mock<ISecurityService>();
        securityService
            .Setup(s => s.GetRequiredUser(It.IsAny<ClaimsPrincipal>()))
            .ReturnsAsync(user);

        var services = new ServiceCollection();
        services.AddSingleton(securityService.Object);

        var httpContext = new DefaultHttpContext
        {
            RequestServices = services.BuildServiceProvider(),
            User = new ClaimsPrincipal(new ClaimsIdentity("test")),
        };

        var actionContext = new ActionContext(
            httpContext,
            new RouteData(),
            new ActionDescriptor());

        return new AuthorizationFilterContext(actionContext, new List<IFilterMetadata>());
    }

    private static IUser UserWith(PermissionSet permissions)
    {
        var mock = new Mock<IUser>();
        mock.SetupGet(u => u.Permissions).Returns(permissions);
        mock.Setup(u => u.HasAnyPermission(It.IsAny<Permission[]>()))
            .Returns<Permission[]>(ps =>
            {
                foreach (var p in ps)
                    if (permissions.Contains(p))
                        return true;
                return false;
            });
        mock.Setup(u => u.HasPermission(It.IsAny<Permission>()))
            .Returns<Permission>(permissions.Contains);
        return mock.Object;
    }

    [Fact]
    public async Task Attribute_with_CanEditAll_short_circuits_and_allows()
    {
        var user = UserWith(PermissionSet.Of(Permission.CanEditAll));
        var context = BuildContext(user);
        var attribute = new OrganisationRegistryAuthorizeAttribute
        {
            RequiredPermissions = new[] { Permission.CanManageKeys },
        };

        await ((IAsyncAuthorizationFilter)attribute).OnAuthorizationAsync(context);

        context.Result.Should().BeNull();
    }

    [Fact]
    public async Task Attribute_allows_when_identity_has_any_required_permission()
    {
        var user = UserWith(PermissionSet.Of(Permission.CanManageKeys, Permission.CanAddLocations));
        var context = BuildContext(user);
        var attribute = new OrganisationRegistryAuthorizeAttribute
        {
            RequiredPermissions = new[] { Permission.CanManageKeys },
        };

        await ((IAsyncAuthorizationFilter)attribute).OnAuthorizationAsync(context);

        context.Result.Should().BeNull();
    }

    [Fact]
    public async Task Attribute_allows_when_any_of_multiple_required_permissions_matches()
    {
        var user = UserWith(PermissionSet.Of(Permission.CanEditBodies));
        var context = BuildContext(user);
        var attribute = new OrganisationRegistryAuthorizeAttribute
        {
            RequiredPermissions = new[] { Permission.CanRegisterBodies, Permission.CanEditBodies },
        };

        await ((IAsyncAuthorizationFilter)attribute).OnAuthorizationAsync(context);

        context.Result.Should().BeNull();
    }

    [Fact]
    public async Task Attribute_forbids_when_no_required_permission_matches()
    {
        var user = UserWith(PermissionSet.Of(Permission.CanReadOrafin));
        var context = BuildContext(user);
        var attribute = new OrganisationRegistryAuthorizeAttribute
        {
            RequiredPermissions = new[] { Permission.CanManageKeys },
        };

        await ((IAsyncAuthorizationFilter)attribute).OnAuthorizationAsync(context);

        context.Result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task Attribute_forbids_when_identity_has_empty_permission_set()
    {
        var user = UserWith(PermissionSet.Empty);
        var context = BuildContext(user);
        var attribute = new OrganisationRegistryAuthorizeAttribute
        {
            RequiredPermissions = new[] { Permission.CanManageKeys },
        };

        await ((IAsyncAuthorizationFilter)attribute).OnAuthorizationAsync(context);

        context.Result.Should().BeOfType<ForbidResult>();
    }

    [Fact]
    public async Task Attribute_without_RequiredPermissions_does_not_enforce_permissions()
    {
        // Parameterless usage: policy-only (authentication), no permission gate.
        var user = UserWith(PermissionSet.Empty);
        var context = BuildContext(user);
        var attribute = new OrganisationRegistryAuthorizeAttribute();

        await ((IAsyncAuthorizationFilter)attribute).OnAuthorizationAsync(context);

        context.Result.Should().BeNull();
    }
}
