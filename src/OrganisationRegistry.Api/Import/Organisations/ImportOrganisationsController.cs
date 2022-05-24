namespace OrganisationRegistry.Api.Import.Organisations;

using System;
using System.IO;
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
[OrganisationRegistryRoute("import/organisations")]
[OrganisationRegistryAuthorize(Roles = Roles.AlgemeenBeheerder)]
[FeatureGate(FeatureFlags.ImportApi)]
public class ImportOrganisationsController : OrganisationRegistryController
{
    public ImportOrganisationsController(ICommandSender commandSender) : base(commandSender)
    {
    }

    [HttpPost]
    public async Task<IActionResult> ImportOrganisations(
        [FromServices] OrganisationRegistryContext context,
        [FromServices] ISecurityService securityService,
        [FromServices] ILogger<ImportOrganisationsController> logger,
        [FromForm] IFormFile bulkimportfile)
    {
        var id = Guid.NewGuid();
        var content = await GetFileData(bulkimportfile);
        var user = await securityService.GetRequiredUser(User);

        var validationResult = ImportOrganisationCsvHeaderValidator.Validate(logger, bulkimportfile.FileName, content);
        if (!validationResult.IsValid)
            return BadRequest(validationResult);

        context.ImportOrganisationsStatusList.Add(
            new ImportOrganisationsStatusListItem
            {
                Id = id,
                Status = ImportProcessStatus.Processing,
                UserId = user.UserId,
                UserName = $"{user.FirstName} {user.LastName}",
                FileContent = content,
                FileName = bulkimportfile.FileName,
                UploadedAt = DateTimeOffset.Now
            });

        await context.SaveChangesAsync();

        return Accepted(
            new
            {
                Task = new
                {
                    Href = $"import/organisations/{id}",
                    Id = id,
                }
            });
    }

    private static async Task<string> GetFileData(IFormFile bulkimportfile)
    {
        using var streamReader = new StreamReader(bulkimportfile.OpenReadStream());
        return await streamReader.ReadToEndAsync();
    }

    [HttpGet]
    public async Task<IActionResult> GetImportStatuses(
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
                    import.UploadedAt
                )));

        return Ok(response);
    }

    [HttpGet("{id:guid}/content")]
    public async Task<IActionResult> GetImportStatusFile(
        [FromServices] OrganisationRegistryContext context,
        [FromRoute] Guid id
    )
    {
        var maybeImport = await context.ImportOrganisationsStatusList
            .FindAsync(id);

        if (maybeImport is not { } import)
            return NotFound();

        return File(Encoding.UTF8.GetBytes(import.FileContent), "text/csv");
    }
}
