namespace OrganisationRegistry.Organisation
{
    public interface IValidityBuilder<T> where T: IOrganisationField
    {
        T WithValidity(Period period);

        T WithValidFrom(ValidFrom validFrom);
        T WithValidTo(ValidTo validTo);
    }
}
