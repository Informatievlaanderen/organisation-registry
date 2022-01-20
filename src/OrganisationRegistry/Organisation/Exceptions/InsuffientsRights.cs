namespace OrganisationRegistry.Organisation.Exceptions
{
    public class InsuffientsRights : DomainException
    {
        public InsuffientsRights()
            : base("U heeft onvoldoende rechten voor deze actie.") { }
    }
}
