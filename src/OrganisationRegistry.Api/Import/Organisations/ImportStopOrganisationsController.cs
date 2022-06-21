namespace OrganisationRegistry.Api.Import.Organisations;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.FeatureManagement.Mvc;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Commands;
using Security;
using SqlServer.Import.Organisations;
using SqlServer.Infrastructure;
using Validation;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("import/stoppedorganisations")]
[OrganisationRegistryAuthorize(Roles = Roles.AlgemeenBeheerder)]
[FeatureGate(FeatureFlags.ImportApi)]
public class ImportStopOrganisationsController : OrganisationRegistryController
{
    public ImportStopOrganisationsController(ICommandSender commandSender) : base(commandSender)
    {
    }

    [HttpPost]
    public async Task<IActionResult> ImportOrganisations(
        [FromServices] OrganisationRegistryContext context,
        [FromServices] ISecurityService securityService,
        [FromServices] ILogger<ImportOrganisationsController> logger,
        [FromForm] IFormFile bulkimportfile)
    {
        var content = await ImportHelper.GetFileData(bulkimportfile);
        var user = await securityService.GetRequiredUser(User);

        var validationResult = ImportStopOrganisationCsvHeaderValidator.Validate(logger, bulkimportfile.FileName, content);
        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var statusItem = ImportOrganisationsStatusListItem.Create(
            DateTimeOffset.Now,
            user,
            bulkimportfile.FileName,
            content,
            ImportFileTypes.Stop
        );

        context.ImportOrganisationsStatusList.Add(statusItem);

        await context.SaveChangesAsync();

        return Accepted(
            new
            {
                Task = new
                {
                    Href = $"import/stoppedorganisations/{statusItem.Id}",
                    Id = statusItem.Id,
                },
            });
    }

}
