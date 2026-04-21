namespace OrganisationRegistry.UnitTests.Security;

using System;
using System.Reflection;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Api.Infrastructure;
using Api.Infrastructure.Security;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OrganisationRegistry.Infrastructure.Authorization;
using Xunit;
using Schemes = Api.Infrastructure.Security.AuthenticationSchemes;

public class AuthenticationSetupTests
{
    [Fact]
    public async Task BackofficeUserPolicy_AllowsTokenExchangeJwtBearerUser()
    {
        var services = CreateServices(
            bffApiEnabled: true,
            jwtPrincipal: UserPrincipal(Schemes.JwtBearer, new Claim(AcmIdmConstants.Claims.AcmId, "user-1")));

        var result = await AuthorizeAsync(services, PolicyNames.BackofficeUser);

        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public async Task BackofficeUserPolicy_AllowsBffApiUser_WhenBffApiIsEnabled()
    {
        var services = CreateServices(
            bffApiEnabled: true,
            bffPrincipal: UserPrincipal(Schemes.BffApi, new Claim(AcmIdmConstants.Claims.AcmId, "user-2")));

        var result = await AuthorizeAsync(services, PolicyNames.BackofficeUser);

        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public async Task BackofficeUserPolicy_RejectsBffApiUser_WhenBffApiIsDisabled()
    {
        var services = CreateServices(
            bffApiEnabled: false,
            bffPrincipal: UserPrincipal(Schemes.BffApi, new Claim(AcmIdmConstants.Claims.AcmId, "user-3")));

        var result = await AuthorizeAsync(services, PolicyNames.BackofficeUser);

        result.Succeeded.Should().BeFalse();
        result.Challenged.Should().BeTrue();
    }

    [Fact]
    public async Task BackofficeUserPolicy_RejectsMachineToMachineIntrospectionToken()
    {
        var services = CreateServices(
            bffApiEnabled: true,
            introspectionPrincipal: UserPrincipal(
                Schemes.Introspection,
                new Claim(AcmIdmConstants.Claims.Scope, AcmIdmConstants.Scopes.CjmBeheerder)));

        var result = await AuthorizeAsync(services, PolicyNames.BackofficeUser);

        result.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task EditApiPolicy_AllowsIntrospectionClientWithMatchingScope()
    {
        var services = CreateServices(
            bffApiEnabled: true,
            introspectionPrincipal: UserPrincipal(
                Schemes.Introspection,
                new Claim(AcmIdmConstants.Claims.Scope, AcmIdmConstants.Scopes.CjmBeheerder)));

        var result = await AuthorizeAsync(services, PolicyNames.Organisations, Schemes.EditApi);

        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public async Task EditApiPolicy_RejectsJwtBearerUserToken()
    {
        var services = CreateServices(
            bffApiEnabled: true,
            jwtPrincipal: UserPrincipal(Schemes.JwtBearer, new Claim(AcmIdmConstants.Claims.AcmId, "user-4")));

        var result = await AuthorizeAsync(services, PolicyNames.Organisations, Schemes.EditApi);

        result.Succeeded.Should().BeFalse();
    }

    [Fact]
    public async Task GetAuthenticateInfoAsync_ReturnsJwtBearerPrincipal_WhenJwtBearerSucceeds()
    {
        var principal = UserPrincipal(Schemes.JwtBearer, new Claim(AcmIdmConstants.Claims.AcmId, "user-5"));
        var services = CreateServices(bffApiEnabled: true, jwtPrincipal: principal);
        var httpContext = CreateHttpContext(services);

        var result = await httpContext.GetAuthenticateInfoAsync();

        result.Should().NotBeNull();
        result!.Succeeded.Should().BeTrue();
        result.Principal.Should().BeSameAs(principal);
    }

    [Fact]
    public async Task GetAuthenticateInfoAsync_FallsBackToBffApi_WhenJwtBearerDoesNotAuthenticate()
    {
        var principal = UserPrincipal(Schemes.BffApi, new Claim(AcmIdmConstants.Claims.AcmId, "user-6"));
        var services = CreateServices(bffApiEnabled: true, bffPrincipal: principal);
        var httpContext = CreateHttpContext(services);

        var result = await httpContext.GetAuthenticateInfoAsync();

        result.Should().NotBeNull();
        result!.Succeeded.Should().BeTrue();
        result.Principal.Should().BeSameAs(principal);
    }

    [Fact]
    public async Task GetAuthenticateInfoAsync_ReturnsNull_WhenBffApiIsNotRegistered()
    {
        var services = CreateServices(bffApiEnabled: false);
        var httpContext = CreateHttpContext(services);

        var result = await httpContext.GetAuthenticateInfoAsync();

        result.Should().BeNull();
    }

    private static ServiceProvider CreateServices(
        bool bffApiEnabled,
        ClaimsPrincipal? jwtPrincipal = null,
        ClaimsPrincipal? bffPrincipal = null,
        ClaimsPrincipal? introspectionPrincipal = null)
    {
        var services = new ServiceCollection();

        services
            .AddLogging()
            .AddOptions()
            .AddAuthentication(options =>
            {
                options.DefaultScheme = Schemes.JwtBearer;
                options.DefaultAuthenticateScheme = Schemes.JwtBearer;
                options.DefaultChallengeScheme = Schemes.JwtBearer;
            })
            .AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                Schemes.JwtBearer,
                options => options.Principal = jwtPrincipal)
            .AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                Schemes.Introspection,
                options => options.Principal = introspectionPrincipal);

        if (bffApiEnabled)
        {
            services
                .AddAuthentication()
                .AddScheme<TestAuthenticationSchemeOptions, TestAuthenticationHandler>(
                    Schemes.BffApi,
                    options => options.Principal = bffPrincipal);
        }

        services.AddAuthorization(ConfigureAuthPolicies(bffApiEnabled));

        return services.BuildServiceProvider();
    }

    private static DefaultHttpContext CreateHttpContext(IServiceProvider services)
        => new() { RequestServices = services };

    private static ClaimsPrincipal UserPrincipal(string authenticationType, params Claim[] claims)
        => new(new ClaimsIdentity(claims, authenticationType));

    private static async Task<PolicyAuthorizationResult> AuthorizeAsync(
        IServiceProvider services,
        string policyName,
        params string[] authenticationSchemes)
    {
        var httpContext = CreateHttpContext(services);
        var authorizationOptions = services.GetRequiredService<IOptions<AuthorizationOptions>>();
        var basePolicy = authorizationOptions.Value.GetPolicy(policyName)
                         ?? throw new InvalidOperationException($"Missing policy {policyName}");
        var policy = BuildPolicy(basePolicy, authenticationSchemes);
        var policyEvaluator = services.GetRequiredService<IPolicyEvaluator>();

        var authenticateResult = await policyEvaluator.AuthenticateAsync(policy, httpContext);
        return await policyEvaluator.AuthorizeAsync(policy, authenticateResult, httpContext, resource: null);
    }

    private static AuthorizationPolicy BuildPolicy(
        AuthorizationPolicy basePolicy,
        params string[] authenticationSchemes)
    {
        if (authenticationSchemes.Length == 0)
            return basePolicy;

        var builder = new AuthorizationPolicyBuilder()
            .AddAuthenticationSchemes(authenticationSchemes);

        foreach (var requirement in basePolicy.Requirements)
            builder.Requirements.Add(requirement);

        return builder.Build();
    }

    private static Action<AuthorizationOptions> ConfigureAuthPolicies(bool bffApiEnabled)
    {
        var method = typeof(Startup).GetMethod(
            "ConfigureAuthPolicies",
            BindingFlags.NonPublic | BindingFlags.Static);

        return (Action<AuthorizationOptions>)method!.Invoke(null, new object[] { bffApiEnabled })!;
    }

    private sealed class TestAuthenticationSchemeOptions : AuthenticationSchemeOptions
    {
        public ClaimsPrincipal? Principal { get; set; }
    }

    private sealed class TestAuthenticationHandler : AuthenticationHandler<TestAuthenticationSchemeOptions>
    {
        public TestAuthenticationHandler(
            IOptionsMonitor<TestAuthenticationSchemeOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock)
            : base(options, logger, encoder, clock)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            if (Options.Principal == null)
                return Task.FromResult(AuthenticateResult.NoResult());

            var ticket = new AuthenticationTicket(Options.Principal, Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
