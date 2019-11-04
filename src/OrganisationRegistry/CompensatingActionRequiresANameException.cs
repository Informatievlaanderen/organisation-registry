namespace OrganisationRegistry
{
    public class CompensatingActionRequiresANameException : DomainException
    {
        public CompensatingActionRequiresANameException()
            : base("Voor het uitvoeren van een compensating action is een naam vereist.") { }
    }
}
