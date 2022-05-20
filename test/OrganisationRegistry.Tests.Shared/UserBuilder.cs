namespace OrganisationRegistry.Infrastructure.Authorization
{
    using System.Collections.Generic;

    public class UserBuilder
    {
        private string _firstName;
        private string _lastName;
        private string _userId;
        private string _ip;
        private readonly List<Role> _roles;
        private readonly List<string> _organisations;

        public UserBuilder()
        {
            _firstName = string.Empty;
            _lastName = string.Empty;
            _userId = string.Empty;
            _ip = string.Empty;
            _roles = new List<Role>();
            _organisations = new List<string>();
        }

        public UserBuilder WithFirstName(string firstName)
        {
            _firstName = firstName;
            return this;
        }


        public UserBuilder WithLastName(string lastName)
        {
            _lastName = lastName;
            return this;
        }


        public UserBuilder WithUserId(string userId)
        {
            _userId = userId;
            return this;
        }


        public UserBuilder WithIp(string ip)
        {
            _ip = ip;
            return this;
        }

        public UserBuilder AddRoles(params Role[] roles)
        {
            _roles.AddRange(roles);
            return this;
        }

        public UserBuilder AddOrganisations(params string[] organisations)
        {
            _organisations.AddRange(organisations);
            return this;
        }

        public User Build()
            => new(
                _firstName,
                _lastName, _userId, _ip, _roles.ToArray(), _organisations);

        public static User AlgemeenBeheerder()
            => new UserBuilder().AddRoles(Role.AlgemeenBeheerder).Build();

        public static User DecentraalBeheerder()
            => new UserBuilder().AddRoles(Role.DecentraalBeheerder).Build();

        public static User OrgaanBeheerder()
            => new UserBuilder().AddRoles(Role.OrgaanBeheerder).Build();

        public static User RegelgevingBeheerder()
            => new UserBuilder().AddRoles(Role.RegelgevingBeheerder).Build();

        public static User VlimpersBeheerder()
            => new UserBuilder().AddRoles(Role.VlimpersBeheerder).Build();

        public static User User()
            => new UserBuilder().Build();
    }
}
