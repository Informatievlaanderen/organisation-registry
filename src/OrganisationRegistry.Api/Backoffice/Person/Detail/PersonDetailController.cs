namespace OrganisationRegistry.Api.Backoffice.Person.Detail;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Security;
using OrganisationRegistry.Infrastructure.Authorization;
using Security;
using OrganisationRegistry.SqlServer.Infrastructure;
using OrganisationRegistry.SqlServer.Person;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("people")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Personen")]
public class PersonDetailController : OrganisationRegistryController
{
    /// <summary>Vraag een persoon op.</summary>
    /// <response code="200">Als de persoon gevonden is.</response>
    /// <response code="404">Als de persoon niet gevonden kan worden.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var person = await context.PersonList.FirstOrDefaultAsync(x => x.Id == id);

        if (person == null)
            return NotFound();

        var authInfo = await HttpContext.GetAuthenticateInfoAsync();
        if (authInfo?.Principal != null && authInfo.Principal.IsInRole(RoleMapping.Map(Role.AlgemeenBeheerder)))
            return Ok(person);

        return Ok(new PersonListItem { Id = person.Id, FirstName = person.FirstName, Name = person.Name });
    }
}
