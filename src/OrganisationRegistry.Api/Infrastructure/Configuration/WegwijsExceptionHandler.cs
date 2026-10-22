namespace OrganisationRegistry.Api.Infrastructure.Configuration;

using System;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Organisation.Exceptions;
using OrganisationRegistry.Infrastructure.Domain.Exception;

public class OrganisationRegistryApiExceptionHandler { }

public static class OrganisationRegistryExceptionHandler
{
    public static IApplicationBuilder UseOrganisationRegistryExceptionHandler(this IApplicationBuilder app, ILoggerFactory loggerFactory)
    {
        var logger = loggerFactory.CreateLogger<OrganisationRegistryApiExceptionHandler>();

        app.UseExceptionHandler(builder =>
        {
            builder.UseCors("AllowSpecificOrigin");
            builder.Run(async context =>
            {
                context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var error = context.Features.Get<IExceptionHandlerFeature>();
                var exception = error?.Error;

                // Errors happening in the Apply() stuff result in an InvocationException due to the dynamic stuff.
                if (exception is TargetInvocationException && exception.InnerException != null)
                    exception = exception.InnerException;

                if (exception is DomainException)
                {
                    await HandleDomainException(exception, context, logger);
                }
                else if (exception is ApiException)
                {
                    await HandleApiException(exception, context, logger);
                }
                else if (exception is AggregateNotFoundException aggregateNotFoundException)
                {
                    await HandleNotFoundException(aggregateNotFoundException, context, logger);
                }
                else if (exception is ConcurrencyException concurrencyException)
                {
                    // Thrown when a client-supplied id for a brand new aggregate
                    // (e.g. POST /organisations, POST /bodies) already has events
                    // in the store. This is a client input problem (the resource
                    // already exists), not a server fault, so it maps to 409
                    // Conflict rather than falling through to 500.
                    await HandleConcurrencyException(concurrencyException, context, logger);
                }
                else if (TryGetDuplicateKeyViolation(exception, out var duplicateKeyException))
                {
                    // Thrown when a client-supplied id for a sub-entity of an
                    // existing aggregate (e.g. a label, key, or capacity on an
                    // organisation) collides with an existing row in a SQL
                    // projection table. Same rationale as ConcurrencyException
                    // above: the resource already exists, so this maps to 409
                    // Conflict.
                    await HandleDuplicateKeyException(duplicateKeyException!, context, logger);
                }
                else
                {
                    // Use the already-unwrapped `exception` (not `error?.Error`)
                    // so a TargetInvocationException's real inner cause is what
                    // gets logged, not the invocation wrapper.
                    await HandleUnhandledException(exception, context, logger);
                }
            });
        });

        return app;
    }

    private static async Task HandleDomainException(Exception exception, HttpContext context, ILogger<OrganisationRegistryApiExceptionHandler> logger)
    {
        var message = exception.Message;
        var exceptionNumber = GetExceptionNumber();
        logger.LogInformation(0, exception, "[{ErrorNumber}] DomainException handled: {ExceptionMessage}", exceptionNumber, message);

        var statusCode = IsInsufficientRights(exception)
            ? HttpStatusCode.Forbidden
            : HttpStatusCode.BadRequest;

        context.Response.StatusCode = (int) statusCode;
        await context.Response.WriteAsync(JsonConvert.SerializeObject(new BasicApiProblem
        {
            HttpStatus = context.Response.StatusCode.ToString(),
            Title = "Er heeft zich een fout voorgedaan!",
            Detail = message,
            Reference = exceptionNumber,
        })).ConfigureAwait(false);
    }

    private static bool IsInsufficientRights(Exception exception)
    {
        var exceptionType = exception.GetType();
        return exceptionType.IsGenericType &&
               exceptionType.GetGenericTypeDefinition() == typeof(OrganisationRegistry.Organisation.Exceptions.InsufficientRights<>);
    }

    private static async Task HandleApiException(Exception exception, HttpContext context, ILogger<OrganisationRegistryApiExceptionHandler> logger)
    {
        var message = exception.Message;
        var exceptionNumber = GetExceptionNumber();
        logger.LogInformation(0, exception, "[{ErrorNumber}] ApiException handled: {ExceptionMessage}", exceptionNumber, message);

        context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
        await context.Response.WriteAsync(JsonConvert.SerializeObject(new BasicApiProblem
        {
            HttpStatus = context.Response.StatusCode.ToString(),
            Title = "Er heeft zich een fout voorgedaan!",
            Detail = message,
            Reference = exceptionNumber,
        })).ConfigureAwait(false);
    }

    private static async Task HandleNotFoundException(AggregateNotFoundException exception, HttpContext context, ILogger<OrganisationRegistryApiExceptionHandler> logger)
    {
        var message = $"De resource met id '{exception.Id}' werd niet gevonden.";
        var exceptionNumber = GetExceptionNumber();
        logger.LogInformation(0, exception, "[{ErrorNumber}] NotFoundException handled: {ExceptionMessage}", exceptionNumber, message);

        context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
        await context.Response.WriteAsync(JsonConvert.SerializeObject(new BasicApiProblem
        {
            HttpStatus = context.Response.StatusCode.ToString(),
            Title = "Deze actie is niet geldig",
            Detail = message,
            Reference = exceptionNumber,
        })).ConfigureAwait(false);
    }

    private static async Task HandleConcurrencyException(ConcurrencyException exception, HttpContext context, ILogger<OrganisationRegistryApiExceptionHandler> logger)
    {
        // ConcurrencyException is thrown both for a brand new aggregate whose
        // client-supplied id already has events (a duplicate-create), and for
        // a genuine optimistic-concurrency conflict on an existing aggregate
        // (e.g. two users updating the same organisation at once). The message
        // must therefore stay generic rather than assuming "duplicate identifier".
        var message = "De resource werd ondertussen gewijzigd of bestaat reeds.";
        var exceptionNumber = GetExceptionNumber();
        logger.LogInformation(0, exception, "[{ErrorNumber}] ConcurrencyException handled: {ExceptionMessage}", exceptionNumber, exception.Message);

        context.Response.StatusCode = (int) HttpStatusCode.Conflict;
        await context.Response.WriteAsync(JsonConvert.SerializeObject(new BasicApiProblem
        {
            HttpStatus = context.Response.StatusCode.ToString(),
            Title = "Er heeft zich een fout voorgedaan!",
            Detail = message,
            Reference = exceptionNumber,
        })).ConfigureAwait(false);
    }

    /// <summary>
    /// SQL Server error 2627 ("Violation of PRIMARY KEY constraint") and 2601
    /// ("Cannot insert duplicate key row in object ... with unique index") both
    /// indicate a client-supplied identifier collided with an existing row.
    /// </summary>
    private const int SqlPrimaryKeyViolation = 2627;
    private const int SqlUniqueIndexViolation = 2601;

    private static bool TryGetDuplicateKeyViolation(Exception? exception, out DbUpdateException? duplicateKeyException)
    {
        duplicateKeyException = null;

        if (exception is not DbUpdateException dbUpdateException)
            return false;

        if (dbUpdateException.InnerException is not SqlException sqlException)
            return false;

        if (sqlException.Number != SqlPrimaryKeyViolation && sqlException.Number != SqlUniqueIndexViolation)
            return false;

        duplicateKeyException = dbUpdateException;
        return true;
    }

    private static async Task HandleDuplicateKeyException(DbUpdateException exception, HttpContext context, ILogger<OrganisationRegistryApiExceptionHandler> logger)
    {
        // Error 2601 is not limited to the sub-entity's own id column - it can
        // also fire on an unrelated business unique index (e.g. OvoNumber) in
        // the same projection table under a race condition. Keep the message
        // generic rather than assuming it's always the identifier that collided.
        var message = "De resource werd ondertussen gewijzigd of bestaat reeds.";
        var exceptionNumber = GetExceptionNumber();
        logger.LogInformation(0, exception, "[{ErrorNumber}] Duplicate key DbUpdateException handled: {ExceptionMessage}", exceptionNumber, exception.Message);

        context.Response.StatusCode = (int) HttpStatusCode.Conflict;
        await context.Response.WriteAsync(JsonConvert.SerializeObject(new BasicApiProblem
        {
            HttpStatus = context.Response.StatusCode.ToString(),
            Title = "Er heeft zich een fout voorgedaan!",
            Detail = message,
            Reference = exceptionNumber,
        })).ConfigureAwait(false);
    }

    private static async Task HandleUnhandledException(Exception? exception, HttpContext context, ILogger<OrganisationRegistryApiExceptionHandler> logger)
    {
        var exceptionNumber = GetExceptionNumber();

        if (exception != null)
            logger.LogError(0, exception, "[{ErrorNumber}] Unhandled exception!", exceptionNumber);

        context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
        await context.Response.WriteAsync(JsonConvert.SerializeObject(new BasicApiProblem
        {
            HttpStatus = context.Response.StatusCode.ToString(),
            Title = "Er heeft zich een fout voorgedaan!",
            Detail = string.Empty,
            Reference = exceptionNumber,
        })).ConfigureAwait(false);
    }

    private static string GetExceptionNumber()
        => $"{Guid.NewGuid():N}";
}
