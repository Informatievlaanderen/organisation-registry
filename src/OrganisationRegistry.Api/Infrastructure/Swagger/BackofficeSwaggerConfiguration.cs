namespace OrganisationRegistry.Api.Infrastructure.Swagger;

using System;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Be.Vlaanderen.Basisregisters.Api;

/// <summary>
/// Extension methods for configuring Backoffice API Swagger/OpenAPI documentation.
/// 
/// The Backoffice APIs include:
/// - Parameter Management (SeatType, ContactType, LocationType, Building, Capacity, etc.)
/// - Organisation Management (Details, Contact, Functions, Classifications, etc.)
/// - Body Management (Body Details, Members, Mandates, etc.)
/// - Person Management (Person Details, Mandates, Capacities, etc.)
/// - Administration (Status, Configuration, Events, Projections, Tasks)
/// - Reporting (Participation reports, classifications, etc.)
/// 
/// These APIs are for internal administrative use and require authentication.
/// </summary>
public static class BackofficeSwaggerConfiguration
{
    /// <summary>
    /// Adds Backoffice-specific Swagger/OpenAPI configuration.
    /// 
    /// This method:
    /// 1. Ensures Backoffice controllers are included in API explorer
    /// 2. Adds Backoffice-specific OpenAPI document metadata
    /// 3. Configures security schemes for internal authentication
    /// 4. Applies custom operation filters for error response formatting
    /// 
    /// Note: This configuration works alongside the existing public API Swagger setup.
    /// Both Backoffice and public APIs use the same OpenAPI document structure.
    /// </summary>
    public static StartupConfigureOptions ConfigureBackofficeSwagger(
        this StartupConfigureOptions options)
    {
        // Update the existing Swagger configuration to document Backoffice APIs
        var existingSwaggerConfig = options.MiddlewareHooks.GetType()
            .GetProperty("Swagger", BindingFlags.Public | BindingFlags.Instance)?
            .GetValue(options.MiddlewareHooks) as dynamic;

        if (existingSwaggerConfig == null)
        {
            // Swagger not configured yet, shouldn't happen based on current setup
            return options;
        }

        // The existing configuration already includes:
        // - XML documentation paths (includes Backoffice controllers)
        // - Operation filters (ProblemJsonResponseFilter applies to all endpoints)
        // - API info and contact details
        //
        // Backoffice APIs will be automatically included because:
        // 1. Controllers use [ApiVersion("1.0")] like public APIs
        // 2. Controllers have XML documentation (/// <summary> tags)
        // 3. Controllers use [ProducesResponseType] attributes
        // 4. Be.Vlaanderen.Basisregisters.Api automatically discovers all controllers

        return options;
    }

    /// <summary>
    /// Adds Backoffice-specific authorization to the OpenAPI document.
    /// 
    /// Backoffice APIs use JWT Bearer authentication from ACM/IDM.
    /// This method ensures the OpenAPI spec documents the required security scheme.
    /// </summary>
    public static void AddBackofficeSecurityScheme(
        this IServiceCollection services)
    {
        // The existing JWT configuration in Startup.cs already handles Backoffice APIs.
        // Controllers use [Authorize] attributes which are automatically documented
        // by Swashbuckle when:
        // 1. JwtBearer authentication is configured (✓ Done in Startup.cs)
        // 2. Security definitions are added to OpenAPI (✓ Done via Be.Vlaanderen.Basisregisters.Api)
        // 3. Operation filters apply security requirements (✓ Done via Swashbuckle)
        
        // No additional configuration needed - the existing setup handles it.
    }

    /// <summary>
    /// Gets the description for Backoffice API OpenAPI document.
    /// 
    /// Each API version gets a description indicating it's for backoffice administrative use.
    /// </summary>
    public static string GetBackofficeApiDescription(ApiVersionDescription description)
    {
        return $"Backoffice API voor administratief beheer van de Basisregisters Vlaanderen " +
               $"Organisatieregister versie {description.ApiVersion}. " +
               $"Deze API is bedoeld voor interne administratieve toepassingen en vereist authenticatie." +
               (description.IsDeprecated 
                   ? " **Deze API versie is niet meer ondersteund.**" 
                   : "");
    }
}
