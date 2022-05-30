namespace OrganisationRegistry.UnitTests.Organisation.Kbo;

using OrganisationRegistry.Organisation;

public class UniqueKboNumberValidatorStub : IUniqueKboValidator
{
    private readonly bool _returnValue;

    public UniqueKboNumberValidatorStub(bool returnValue)
        => _returnValue = returnValue;

    public bool IsKboNumberTaken(KboNumber kboNumber)
        => _returnValue;
}