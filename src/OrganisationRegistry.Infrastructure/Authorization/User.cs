namespace OrganisationRegistry.Infrastructure.Authorization
{
    using System.Collections.Generic;
    using System.Linq;

    public class User : IUser
    {
        public User(
            string firstName,
            string lastName,
            string userId,
            string? ip,
            Role[] roles,
            IEnumerable<string> organisations)
        {
            Organisations = organisations.ToList();
            FirstName = firstName;
            LastName = lastName;
            UserId = userId;
            Ip = ip ?? string.Empty;
            Roles = roles;
        }

        public List<string> Organisations { get; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Ip { get; set; }
        public string UserId { get; set; }
        public Role[] Roles { get; set; }

        public bool IsAuthorizedForVlimpersOrganisations
            => IsInRole(Role.VlimpersBeheerder) ||
               IsInRole(Role.Developer) ||
               IsInRole(Role.AlgemeenBeheerder);

        public bool IsInRole(Role role)
        {
            return Roles.Any(x => x == role);
        }

        public bool IsOrganisatieBeheerderFor(string ovoNumber)
            => IsInRole(Role.DecentraalBeheerder) &&
               Organisations.Contains(ovoNumber);
    }
}
