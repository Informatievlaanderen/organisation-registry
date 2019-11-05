namespace OrganisationRegistry.Api.Infrastructure.Search.Pagination
{
    using System;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Primitives;

    public static class ExtractPaginationRequestExtension
    {
        private const string NoPagination = "none";

        public static IPaginationRequest ExtractPaginationRequest(this HttpRequest request)
        {
            var pagination = request.Headers["x-pagination"];

            if (pagination == new StringValues(NoPagination))
                return new NoPaginationRequest();

            var page = 1;
            var pageSize = 10; // TODO: Try to inject Constants in here

            if (string.IsNullOrEmpty(pagination))
                return new PaginationRequest(page, pageSize);

            var headerValues = pagination.ToString().Split(new [] { ','}, 2, StringSplitOptions.RemoveEmptyEntries);
            int.TryParse(headerValues[0], out page);
            int.TryParse(headerValues[1], out pageSize);

            return new PaginationRequest(page, pageSize);
        }
    }
}
