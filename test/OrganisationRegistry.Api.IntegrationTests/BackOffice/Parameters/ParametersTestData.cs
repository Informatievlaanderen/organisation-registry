namespace OrganisationRegistry.Api.IntegrationTests.BackOffice.Parameters;

using System.Collections.Generic;
using System.Collections.Immutable;
using Backoffice.Parameters.BodyClassification.Requests;
using Backoffice.Parameters.BodyClassificationType.Requests;
using Backoffice.Parameters.Building.Requests;
using Backoffice.Parameters.Capacity.Requests;
using Backoffice.Parameters.FormalFramework.Requests;
using Backoffice.Parameters.FormalFrameworkCategory.Requests;
using Backoffice.Parameters.FunctionType.Requests;
using Backoffice.Parameters.KeyType.Requests;
using Backoffice.Parameters.LabelType.Requests;
using Backoffice.Parameters.LifecyclePhaseType.Requests;
using Backoffice.Parameters.Location.Requests;
using Backoffice.Parameters.LocationType.Requests;
using Backoffice.Parameters.MandateRoleType.Requests;
using Backoffice.Parameters.OrganisationClassification.Requests;
using Backoffice.Parameters.OrganisationClassificationType.Requests;
using Backoffice.Parameters.OrganisationRelationType.Requests;
using Backoffice.Parameters.Purpose.Requests;
using Backoffice.Parameters.RegulationSubTheme.Requests;
using Backoffice.Parameters.RegulationTheme.Requests;
using Backoffice.Parameters.SeatType.Requests;

public class ParametersTestData
{
    public const string KeytypesRoute = "/v1/keytypes";
    public const string LabeltypesRoute = "/v1/labeltypes";
    public const string BodyclassificationtypesRoute = "/v1/bodyclassificationtypes";
    public const string CapacitiesRoute = "/v1/capacities";
    public const string FormalframeworkcategoriesRoute = "/v1/formalframeworkcategories";
    public const string FunctiontypesRoute = "/v1/functiontypes";
    public const string LocationtypesRoute = "/v1/locationtypes";
    public const string MandateroletypesRoute = "/v1/mandateroletypes";
    public const string PurposesRoute = "/v1/purposes";
    public const string RegulationthemesRoute = "/v1/regulationthemes";
    public const string LocationsRoute = "/v1/locations";
    public const string OrganisationrelationtypesRoute = "/v1/organisationrelationtypes";
    public const string SeattypesRoute = "/v1/seattypes";
    public const string BuildingsRoute = "/v1/buildings";
    public const string OrganisationclassificationtypesRoute = "/v1/organisationclassificationtypes";
    public const string FormalframeworksRoute = "/v1/formalframeworks";
    public const string RegulationsubthemesRoute = "/v1/regulationsubthemes";
    public const string BodyclassificationsRoute = "/v1/bodyclassifications";
    public const string OrganisationclassificationsRoute = "/v1/organisationclassifications";
    public const string LifecyclephasetypesRoute = "/v1/lifecyclephasetypes";

    public static readonly Dictionary<string, ParameterTestParameterDependencies> ParametersToTestDependencies = new()
    {
        { FormalframeworkcategoriesRoute, new ParameterTestParameterDependencies(nameof(CreateFormalFrameworkRequest.FormalFrameworkCategoryId), typeof(CreateFormalFrameworkCategoryRequest)) },
        { RegulationthemesRoute, new ParameterTestParameterDependencies(nameof(CreateRegulationSubThemeRequest.RegulationThemeId), typeof(CreateRegulationThemeRequest)) },
        { BodyclassificationtypesRoute, new ParameterTestParameterDependencies(nameof(CreateBodyClassificationRequest.BodyClassificationTypeId), typeof(CreateBodyClassificationTypeRequest)) },
        { OrganisationclassificationtypesRoute, new ParameterTestParameterDependencies(nameof(CreateOrganisationClassificationRequest.OrganisationClassificationTypeId), typeof(CreateOrganisationClassificationTypeRequest)) },
    };

    public static readonly Dictionary<string, ParameterTestParameters> ParametersToTest = new()
    {
        { KeytypesRoute, new ParameterTestParameters(typeof(CreateKeyTypeRequest), true, ImmutableList<string>.Empty) },
        { LifecyclephasetypesRoute, new ParameterTestParameters(typeof(CreateLifecyclePhaseTypeRequest), false, ImmutableList<string>.Empty) },
        { LabeltypesRoute, new ParameterTestParameters(typeof(CreateLabelTypeRequest), false, ImmutableList<string>.Empty) },
        { BodyclassificationtypesRoute, new ParameterTestParameters(typeof(CreateBodyClassificationTypeRequest), false, ImmutableList<string>.Empty) },
        { CapacitiesRoute, new ParameterTestParameters(typeof(CreateCapacityRequest), true, ImmutableList<string>.Empty) },
        { FormalframeworkcategoriesRoute, new ParameterTestParameters(typeof(CreateFormalFrameworkCategoryRequest), false, ImmutableList<string>.Empty) },
        { FunctiontypesRoute, new ParameterTestParameters(typeof(CreateFunctionTypeRequest), false, ImmutableList<string>.Empty) },
        { LocationtypesRoute, new ParameterTestParameters(typeof(CreateLocationTypeRequest), false, ImmutableList<string>.Empty) },
        { MandateroletypesRoute, new ParameterTestParameters(typeof(CreateMandateRoleTypeRequest), false, ImmutableList<string>.Empty) },
        { PurposesRoute, new ParameterTestParameters(typeof(CreatePurposeRequest), false, ImmutableList<string>.Empty) },
        { RegulationthemesRoute, new ParameterTestParameters(typeof(CreateRegulationThemeRequest), false, ImmutableList<string>.Empty) },
        { LocationsRoute, new ParameterTestParameters(typeof(CreateLocationRequest), false, ImmutableList<string>.Empty) },
        { OrganisationrelationtypesRoute, new ParameterTestParameters(typeof(CreateOrganisationRelationTypeRequest), false, ImmutableList<string>.Empty) },
        { SeattypesRoute, new ParameterTestParameters(typeof(CreateSeatTypeRequest), false, ImmutableList<string>.Empty) },
        { BuildingsRoute, new ParameterTestParameters(typeof(CreateBuildingRequest), false, ImmutableList<string>.Empty) },
        { OrganisationclassificationtypesRoute, new ParameterTestParameters(typeof(CreateOrganisationClassificationTypeRequest), false, ImmutableList<string>.Empty) },
        { FormalframeworksRoute, new ParameterTestParameters(typeof(CreateFormalFrameworkRequest), false, ImmutableList.Create(FormalframeworkcategoriesRoute)) },
        { RegulationsubthemesRoute, new ParameterTestParameters(typeof(CreateRegulationSubThemeRequest), false, ImmutableList.Create(RegulationthemesRoute)) },
        { BodyclassificationsRoute, new ParameterTestParameters(typeof(CreateBodyClassificationRequest), false, ImmutableList.Create(BodyclassificationtypesRoute)) },
        { OrganisationclassificationsRoute, new ParameterTestParameters(typeof(CreateOrganisationClassificationRequest), false, ImmutableList.Create(OrganisationclassificationtypesRoute)) },
    };
}
