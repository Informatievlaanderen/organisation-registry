namespace OrganisationRegistry.Api.Infrastructure.Configuration
{
    using System;
    using System.Net;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Diagnostics;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using Newtonsoft.Json;
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
                    else
                    {
                        await HandleUnhandledException(error?.Error, context, logger);
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

            context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
            await context.Response.WriteAsync(JsonConvert.SerializeObject(new BasicApiProblem
            {
                HttpStatus = context.Response.StatusCode.ToString(),
                Title = "Er heeft zich een fout voorgedaan!",
                Detail = message,
                Reference = exceptionNumber
            })).ConfigureAwait(false);
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
                Reference = exceptionNumber
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
                Reference = exceptionNumber
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
                Reference = exceptionNumber
            })).ConfigureAwait(false);
        }

        private static string GetExceptionNumber()
            => $"{Guid.NewGuid():N}";
    }
}
