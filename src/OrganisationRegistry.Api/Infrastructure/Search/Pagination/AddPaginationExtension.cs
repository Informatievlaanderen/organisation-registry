namespace OrganisationRegistry.Api.Infrastructure.Search.Pagination;

using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

public static class AddPaginationExtension
{
    public static void AddPaginationResponse(this HttpResponse response, PaginationInfo paginationInfo)
    {
        response.Headers.Add("x-pagination", JsonConvert.SerializeObject(paginationInfo));
    }
}
