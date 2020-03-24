namespace OrganisationRegistry.Organisation
{
    using System.Security.Claims;
    using System.Threading.Tasks;

    public interface IKboOrganisationRetriever
    {
        Task<Result<IMagdaOrganisationResponse>> RetrieveOrganisation(ClaimsPrincipal user, KboNumber kboNumber);
    }
}
