namespace OrganisationRegistry.UnitTests.Organisation.Kbo;

using System;
using OrganisationRegistry.Organisation;

public class KboLocationRetrieverStub : IKboLocationRetriever
{
    private readonly Func<IMagdaAddress, Guid?> _returnValue;

    public KboLocationRetrieverStub(Func<IMagdaAddress, Guid?> returnValue)
    {
        _returnValue = returnValue;
    }

    public Guid? RetrieveLocation(IMagdaAddress address)
    {
        return _returnValue(address);
    }
}