namespace OrganisationRegistry.Tests.Shared;

using Xunit;

public class SkipBankAccountsAttribute: FactAttribute
{
    public SkipBankAccountsAttribute()
    {
        Skip = "Skip Bankaccounts";
    }
}
