namespace OrganisationRegistry
{
    using System;
    using Organisation;

    public interface IUniqueKboValidator
    {
        bool IsKboNumberTaken(KboNumber kboNumber, DateTime? messageValidFrom, DateTime? messageValidTo);
    }
}
