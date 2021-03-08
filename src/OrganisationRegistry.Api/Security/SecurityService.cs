namespace OrganisationRegistry.Api.Security
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Bus;
    using SqlServer.Infrastructure;

    public class SecurityService : ISecurityService
    {
        private readonly Dictionary<string, Role> _roleMapping = new Dictionary<string, Role>
        {
            { Roles.OrganisationRegistryBeheerder, Role.OrganisationRegistryBeheerder },
            { Roles.OrganisatieBeheerder, Role.OrganisatieBeheerder },
            { Roles.OrgaanBeheerder, Role.OrgaanBeheerder },
            { Roles.Developer, Role.Developer },
            { Roles.AutomatedTask, Role.AutomatedTask}
        };

        private const string ClaimOrganisation = "urn:be:vlaanderen:wegwijs:organisation";

        private readonly OrganisationRegistryContext _context;

        public SecurityService(OrganisationRegistryContext context)
        {
            _context = context;
        }

        public bool CanAddOrganisation(ClaimsPrincipal user, Guid? parentOrganisationId)
        {
            var securityInfo = GetSecurityInformation(user);

            // Admins can do everything
            if (securityInfo.Roles.Contains(Role.OrganisationRegistryBeheerder) ||
                securityInfo.Roles.Contains(Role.Developer))
                return true;

            // Otherwise you can only add to parents you are allowed
            return HasPermissionsForOrganisation(securityInfo, parentOrganisationId);
        }

        public bool CanEditOrganisation(ClaimsPrincipal user, Guid organisationId)
        {
            var securityInfo = GetSecurityInformation(user);

            // Admins can do everything
            if (securityInfo.Roles.Contains(Role.OrganisationRegistryBeheerder) ||
                securityInfo.Roles.Contains(Role.Developer))
                return true;

            // Otherwise you can only edit what you are allowed
            return HasPermissionsForOrganisation(securityInfo, organisationId);
        }

        public bool CanEditDelegation(ClaimsPrincipal user, Guid? organisationId, Guid? bodyId)
        {
            var securityInfo = GetSecurityInformation(user);

            // Admins can do everything
            if (securityInfo.Roles.Contains(Role.OrganisationRegistryBeheerder) ||
                securityInfo.Roles.Contains(Role.Developer))
                return true;

            // Otherwise you can only edit what you are allowed
            return HasPermissionsForOrganisation(securityInfo, organisationId) ||
                HasPermissionsForBody(securityInfo, bodyId);
        }

        public bool CanAddBody(ClaimsPrincipal user, Guid? organisationId)
        {
            var securityInfo = GetSecurityInformation(user);

            // Admins can do everything
            if (securityInfo.Roles.Contains(Role.OrganisationRegistryBeheerder) ||
                securityInfo.Roles.Contains(Role.OrgaanBeheerder) ||
                securityInfo.Roles.Contains(Role.Developer))
                return true;

            // Otherwise you can only add to organisations you are allowed
            return HasPermissionsForOrganisation(securityInfo, organisationId);
        }

        public bool CanEditBody(ClaimsPrincipal user, Guid bodyId)
        {
            var securityInfo = GetSecurityInformation(user);

            // Admins can do everything
            if (securityInfo.Roles.Contains(Role.OrganisationRegistryBeheerder) ||
                securityInfo.Roles.Contains(Role.OrgaanBeheerder) ||
                securityInfo.Roles.Contains(Role.Developer))
                return true;

            // Otherwise you can only add to organisations you are allowed
            return HasPermissionsForBody(securityInfo, bodyId);
        }

        private static bool HasPermissionsForOrganisation(SecurityInformation securityInfo, Guid? organisationId)
        {
            if (!organisationId.HasValue)
                return false;

            return
                securityInfo.Roles.Contains(Role.OrganisatieBeheerder) &&
                securityInfo.OrganisationIds.Contains(organisationId.Value);
        }

        private static bool HasPermissionsForBody(SecurityInformation securityInfo, Guid? bodyId)
        {
            if (!bodyId.HasValue)
                return false;

            return
                securityInfo.Roles.Contains(Role.OrganisatieBeheerder) &&
                securityInfo.BodyIds.Contains(bodyId.Value);
        }

        public SecurityInformation GetSecurityInformation(ClaimsPrincipal user)
        {
            user = user ?? new ClaimsPrincipal();

            var firstName = user.GetClaim(ClaimTypes.GivenName);
            var name = user.GetClaim(ClaimTypes.Surname);

            var roles = user
                .GetClaims(ClaimTypes.Role)
                .Where(role => _roleMapping.ContainsKey(role))
                .Select(role => _roleMapping[role])
                .ToList();

            var organisations = user.GetClaims(ClaimOrganisation).Select(s => s.ToUpperInvariant());
            var organisationSecurity = GetSecurityInformation(organisations);

            return new SecurityInformation(
                $"{firstName} {name}",
                roles,
                organisationSecurity.OvoNumbers,
                organisationSecurity.OrganisationIds,
                organisationSecurity.BodyIds);
        }

        public Role[] GetRoles(ClaimsPrincipal principal)
        {
            return principal
                .GetClaims(ClaimTypes.Role)
                .Where(role => _roleMapping.ContainsKey(role))
                .Select(role => _roleMapping[role])
                .ToArray();
        }

        public IUser GetUser(ClaimsPrincipal? principal)
        {
            if (principal == null)
                throw new Exception("Could not determine current user");

            var firstName = principal.FindFirst(ClaimTypes.GivenName);
            if (firstName == null)
                throw new Exception("Could not determine current user's first name");

            var lastName = principal.FindFirst(ClaimTypes.Surname);
            if (lastName == null)
                throw new Exception("Could not determine current user's last name");

            var acmId = principal.FindFirst(OrganisationRegistryClaims.ClaimAcmId);
            if (acmId == null)
                throw new Exception("Could not determine current user's acm id");

            var ip = principal.FindFirst(OrganisationRegistryClaims.ClaimIp);

            return new User(
                firstName.Value,
                lastName.Value,
                acmId.Value,
                ip?.Value,
                GetRoles(principal));
        }

        private OrganisationSecurityInformation GetSecurityInformation(IEnumerable<string> ovoNumbers)
        {
            var organisationTrees = _context
                .OrganisationTreeList
                .AsQueryable()
                .Where(x => ovoNumbers.Contains(x.OvoNumber))
                .Select(x => x.OrganisationTree)
                .ToList()
                .SelectMany(x => x.Split(new[] {"|"}, StringSplitOptions.RemoveEmptyEntries))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var organisationIds = _context
                .OrganisationDetail
                .AsQueryable()
                .Where(x => organisationTrees.Contains(x.OvoNumber))
                .Select(x => x.Id)
                .Distinct()
                .ToList();

            var bodyIds = _context
                .ActiveBodyOrganisationList
                .AsQueryable()
                .Where(x => organisationIds.Contains(x.OrganisationId))
                .Select(x => x.BodyId)
                .Distinct()
                .ToList();

            return new OrganisationSecurityInformation(organisationTrees, organisationIds, bodyIds);
        }

        private class OrganisationSecurityInformation
        {
            public IList<string> OvoNumbers { get; }

            public IList<Guid> OrganisationIds { get; }

            public IList<Guid> BodyIds { get; }

            public OrganisationSecurityInformation(IList<string> ovoNumbers, IList<Guid> organisationIds, IList<Guid> bodyIds)
            {
                OvoNumbers = ovoNumbers;
                OrganisationIds = organisationIds;
                BodyIds = bodyIds;
            }
        }
    }
}
