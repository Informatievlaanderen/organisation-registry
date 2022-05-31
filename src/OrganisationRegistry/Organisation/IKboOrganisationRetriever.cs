namespace OrganisationRegistry.Organisation;

using System.Threading.Tasks;
using Infrastructure.Authorization;

public interface IKboOrganisationRetriever
{
    Task<Result<IMagdaOrganisationResponse>> RetrieveOrganisation(IUser user, KboNumber kboNumber);
}
