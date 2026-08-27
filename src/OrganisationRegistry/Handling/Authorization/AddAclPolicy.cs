namespace OrganisationRegistry.Handling.Authorization;

using System;
using Infrastructure.Authorization;
using Organisation.Exceptions;

public class AddAclPolicy : ISecurityPolicy
{
    public AuthorizationResult Check(IUser user)
    {
        //TODO Custom Exception
        if(user.AclRunner is null)
        {
            throw new NotImplementedException("TODO");
        }

        var result = user.AclRunner.Run();

        if(result.IsSuccess)
        {
            return AuthorizationResult.Success();
        }
        //TODO: mocht je je eigen custom error message willen doorsturen ==> result.Reason
        return AuthorizationResult.Fail(InsufficientRights.CreateFor(this));;
    }

    public override string ToString()
        => "Geen machtiging op orgaan";
}
