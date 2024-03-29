﻿namespace OrganisationRegistry.Api.Edit;

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
        var maybeLocationHeader = Url.Action(action, parameters);
        if (maybeLocationHeader is not { } locationHeader)
            throw new ApiException($"Action {action} does not exist");

        Response.Headers.Add("Location", locationHeader);
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
        var maybeLocationHeader = Url.Action(action, parameters);
        if (maybeLocationHeader is not { } locationHeader)
            throw new ApiException($"Action {action} does not exist");

        return Created(locationHeader, null);
    }

    [NonAction]
    protected CreatedResult CreatedWithLocation(string controller, string action, object? parameters)
    {
        if (controller.EndsWith("Controller"))
            controller = controller.Replace("Controller", string.Empty);

        var maybeLocationHeader = Url.Action(action, controller, parameters);
        if (maybeLocationHeader is not { } locationHeader)
            throw new ApiException($"Action {action} does not exist");

        return Created(locationHeader, null);
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

    protected string? Action<T>(string actionName, object? parameters = null)
        where T : Controller
    {
        var name = typeof(T).Name;
        var controllerName = name.EndsWith("Controller")
            ? name[..^10] : name;
        return Url.Action(actionName, controllerName, parameters);
    }
}
