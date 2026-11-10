namespace OrganisationRegistry.Api.Security;

using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using IdentityModel.AspNetCore.OAuth2Introspection;
using Microsoft.AspNetCore.Http;
using Serilog;

/// <summary>
/// Logs every step of an introspection scheme without ever logging the token or a secret.
/// A token is identified by a short hash so one token can be followed across schemes and requests.
/// </summary>
public static class IntrospectionLogging
{
    private static readonly ILogger Logger = Log.ForContext(typeof(IntrospectionLogging));

    public static OAuth2IntrospectionEvents Create(string scheme, string failedLabel, string validatedLabel) => new()
    {
        // 1. Only fires when the library really calls ACM (not on a cache hit)
        OnSendingRequest = ctx =>
        {
            ctx.HttpContext.Items[Key(scheme)] = Stopwatch.StartNew();
            Logger.Debug(
                "[{Scheme}] SendingRequest: calling ACM introspection. {Method} {Path} token={Token} trace={Trace}",
                scheme, ctx.HttpContext.Request.Method, ctx.HttpContext.Request.Path, Fingerprint(ctx.HttpContext), Trace(ctx.HttpContext));
            return Task.CompletedTask;
        },

        // 2. Only fires when the client authenticates with a client_assertion (JWT), not with a secret
        OnUpdateClientAssertion = ctx =>
        {
            Logger.Debug("[{Scheme}] UpdateClientAssertion: client assertion is being refreshed", scheme);
            return Task.CompletedTask;
        },

        // 3. The token was accepted: by ACM (introspected=true) or from the cache (introspected=false)
        OnTokenValidated = ctx =>
        {
            var introspected = TryElapsed(ctx.HttpContext, scheme, out var ms);
            var principal = ctx.Principal;
            Logger.Debug(
                "{Label}: scheme={Scheme} introspected={Introspected} acmMs={AcmMs} token={Token} subject={Subject} client={Client} scopes=[{Scopes}] claimTypes=[{ClaimTypes}] path={Path} trace={Trace}",
                validatedLabel, scheme, introspected, ms, Fingerprint(ctx.HttpContext),
                principal?.FindFirst("sub")?.Value, principal?.FindFirst("client_id")?.Value,
                string.Join(" ", principal?.FindAll("scope").Select(c => c.Value) ?? []),
                string.Join(",", principal?.Claims.Select(c => c.Type).Distinct() ?? []),
                ctx.HttpContext.Request.Path, Trace(ctx.HttpContext));

            // full claim values (names, vo_id) only when Debug is enabled for this class
            Logger.Debug("[{Scheme}] claims: {@Claims}", scheme, principal?.Claims.Select(c => new { c.Type, c.Value }));
            return Task.CompletedTask;
        },

        // 4. The token was rejected (inactive, expired, introspection failed)
        OnAuthenticationFailed = ctx =>
        {
            var introspected = TryElapsed(ctx.HttpContext, scheme, out var ms);
            Logger.Debug(
                "{Label}: scheme={Scheme} error={Error} introspected={Introspected} acmMs={AcmMs} token={Token} path={Path} trace={Trace}",
                failedLabel, scheme, ctx.Error, introspected, ms, Fingerprint(ctx.HttpContext),
                ctx.HttpContext.Request.Path, Trace(ctx.HttpContext));
            return Task.CompletedTask;
        },
    };

    private static string Key(string scheme) => "introspection.timer." + scheme;

    private static bool TryElapsed(HttpContext http, string scheme, out long ms)
    {
        ms = -1;
        if (!http.Items.TryGetValue(Key(scheme), out var o) || o is not Stopwatch sw)
            return false;
        ms = sw.ElapsedMilliseconds;
        return true;
    }

    private static string Trace(HttpContext http) => Activity.Current?.TraceId.ToString() ?? http.TraceIdentifier;

    // first 8 hex chars of SHA-256 of the bearer token: enough to correlate, useless to an attacker
    private static string Fingerprint(HttpContext http)
    {
        var header = http.Request.Headers.Authorization.ToString();
        if (!header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)) return "none";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(header["Bearer ".Length..].Trim()));
        return Convert.ToHexString(hash, 0, 4).ToLowerInvariant();
    }
}
