namespace OrganisationRegistry.Api.Edit;

using System;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api.Exceptions;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure.Commands;

public abstract class EditApiController : Controller
{
    protected ICommandSender CommandSender { get; }

    protected EditApiController(ICommandSender commandSender)
    {
        CommandSender = commandSender;
    }

    [NonAction]
    public override bool TryValidateModel(object model)
        => ModelState.IsValid && base.TryValidateModel(model);

    [NonAction]
    protected OkResult OkWithLocationHeader(string action, object? parameters)
    {
        Response.Headers.Add("Location", Request.Path.ToString());
        return Ok();
    }

    [NonAction]
    protected OkObjectResult OkValueWithLocationHeader(string uri, object? value)
    {
        Response.Headers.Add("Location", uri);
        return Ok(value);
    }

    [NonAction]
    protected CreatedResult CreatedWithLocation(string action, object? parameters)
    {
        var locationHeader = BuildCreatedLocationHeader(parameters);
        return Created(locationHeader, null);
    }

    [NonAction]
    protected CreatedResult CreatedWithLocation(string controller, string action, object? parameters)
    {
        var locationHeader = BuildCreatedLocationHeader(parameters);
        return Created(locationHeader, null);
    }

    // Url.Action faalt met Asp.Versioning 10 + convention-based routing op verschillende plekken
    // (ambiguous action names, ontbrekende route values). Bij een POST is de resource-locatie de
    // huidige request path met de nieuwe id erachter.
    private string BuildCreatedLocationHeader(object? parameters)
    {
        var id = parameters?.GetType().GetProperty("id")?.GetValue(parameters);
        return id is not null
            ? $"{Request.Path}/{id}"
            : Request.Path.ToString();
    }

    [NonAction]
    protected Task<IActionResult> OkAsync(object? value)
        => Task.FromResult((IActionResult)Ok(value));

    [NonAction]
    protected Task<IActionResult> CreatedAsync(string uri, object? value)
        => Task.FromResult((IActionResult)Created(uri, value));

    [NonAction]
    protected Task<IActionResult> CreatedAsync(Uri uri, object? value)
        => Task.FromResult((IActionResult)Created(uri, value));

    [NonAction]
    protected Task<IActionResult> ContentAsync(string value)
        => Task.FromResult((IActionResult)Content(value));

    [NonAction]
    [Obsolete("Use Request.Path or a hardcoded route instead of Url.Action")]
    protected string? Action<T>(string actionName, object? parameters = null)
        where T : Controller
    {
        var id = parameters?.GetType().GetProperty("id")?.GetValue(parameters);
        return id is not null
            ? $"{Request.Path}/{id}"
            : Request.Path.ToString();
    }
}
