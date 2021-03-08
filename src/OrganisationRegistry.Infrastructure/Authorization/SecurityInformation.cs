namespace OrganisationRegistry.Infrastructure.Authorization
{
    using System;
    using System.Collections.Generic;

    public class SecurityInformation
    {
        public string UserName { get; }

        public IList<Role> Roles { get; }

        public IList<string> OvoNumbers { get; }

        public IList<Guid> OrganisationIds { get; }

        public IList<Guid> BodyIds { get; }

        public SecurityInformation(string userName) : this(userName, new List<Role>(), new List<string>(), new List<Guid>(), new List<Guid>()) { }

        public SecurityInformation(string userName, IList<Role> roles, IList<string> ovoNumbers, IList<Guid> organisationIds, IList<Guid> bodyIds)
        {
            UserName = userName;
            Roles = roles;
            OvoNumbers = ovoNumbers;
            OrganisationIds = organisationIds;
            BodyIds = bodyIds;
        }
    }
}
