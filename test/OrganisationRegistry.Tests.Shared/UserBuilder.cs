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
        {
            return new User(
                _firstName,
                _lastName, _userId, _ip, _roles.ToArray(), _organisations);
        }
    }
}
