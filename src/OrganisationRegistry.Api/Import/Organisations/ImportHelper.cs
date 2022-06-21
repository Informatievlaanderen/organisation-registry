namespace OrganisationRegistry.Api.Import.Organisations;

using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

public static class ImportHelper
{
    public static async Task<string> GetFileData(IFormFile bulkimportfile)
    {
        using var streamReader = new StreamReader(bulkimportfile.OpenReadStream(), Encoding.UTF8);
        return await streamReader.ReadToEndAsync();
    }
}
