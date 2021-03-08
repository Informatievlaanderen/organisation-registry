namespace OrganisationRegistry.Infrastructure.Authorization
{
    using System.Linq;

    public class User : IUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string? Ip { get; set; }
        public string UserId { get; set; }
        public Role[] Roles { get; set; }

        public User(string firstName, string lastName, string userId, string? ip, Role[] roles)
        {
            FirstName = firstName;
            LastName = lastName;
            UserId = userId;
            Ip = ip;
            Roles = roles;
        }

        public bool IsInRole(Role role)
        {
            return Roles.Any(x => x == role);
        }
    }
}
