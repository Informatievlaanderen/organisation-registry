namespace OrganisationRegistry.Api.Infrastructure;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

public class OperationCancelledExceptionFilter : ExceptionFilterAttribute
{
    private readonly ILogger _logger;

    public OperationCancelledExceptionFilter(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<OperationCancelledExceptionFilter>();
    }
    public override void OnException(ExceptionContext context)
    {
        if (context.Exception is not TaskCanceledException || context.Exception.InnerException is not TaskCanceledException) return;
        _logger.LogInformation("Request was cancelled");
        context.ExceptionHandled = true;
        context.Result = new StatusCodeResult(499);
    }
}
