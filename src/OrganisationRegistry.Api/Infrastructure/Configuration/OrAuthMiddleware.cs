namespace OrganisationRegistry.Api.Infrastructure.Configuration;

using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Auth.CustomAttributes;
using Auth.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Authorization;
using ApiException = Be.Vlaanderen.Basisregisters.Api.Exceptions.ApiException;

public class OrAuthMiddleware(RequestDelegate next)
{
    public async Task Invoke(HttpContext ctx,
        ISecurityService securityService,
        ILogger<OrAuthMiddleware> logger)
    {
        var path = ctx.Request.Path;
        var endpoint = ctx.GetEndpoint()?.Metadata.GetMetadata<GlobalResource>();

        if (endpoint is null)
        {
            logger.LogInformation("AUTH no GlobalResource metadata on endpoint for {Path} -> skipping auth", path);
            await next.Invoke(ctx);
            return;
        }

        // Log the raw claims so you can see exactly what's on the principal
        // logger.LogInformation(
        //     "AUTH ClaimsPrincipal for {Path}: IsAuthenticated={IsAuthenticated}, AuthType={AuthType}, Claims={@Claims}",
        //     path,
        //     ctx.User?.Identity?.IsAuthenticated,
        //     ctx.User?.Identity?.AuthenticationType,
        //     ctx.User?.Claims.Select(c => new { c.Type, c.Value }));

        var user = await GetUser(ctx, securityService, logger);

        if (user is null)
        {
            await HandleApiException("De gebruiker is niet geauthenticeerd.", HttpStatusCode.Unauthorized, ctx, logger);
            return;
        }

        if (user == WellknownUsers.Nobody)
        {
            await HandleApiException("De gebruiker beschikt niet over een geldige Wegwijs-rol.", HttpStatusCode.Forbidden, ctx, logger);
            return;
        }

        if (!user.Roles.Any())
        {
            logger.LogWarning("AUTH FAIL - user {UserId} has NO roles for {Path}, First() will throw", user.UserId, path);
        }

        var role = user.Roles.First();

        var hasAccess = RolePermissions.Validate(role, endpoint.Resource, endpoint.Operations, logger);

        if (!hasAccess)
        {
            await HandleApiException("De gebruiker beschikt niet over een geldige Wegwijs-rol.", HttpStatusCode.Forbidden, ctx, logger);
            return;
        }




        await next.Invoke(ctx);
    }

    private async Task<IUser?> GetUser(HttpContext ctx,
        ISecurityService securityService,
        ILogger<OrAuthMiddleware> logger)
    {
        try
        {
            var user = await securityService.GetUser(ctx.User);
            logger.LogInformation("AUTH securityService.GetUser returned {@User}", user);
            return user;
        }
        catch (Exception ex)
        {
            // This was silently swallowing the real error before - now you'll see it
            logger.LogError(ex, "AUTH securityService.GetUser THREW an exception");
            return null;
        }
    }

    private static async Task HandleApiException(string message, HttpStatusCode statuscode, HttpContext context, ILogger<OrAuthMiddleware> logger)
    {
        var exceptionNumber = GetExceptionNumber();
        logger.LogInformation(0, "[{ErrorNumber}] ApiException handled: {ExceptionMessage}", exceptionNumber, message);

        context.Response.StatusCode = (int)statuscode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(Newtonsoft.Json.JsonConvert.SerializeObject(new BasicApiProblem
        {
            HttpStatus = context.Response.StatusCode.ToString(),
            Title = "U heeft niet de juiste rechten!",
            Detail = message,
            Reference = exceptionNumber,
        })).ConfigureAwait(false);
    }

    private static string GetExceptionNumber() => $"{Guid.NewGuid():N}";
}
