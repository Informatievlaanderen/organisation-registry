namespace OrganisationRegistry.Api.HostedServices;

using SqlServer.Import.Organisations;
using SqlServer.Infrastructure;

public interface IImportFileParserAndValidator
{
    ParseAndValidatorResult ParseAndValidate(
        ImportOrganisationsStatusListItem importFile,
        OrganisationRegistryContext context,
        IDateTimeProvider dateTimeProvider);
}
