namespace OrganisationRegistry.Api.KeyType
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Infrastructure;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Pagination;
    using Infrastructure.Search.Sorting;
    using Infrastructure.Security;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Queries;
    using Requests;
    using Security;
    using SqlServer.Infrastructure;
    using SqlServer.KeyType;
    using OrganisationRegistry.Infrastructure.Commands;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("keytypes")]
    public class KeyTypeController : OrganisationRegistryController
    {
        public KeyTypeController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a list of available key types.</summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
        {
            var filtering = Request.ExtractFilteringRequest<KeyTypeListItem>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedKeyTypes = new KeyTypeListQuery(context).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedKeyTypes.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedKeyTypes.Items.ToListAsync());
        }

        /// <summary>Get a key type.</summary>
        /// <response code="200">If the key type is found.</response>
        /// <response code="404">If the key type cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(KeyTypeListItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
        {
            var keyType = await context.KeyTypeList.FirstOrDefaultAsync(x => x.Id == id);

            if (keyType == null)
                return NotFound();

            return Ok(keyType);
        }

        /// <summary>Create a key type.</summary>
        /// <response code="201">If the key type is created, together with the location.</response>
        /// <response code="400">If the key type information does not pass validation.</response>
        [HttpPost]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder)]
        [ProducesResponseType(typeof(CreatedResult), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public IActionResult Post([FromBody] CreateKeyTypeRequest message)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            CommandSender.Send(CreateKeyTypeRequestMapping.Map(message));

            return Created(Url.Action(nameof(Get), new { id = message.Id }), null);
        }

        /// <summary>Update a key type.</summary>
        /// <response code="200">If the key type is updated, together with the location.</response>
        /// <response code="400">If the key type information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder)]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public IActionResult Put([FromRoute] Guid id, [FromBody] UpdateKeyTypeRequest message)
        {
            var internalMessage = new UpdateKeyTypeInternalRequest(id, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            CommandSender.Send(UpdateKeyTypeRequestMapping.Map(internalMessage));

            return OkWithLocation(Url.Action(nameof(Get), new { id = internalMessage.KeyTypeId }));
        }
    }
}
