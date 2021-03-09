namespace OrganisationRegistry.Organisation
{
    public class InsuffientsRights : DomainException
    {
        public InsuffientsRights()
            : base("U heeft onvoldoende rechten voor deze actie.") { }
    }
}
