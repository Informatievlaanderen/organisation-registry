namespace OrganisationRegistry.Api.Dump.AgentschapZorgEnGezondheid;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElasticSearch.Organisations;
using Osc;

public class OrganisationDump
{
    /// <summary>
    /// Scroll through all <see cref="OrganisationDocument"/> and return entire dataset
    /// </summary>
    /// <param name="client"></param>
    /// <param name="scrollSize"></param>
    /// <param name="scrollTimeout"></param>
    /// <returns></returns>
    public static async Task<IList<OrganisationDocument>> Search(
        IOpenSearchClient client,
        int scrollSize,
        string scrollTimeout)
    {
        var results = new List<OrganisationDocument>();

        var scroll = await client.SearchAsync<OrganisationDocument>(s => s
            .From(0)
            .Size(scrollSize)
            .Scroll(scrollTimeout)
            .Source(source => source.Includes(x => x.Fields(
                field => field.Id,
                field => field.ChangeId,
                field => field.ChangeTime,
                field => field.OvoNumber,
                field => field.Name,
                field => field.ShortName,
                field => field.Validity,
                field => field.Parents,
                field => field.FormalFrameworks,
                field => field.OrganisationClassifications,
                field => field.Labels,
                field => field.Keys))));

        if (scroll.IsValid)
            results.AddRange(scroll.Documents);

        while (scroll.Documents.Any())
        {
            scroll = await client.ScrollAsync<OrganisationDocument>(scrollTimeout, scroll.ScrollId);

            if (scroll.IsValid)
                results.AddRange(scroll.Documents);
        }

        return results;
    }
}