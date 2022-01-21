namespace OrganisationRegistry.Body.Exceptions
{
    public class BodyMandateAlreadyCoupledToBodySeatInThisPeriod : DomainException
    {
        public BodyMandateAlreadyCoupledToBodySeatInThisPeriod()
            : base("Er is in deze periode reeds een mandaat gekoppeld aan deze post.") { }
    }
}
