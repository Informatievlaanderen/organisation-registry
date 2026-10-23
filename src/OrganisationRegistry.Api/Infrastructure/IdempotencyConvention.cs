namespace OrganisationRegistry.Api.Infrastructure;

using System;
using System.Linq;
using IdempotentAPI.Filters;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

/// <summary>
/// Automatically applies <see cref="IdempotentAttribute"/> (idempotency-key
/// support, see <see cref="IdempotencyConstants"/>) to every mutating (POST or
/// PATCH) action on a command controller, instead of requiring it to be added
/// by hand on every action. An action can still opt out or use a different
/// configuration by carrying its own <see cref="IdempotentAttribute"/>: this
/// convention only adds one when the action doesn't already have one.
///
/// Note: IdempotentAPI only ever acts on POST and PATCH requests (it skips
/// everything else internally), so PUT and DELETE actions are intentionally
/// not touched here - the attribute would be a no-op for them.
/// </summary>
public sealed class IdempotencyConvention : IApplicationModelConvention
{
    private static readonly string[] MutatingMethods = { "POST", "PATCH" };

    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            if (!typeof(OrganisationRegistryCommandController).IsAssignableFrom(controller.ControllerType))
                continue;

            foreach (var action in controller.Actions)
                ApplyToAction(action);
        }
    }

    private static void ApplyToAction(ActionModel action)
    {
        var httpMethods = action.Selectors
            .SelectMany(selector => selector.ActionConstraints?.OfType<HttpMethodActionConstraint>() ?? [])
            .SelectMany(constraint => constraint.HttpMethods);

        if (!httpMethods.Any(method => MutatingMethods.Contains(method, StringComparer.OrdinalIgnoreCase)))
            return;

        // An explicit attribute on the action wins (e.g. a different expiry).
        if (action.Attributes.OfType<IdempotentAttribute>().Any())
            return;

        action.Filters.Add(new IdempotentAttribute
        {
            HeaderKeyName = IdempotencyConstants.HeaderName,
            IsIdempotencyOptional = true,
            ExpireHours = IdempotencyConstants.ExpireHours,
        });
    }
}
