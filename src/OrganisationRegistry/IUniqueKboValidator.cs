namespace OrganisationRegistry;

using Organisation;

public interface IUniqueKboValidator
{
    bool IsKboNumberTaken(KboNumber kboNumber);
}
