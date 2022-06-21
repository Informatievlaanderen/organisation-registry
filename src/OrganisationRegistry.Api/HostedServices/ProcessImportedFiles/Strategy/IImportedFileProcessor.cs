namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Strategy;

using System.Threading;
using System.Threading.Tasks;
using OrganisationRegistry.SqlServer.Import.Organisations;

public interface IImportedFileProcessor
{
    Task<ProcessImportedFileResult> Process(ImportOrganisationsStatusListItem importFile, CancellationToken cancellationToken);
}
