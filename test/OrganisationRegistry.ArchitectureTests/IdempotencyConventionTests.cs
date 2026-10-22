namespace OrganisationRegistry.ArchitectureTests;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IdempotentAPI.Filters;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Routing;
using OrganisationRegistry.Api.Infrastructure;
using Xunit;

/// <summary>
/// OR-3539: idempotency-key support (see <see cref="IdempotencyConvention"/>) is
/// wired up once, globally, instead of being applied by hand to every action.
/// These tests exercise the convention directly against every real command
/// controller/action in the Api assembly - via the actual MVC
/// <see cref="ApplicationModel"/> types, not a running server - to prove every
/// mutating (POST/PATCH) action actually receives the
/// <see cref="IdempotentAttribute"/> filter, and that non-mutating/PUT/DELETE
/// actions are left untouched (the library no-ops on those methods anyway).
/// </summary>
public class IdempotencyConventionTests
{
    private static readonly Type[] CommandControllerTypes = typeof(OrganisationRegistryController)
        .Assembly
        .GetTypes()
        .Where(t => t is { IsClass: true, IsAbstract: false }
                    && typeof(OrganisationRegistryCommandController).IsAssignableFrom(t))
        .ToArray();

    [Fact]
    public void FoundAtLeastOneCommandController()
    {
        // Sanity check: if this ever becomes 0 (e.g. a refactor renames/moves
        // the base class), every other assertion below would vacuously pass.
        Assert.NotEmpty(CommandControllerTypes);
    }

    [Fact]
    public void EveryMutatingActionOnEveryCommandControllerReceivesTheIdempotentFilter()
    {
        var application = BuildApplicationModel(CommandControllerTypes);
        new IdempotencyConvention().Apply(application);

        var missing = new List<string>();
        var checkedActionCount = 0;

        foreach (var controller in application.Controllers)
        {
            foreach (var action in controller.Actions)
            {
                if (!IsMutating(action))
                    continue;

                checkedActionCount++;

                var hasIdempotentFilter = action.Filters.OfType<IdempotentAttribute>().Any();
                if (!hasIdempotentFilter)
                    missing.Add($"{controller.ControllerType.FullName}.{action.ActionMethod.Name}");
            }
        }

        Assert.True(
            checkedActionCount > 0,
            "No POST/PATCH actions were found on any command controller - the test setup is broken.");

        Assert.True(
            missing.Count == 0,
            $"The following {missing.Count} mutating (POST/PATCH) action(s) did not receive the " +
            $"[Idempotent] filter from {nameof(IdempotencyConvention)}:{Environment.NewLine}" +
            string.Join(Environment.NewLine, missing));
    }

    [Fact]
    public void NonMutatingActionsAreNotGivenTheIdempotentFilter()
    {
        // PUT/DELETE/GET actions should stay untouched: IdempotentAPI itself
        // only ever acts on POST/PATCH, so attaching the filter elsewhere
        // would be dead weight (or worse, misleading documentation).
        var application = BuildApplicationModel(CommandControllerTypes);
        new IdempotencyConvention().Apply(application);

        var wronglyAttached = new List<string>();

        foreach (var controller in application.Controllers)
        {
            foreach (var action in controller.Actions)
            {
                if (IsMutating(action))
                    continue;

                if (action.Filters.OfType<IdempotentAttribute>().Any())
                    wronglyAttached.Add($"{controller.ControllerType.FullName}.{action.ActionMethod.Name}");
            }
        }

        Assert.True(
            wronglyAttached.Count == 0,
            $"The following non-mutating action(s) unexpectedly received the [Idempotent] filter:{Environment.NewLine}" +
            string.Join(Environment.NewLine, wronglyAttached));
    }

    [Fact]
    public void AnExplicitIdempotentAttributeOnAnActionIsNotOverriddenByTheConvention()
    {
        var controllerType = typeof(ConventionOverrideTestController);
        var application = BuildApplicationModel([controllerType]);
        new IdempotencyConvention().Apply(application);

        var action = application.Controllers.Single().Actions.Single();
        var attribute = action.Filters.OfType<IdempotentAttribute>().Single();

        // The explicit attribute on the test controller below uses a
        // different ExpireHours than the convention's default: if the
        // convention added its own on top, there would be two filters (the
        // `.Single()` above would already have thrown), or this one's
        // configuration would be overwritten.
        Assert.Equal(1, attribute.ExpireHours);
    }

    private static bool IsMutating(ActionModel action)
        => action.Selectors
            .SelectMany(selector => selector.ActionConstraints?.OfType<HttpMethodActionConstraint>() ?? [])
            .SelectMany(constraint => constraint.HttpMethods)
            .Any(method => method is "POST" or "PATCH");

    private static ApplicationModel BuildApplicationModel(IReadOnlyCollection<Type> controllerTypes)
    {
        var application = new ApplicationModel();

        foreach (var controllerType in controllerTypes)
        {
            var controllerTypeInfo = controllerType.GetTypeInfo();
            var controllerModel = new ControllerModel(controllerTypeInfo, controllerTypeInfo.GetCustomAttributes(true));

            foreach (var method in controllerTypeInfo.DeclaredMethods)
            {
                if (!method.IsPublic || method.IsSpecialName)
                    continue;

                var httpMethodAttributes = method
                    .GetCustomAttributes(inherit: true)
                    .OfType<IActionHttpMethodProvider>()
                    .ToArray();

                if (httpMethodAttributes.Length == 0)
                    continue;

                var actionModel = new ActionModel(method, method.GetCustomAttributes(true))
                {
                    Controller = controllerModel,
                };

                // Mirror what ASP.NET Core's DefaultApplicationModelProvider
                // does for real: attributes that are themselves filters (like
                // an explicit [Idempotent]) are copied into Filters before any
                // IApplicationModelConvention runs.
                foreach (var filterAttribute in actionModel.Attributes.OfType<IFilterMetadata>())
                    actionModel.Filters.Add(filterAttribute);

                var selector = new SelectorModel();
                selector.ActionConstraints.Add(
                    new HttpMethodActionConstraint(httpMethodAttributes.SelectMany(a => a.HttpMethods)));
                actionModel.Selectors.Add(selector);

                controllerModel.Actions.Add(actionModel);
            }

            application.Controllers.Add(controllerModel);
        }

        return application;
    }

    private class ConventionOverrideTestController : OrganisationRegistryCommandController
    {
        public ConventionOverrideTestController() : base(null!)
        {
        }

        [Microsoft.AspNetCore.Mvc.HttpPost]
        [Idempotent(ExpireHours = 1)]
        public void Post()
        {
        }
    }
}
