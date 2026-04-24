namespace OrganisationRegistry.Api.Security;

using System.Threading.Tasks;

public interface ITokenIntrospectionService
{
    Task<IntrospectionResponse> IntrospectTokenAsync(string token);
}