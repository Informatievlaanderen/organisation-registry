namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

using OrganisationRegistry.SqlServer.Import.Organisations;
using OrganisationRegistry.SqlServer.Infrastructure;

public interface IImportFileParserAndValidator
{
    ParseAndValidatorResult ParseAndValidate(
        ImportOrganisationsStatusListItem importFile,
        OrganisationRegistryContext context,
        IDateTimeProvider dateTimeProvider);
}
