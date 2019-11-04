namespace OrganisationRegistry.Person
{
    using System;

    public class PersonId : GenericId<PersonId>
    {
        public PersonId(Guid id) : base(id) { }
    }
}
