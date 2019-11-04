namespace OrganisationRegistry.Body
{
    public class BodyMandateAlreadyCoupledToBodySeatInThisPeriodException : DomainException
    {
        public BodyMandateAlreadyCoupledToBodySeatInThisPeriodException()
            : base("Er is in deze periode reeds een mandaat gekoppeld aan deze post.") { }
    }
}
