namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using SqlServer.Import.Organisations;
using Strategy;

public abstract class ImportedFileProcessor<TDeserializedRecord, TImportCommandItem> : IImportedFileProcessor
{
    public async Task<ProcessImportedFileResult> Process(ImportOrganisationsStatusListItem importFile, CancellationToken cancellationToken)
    {
        var parsedRecords = Parse(importFile);
        var validationResult = Validate(parsedRecords);
        var processResult = await Process(importFile, validationResult, cancellationToken);

        return new ProcessImportedFileResult(importFile, processResult, validationResult.ValidationOk);
    }

    protected abstract List<ParsedRecord<TDeserializedRecord>> Parse(ImportOrganisationsStatusListItem importFile);

    protected abstract ValidationResult<TImportCommandItem> Validate(List<ParsedRecord<TDeserializedRecord>> parsedRecords);

    protected abstract Task<string> Process(ImportOrganisationsStatusListItem importFile, ValidationResult<TImportCommandItem> validationResult, CancellationToken cancellationToken);
}
