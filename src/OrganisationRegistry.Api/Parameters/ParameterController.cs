namespace OrganisationRegistry.Api.Parameters;

using System.Threading.Tasks;
using Backoffice.Parameters.BodyClassification.Queries;
using Backoffice.Parameters.BodyClassificationType.Queries;
using Backoffice.Parameters.Building.Queries;
using Backoffice.Parameters.Capacity.Queries;
using Backoffice.Parameters.ContactType.Queries;
using Backoffice.Parameters.FormalFramework.Queries;
using Backoffice.Parameters.FormalFrameworkCategory.Queries;
using Backoffice.Parameters.FunctionType.Queries;
using Backoffice.Parameters.KeyType.Queries;
using Backoffice.Parameters.LabelType.Queries;
using Backoffice.Parameters.LifecyclePhaseType.Queries;
using Backoffice.Parameters.Location.Queries;
using Backoffice.Parameters.LocationType.Queries;
using Backoffice.Parameters.MandateRoleType.Queries;
using Backoffice.Parameters.OrganisationClassification.Queries;
using Backoffice.Parameters.OrganisationClassificationType.Queries;
using Backoffice.Parameters.OrganisationRelationType.Queries;
using Backoffice.Parameters.Purpose.Queries;
using Backoffice.Parameters.RegulationSubTheme.Queries;
using Backoffice.Parameters.RegulationTheme.Queries;
using Backoffice.Parameters.SeatType.Queries;
using Infrastructure;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Pagination;
using Infrastructure.Search.Sorting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.Configuration;
using SqlServer.BodyClassificationType;
using SqlServer.ContactType;
using SqlServer.FormalFrameworkCategory;
using SqlServer.FunctionType;
using SqlServer.Infrastructure;
using SqlServer.LabelType;
using SqlServer.LifecyclePhaseType;
using SqlServer.LocationType;
using SqlServer.MandateRoleType;
using SqlServer.OrganisationClassificationType;
using SqlServer.OrganisationRelationType;
using SqlServer.Purpose;
using SqlServer.RegulationTheme;
using SqlServer.SeatType;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("parameters")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class ParameterController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van orgaanclassificaties op.</summary>
    /// <remarks>
    ///     Vraag een lijst met orgaan classificaties op. <br />
    ///     Geef de header `x-pagination: none` mee om alle entiteiten op te vragen.
    /// </remarks>
    /// <response code="200">Een lijst van orgaanclassificaties.</response>
    [HttpGet("bodyclassifications")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBodyClassifications([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<BodyClassificationListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedBodyClassifications = new BodyClassificationListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedBodyClassifications.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedBodyClassifications.Items.ToListAsync());
    }

    /// <summary>Vraag een lijst van orgaanclassificatietypes op.</summary>
    /// <remarks>
    ///     Vraag een lijst met orgaan classificatietypes op. <br />
    ///     Geef de header `x-pagination: none` mee om alle entiteiten op te vragen.
    /// </remarks>
    /// <response code="200">Een lijst van orgaanclassificatietypes.</response>
    [HttpGet("bodyclassificationtypes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBodyClassificationTypes([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<BodyClassificationTypeListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedBodyClassificationTypes = new BodyClassificationTypeListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedBodyClassificationTypes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedBodyClassificationTypes.Items.ToListAsync());
    }

    /// <summary>Vraag een lijst van gebouwen op.</summary>
    /// <remarks>
    ///     Vraag een lijst met gebouwen op. <br />
    ///     Geef de header `x-pagination: none` mee om alle entiteiten op te vragen.
    /// </remarks>
    /// <response code="200">Een lijst van gebouwen.</response>
    [HttpGet("buildings")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBuildings([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<BuildingListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedBuildings = new BuildingListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedBuildings.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedBuildings.Items.ToListAsync());
    }

    /// <summary>Vraag een lijst van beleidsvelden op.</summary>
    /// <remarks>
    ///     Vraag een lijst met beleidsvelden op. <br />
    ///     Geef de header `x-pagination: none` mee om alle entiteiten op te vragen.
    /// </remarks>
    /// <response code="200">Een lijst van beleidsvelden.</response>
    [HttpGet("purposes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPurposes([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<PurposeListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedPurposes = new PurposeListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedPurposes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedPurposes.Items.ToListAsync());
    }

    /// <summary>Vraag een lijst van regelgevingsubthema's op.</summary>
    /// <remarks>
    ///     Vraag een lijst met regelgevingsubthema's op. <br />
    ///     Geef de header `x-pagination: none` mee om alle entiteiten op te vragen.
    /// </remarks>
    /// <response code="200">Een lijst van regelgevingsubthema's.</response>
    [HttpGet("regulationsubthemes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRegulationSubThemes([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<RegulationSubThemeListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedRegulationSubThemes = new RegulationSubThemeListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedRegulationSubThemes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedRegulationSubThemes.Items.ToListAsync());
    }

    /// <summary>Vraag een lijst van regelgevingthema's op.</summary>
    /// <remarks>
    ///     Vraag een lijst met regelgevingthema's op. <br />
    ///     Geef de header `x-pagination: none` mee om alle entiteiten op te vragen.
    /// </remarks>
    /// <response code="200">Een lijst van regelgevingthema's.</response>
    [HttpGet("regulationthemes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRegulationThemes([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<RegulationThemeListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedRegulationThemes = new RegulationThemeListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedRegulationThemes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedRegulationThemes.Items.ToListAsync());
    }

    /// <summary>Vraag een lijst van zeteltypen op.</summary>
    /// <remarks>
    ///     Vraag een lijst met zeteltypen op. <br />
    ///     Geef de header `x-pagination: none` mee om alle entiteiten op te vragen.
    /// </remarks>
    /// <response code="200">Een lijst van zeteltypen.</response>
    [HttpGet("seattypes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSeatTypes([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<SeatTypeListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedSeatTypes = new SeatTypeListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedSeatTypes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedSeatTypes.Items.ToListAsync());
    }

    /// <summary>Vraag een lijst van organisatie relatie types op.</summary>
    /// <remarks>
    ///     Vraag een lijst met organisatie relatie types op. <br />
    ///     Geef de header `x-pagination: none` mee om alle entiteiten op te vragen.
    /// </remarks>
    /// <response code="200">Een lijst van organisatie relatie types.</response>
    [HttpGet("organisationrelationtypes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrganisationRelationTypes([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<OrganisationRelationTypeListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedOrganisationRelationTypes = new OrganisationRelationTypeListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedOrganisationRelationTypes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedOrganisationRelationTypes.Items.ToListAsync());
    }

    /// <summary>Vraag een lijst van organisatie classificatie types op.</summary>
    /// <remarks>
    ///     Vraag een lijst met organisatie classificatie types op. <br />
    ///     Geef de header `x-pagination: none` mee om alle entiteiten op te vragen.
    /// </remarks>
    /// <response code="200">Een lijst van organisatie classificatie types.</response>
    [HttpGet("organisationclassificationtypes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrganisationClassificationTypes(
        [FromServices] OrganisationRegistryContext context,
        [FromServices] IOrganisationRegistryConfiguration organisationRegistryConfiguration)
    {
        var filtering = Request.ExtractFilteringRequest<OrganisationClassificationTypeListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedOrganisationClassificationTypes = new OrganisationClassificationTypeListQuery(context, organisationRegistryConfiguration).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedOrganisationClassificationTypes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedOrganisationClassificationTypes.Items.ToListAsync());
    }

    /// <summary>Vraag een lijst van organisatie classificaties op.</summary>
    /// <remarks>
    ///     Vraag een lijst met organisatie classificaties op. <br />
    ///     Geef de header `x-pagination: none` mee om alle entiteiten op te vragen.
    /// </remarks>
    /// <response code="200">Een lijst van organisatie classificaties.</response>
    [HttpGet("organisationclassifications")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetOrganisationClassifications([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<OrganisationClassificationListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedOrganisationClassifications = new OrganisationClassificationListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedOrganisationClassifications.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedOrganisationClassifications.Items.ToListAsync());
    }

    /// <summary>Vraag een lijst van mandaat rol types op.</summary>
    /// <remarks>
    ///     Vraag een lijst met mandaat rol types op. <br />
    ///     Geef de header `x-pagination: none` mee om alle entiteiten op te vragen.
    /// </remarks>
    /// <response code="200">Een lijst van mandaat rol types.</response>
    [HttpGet("mandateroletypes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMandateRoleTypes([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<MandateRoleTypeListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedMandateRoleTypes = new MandateRoleTypeListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedMandateRoleTypes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedMandateRoleTypes.Items.ToListAsync());
    }

    /// <summary>Vraag een lijst van locatie types op.</summary>
    /// <remarks>
    ///     Vraag een lijst met locatie types op. <br />
    ///     Geef de header `x-pagination: none` mee om alle entiteiten op te vragen.
    /// </remarks>
    /// <response code="200">Een lijst van locatie types.</response>
    [HttpGet("locationtypes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLocationTypes(
        [FromServices] OrganisationRegistryContext context,
        [FromServices] IOrganisationRegistryConfiguration configuration)
    {
        var filtering = Request.ExtractFilteringRequest<LocationTypeListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedLocationTypes = new LocationTypeListQuery(context, configuration).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedLocationTypes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedLocationTypes.Items.ToListAsync());
    }

    /// <summary>Vraag een lijst van locaties op.</summary>
    /// <remarks>
    ///     Vraag een lijst met locaties op. <br />
    ///     Geef de header `x-pagination: none` mee om alle entiteiten op te vragen.
    /// </remarks>
    /// <response code="200">Een lijst van locaties.</response>
    [HttpGet("locations")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLocations([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<LocationListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedLocations = new LocationListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedLocations.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedLocations.Items.ToListAsync());
    }

    /// <summary>Vraag een lijst van levensloopfase types op.</summary>
    /// <remarks>
    ///     Vraag een lijst met levensloopfase types op. <br />
    ///     Geef de header `x-pagination: none` mee om alle entiteiten op te vragen.
    /// </remarks>
    /// <response code="200">Een lijst van levensloopfase types.</response>
    [HttpGet("lifecyclephasetypes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLifecyclePhaseTypes([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<LifecyclePhaseTypeListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedLifecyclePhaseTypes = new LifecyclePhaseTypeListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedLifecyclePhaseTypes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedLifecyclePhaseTypes.Items.ToListAsync());
    }

    /// <summary>Vraag een lijst van benaming types op.</summary>
    /// <remarks>
    ///     Vraag een lijst met benaming types op. <br />
    ///     Geef de header `x-pagination: none` mee om alle entiteiten op te vragen.
    /// </remarks>
    /// <response code="200">Een lijst van benaming types.</response>
    [HttpGet("labeltypes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetLabelTypes(
        [FromServices] OrganisationRegistryContext context,
        [FromServices] IOrganisationRegistryConfiguration configuration)
    {
        var filtering = Request.ExtractFilteringRequest<LabelTypeListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedLabelTypes = new LabelTypeListQuery(context, configuration, _ => true).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedLabelTypes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedLabelTypes.Items.ToListAsync());
    }

    /// <summary>Vraag een lijst van informatiesystemen op.</summary>
    /// <remarks>
    ///     Vraag een lijst met informatiesystemen op. <br />
    ///     Geef de header `x-pagination: none` mee om alle entiteiten op te vragen.
    /// </remarks>
    /// <response code="200">Een lijst van informatiesystemen.</response>
    [HttpGet("keytypes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetKeyTypes(
        [FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<KeyTypeListQuery.KeyTypeListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        filtering.Filter ??= new KeyTypeListQuery.KeyTypeListItemFilter();

        var pagedKeyTypes = new KeyTypeListQuery(context, _ => true).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedKeyTypes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedKeyTypes.Items.ToListAsync());
    }

    /// <summary>Vraag een lijst van functie types op.</summary>
    /// <remarks>
    ///     Vraag een lijst met functie types op. <br />
    ///     Geef de header `x-pagination: none` mee om alle entiteiten op te vragen.
    /// </remarks>
    /// <response code="200">Een lijst van functie types.</response>
    [HttpGet("functiontypes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFunctionTypes([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<FunctionTypeListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedFunctionTypes = new FunctionTypeListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedFunctionTypes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedFunctionTypes.Items.ToListAsync());
    }

    /// <summary>Vraag een lijst van toepassingsgebied categorieën op.</summary>
    /// <remarks>Vraag een lijst met toepassingsgebied categorieën op. <br />
    /// Geef de header `x-pagination: none` mee om alle entiteiten op te vragen.
    /// </remarks>
    [HttpGet("formalframeworkcategories")]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<FormalFrameworkCategoryListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedFormalFrameworkCategories = new FormalFrameworkCategoryListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedFormalFrameworkCategories.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedFormalFrameworkCategories.Items.ToListAsync());
    }

    /// <summary>Vraag een lijst van toepassingsgebieden op.</summary>
    /// <remarks>
    ///     Vraag een lijst met toepassingsgebieden op. <br />
    ///     Geef de header `x-pagination: none` mee om alle entiteiten op te vragen.
    /// </remarks>
    /// <response code="200">Een lijst van toepassingsgebieden.</response>
    [HttpGet("formalframeworks")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFormalFrameworks([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<FormalFrameworkListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedFormalFrameworks = new FormalFrameworkListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedFormalFrameworks.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedFormalFrameworks.Items.ToListAsync());
    }

    /// <summary>Vraag een lijst van contact types op.</summary>
    /// <remarks>
    ///     Vraag een lijst met contact types op. <br />
    ///     Geef de header `x-pagination: none` mee om alle entiteiten op te vragen.
    /// </remarks>
    /// <response code="200">Een lijst van contact types.</response>
    [HttpGet("contacttypes")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetContactTypes([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<ContactTypeListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedContactTypes = new ContactTypeListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedContactTypes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedContactTypes.Items.ToListAsync());
    }

    /// <summary>Vraag een lijst van hoedanigheden op.</summary>
    /// <remarks>
    ///     Vraag een lijst met hoedanigheden op. <br />
    ///     Geef de header `x-pagination: none` mee om alle entiteiten op te vragen.
    /// </remarks>
    /// <response code="200">Een lijst van hoedanigheden.</response>
    [HttpGet("capacities")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCapacities([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<CapacityListQuery.CapacityListFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedCapacities = new CapacityListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedCapacities.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedCapacities.Items.ToListAsync());
    }
}
