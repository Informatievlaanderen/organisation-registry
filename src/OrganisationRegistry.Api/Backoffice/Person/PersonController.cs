namespace OrganisationRegistry.Api.Backoffice.Person
{
    using System;
    using System.Threading.Tasks;
    using Infrastructure;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Pagination;
    using Infrastructure.Search.Sorting;
    using Infrastructure.Security;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using OrganisationRegistry.Infrastructure.Commands;
    using Queries;
    using Requests;
    using Security;
    using SqlServer.Infrastructure;
    using SqlServer.Person;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("people")]
    public class PersonController : OrganisationRegistryController
    {
        public PersonController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a list of available people.</summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
        {
            var filtering = Request.ExtractFilteringRequest<PersonListItemFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedPersons = new PersonListQuery(context).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedPersons.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedPersons.Items.ToListAsync());
        }

        /// <summary>Get a person.</summary>
        /// <response code="200">If the person is found.</response>
        /// <response code="404">If the person cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
        {
            var person = await context.PersonList.FirstOrDefaultAsync(x => x.Id == id);

            if (person == null)
                return NotFound();

            var authInfo = await HttpContext.GetAuthenticateInfoAsync();
            if (authInfo?.Principal != null && authInfo.Principal.IsInRole(Roles.AlgemeenBeheerder))
                return Ok(person);

            return Ok(new PersonListItem { Id = person.Id, FirstName = person.FirstName, Name = person.Name });
        }

        /// <summary>Create a person.</summary>
        /// <response code="201">If the person is created, together with the location.</response>
        /// <response code="400">If the person information does not pass validation.</response>
        [HttpPost]
        [OrganisationRegistryAuthorize(Roles = Roles.AlgemeenBeheerder)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] CreatePersonRequest message)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await CommandSender.Send(CreatePersonRequestMapping.Map(message));

            return Created(Url.Action(nameof(Get), new { id = message.Id }), null);
        }

        /// <summary>Update a person.</summary>
        /// <response code="200">If the person is updated, together with the location.</response>
        /// <response code="400">If the person information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize(Roles = Roles.AlgemeenBeheerder)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdatePersonRequest message)
        {
            var internalMessage = new UpdatePersonInternalRequest(id, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(UpdatePersonRequestMapping.Map(internalMessage));

            return OkWithLocation(Url.Action(nameof(Get), new { id = internalMessage.PersonId }));
        }
    }
}
