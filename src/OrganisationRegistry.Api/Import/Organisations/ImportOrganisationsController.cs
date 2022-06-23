namespace OrganisationRegistry.Api.Import.Organisations;

using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
[OrganisationRegistryRoute("imports")]
[OrganisationRegistryAuthorize(Roles = Roles.AlgemeenBeheerder + "," + Roles.VlimpersBeheerder)]
[FeatureGate(FeatureFlags.ImportApi)]
public class ImportOrganisationsController : OrganisationRegistryController
{
    public ImportOrganisationsController(ICommandSender commandSender) : base(commandSender)
    {
    }

    [HttpPost("organisation-creations")]
    public async Task<IActionResult> ImportOrganisationCreations(
        [FromServices] OrganisationRegistryContext context,
        [FromServices] ISecurityService securityService,
        [FromServices] ILogger<ImportOrganisationsController> logger,
        [FromForm] IFormFile bulkimportfile)
    {
        var content = await ImportHelper.GetFileData(bulkimportfile);
        var user = await securityService.GetRequiredUser(User);

        var labelTypes = await LabelTypes.GetNames(context);

        var validationResult = OrganisationCreationsCsvHeaderValidator.Validate(logger, labelTypes, bulkimportfile.FileName, content);
        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        var statusItem = ImportOrganisationsStatusListItem.Create(
            DateTimeOffset.Now,
            user,
            bulkimportfile.FileName,
            content,
            ImportFileTypes.Create
        );

        context.ImportOrganisationsStatusList.Add(statusItem);

        await context.SaveChangesAsync();

        return Accepted(
            new
            {
                Task = new
                {
                    Href = $"import/organisations/{statusItem.Id}",
                    Id = statusItem.Id,
                },
            });
    }

    [HttpPost("organisation-terminations")]
    public async Task<IActionResult> ImportOrganisationTerminations(
        [FromServices] OrganisationRegistryContext context,
        [FromServices] ISecurityService securityService,
        [FromServices] ILogger<ImportOrganisationsController> logger,
        [FromForm] IFormFile bulkimportfile)
    {
        var content = await ImportHelper.GetFileData(bulkimportfile);
        var user = await securityService.GetRequiredUser(User);

        var validationResult = ImportOrganisationTerminationsCsvHeaderValidator.Validate(logger, bulkimportfile.FileName, content);
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

    [HttpGet]
    public async Task<IActionResult> GetImports(
        [FromServices] ISecurityService securityService,
        [FromServices] OrganisationRegistryContext context
    )
    {
        var user = await securityService.GetRequiredUser(User);
        var imports = await context.ImportOrganisationsStatusList
            .AsNoTracking()
            .Where(import => import.UserId == user.UserId)
            .OrderByDescending(import => import.UploadedAt)
            .Take(100)
            .ToListAsync();

        var response = new ImportOrganisationStatusResponse(
            imports.Select(
                import => new ImportOrganisationStatusResponseItem(
                    import.Id,
                    import.Status,
                    import.FileName,
                    import.UploadedAt,
                    import.ImportFileType
                )));

        return Ok(response);
    }

    [HttpGet("{id:guid}/content")]
    public async Task<IActionResult> GetImportFile(
        [FromServices] OrganisationRegistryContext context,
        [FromRoute] Guid id
    )
    {
        var maybeImport = await context.ImportOrganisationsStatusList
            .FindAsync(id);

        if (maybeImport is not { } import)
            return NotFound();

        if (import.Status == ImportProcessStatus.Processing)
            return NotFound("File not yet processed");

        if (import.OutputFileContent is not { } outputFileContent)
            throw new OutputFileNotGenerated();

        return File(Encoding.UTF8.GetBytes(outputFileContent), "text/csv");
    }
}
