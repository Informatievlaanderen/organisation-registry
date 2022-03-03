namespace OrganisationRegistry.Api.Security
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Caching;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
    using Microsoft.EntityFrameworkCore;
    using OrganisationRegistry.Configuration;
    using OrganisationRegistry.Infrastructure.Authorization;
    using SqlServer;

    public class SecurityService : ISecurityService
    {
        private const string ClaimOrganisation = "urn:be:vlaanderen:wegwijs:organisation";
        private readonly ObjectCache _cache;
        private readonly IOrganisationRegistryConfiguration _configuration;

        private readonly IContextFactory _contextFactory;

        private readonly Dictionary<string, Role> _roleMapping = new()
        {
            { Roles.OrganisationRegistryBeheerder, Role.OrganisationRegistryBeheerder },
            { Roles.VlimpersBeheerder, Role.VlimpersBeheerder },
            { Roles.OrganisatieBeheerder, Role.OrganisatieBeheerder },
            { Roles.OrgaanBeheerder, Role.OrgaanBeheerder },
            { Roles.Developer, Role.Developer },
            { Roles.AutomatedTask, Role.AutomatedTask }
        };

        public SecurityService(IContextFactory contextFactory, IOrganisationRegistryConfiguration configuration)
        {
            _contextFactory = contextFactory;
            _configuration = configuration;
            _cache = MemoryCache.Default;
        }

        public async Task<bool> CanAddOrganisation(ClaimsPrincipal user, Guid? parentOrganisationId)
        {
            var securityInfo = await GetSecurityInformation(user);

            // Admins can do everything
            if (securityInfo.Roles.Contains(Role.OrganisationRegistryBeheerder) ||
                securityInfo.Roles.Contains(Role.Developer))
                return true;

            // Otherwise you can only add to parents you are allowed
            return HasPermissionsForOrganisation(securityInfo, parentOrganisationId);
        }

        public async Task<bool> CanEditOrganisation(ClaimsPrincipal user, Guid organisationId)
        {
            var securityInfo = await GetSecurityInformation(user);

            // Admins can do everything
            if (securityInfo.Roles.Contains(Role.OrganisationRegistryBeheerder) ||
                securityInfo.Roles.Contains(Role.Developer))
                return true;

            // Otherwise you can only edit what you are allowed
            return HasPermissionsForOrganisation(securityInfo, organisationId);
        }

        public async Task<bool> CanEditDelegation(ClaimsPrincipal user, Guid? organisationId, Guid? bodyId)
        {
            var securityInfo = await GetSecurityInformation(user);

            // Admins can do everything
            if (securityInfo.Roles.Contains(Role.OrganisationRegistryBeheerder) ||
                securityInfo.Roles.Contains(Role.Developer))
                return true;

            // Otherwise you can only edit what you are allowed
            return HasPermissionsForOrganisation(securityInfo, organisationId) ||
                   HasPermissionsForBody(securityInfo, bodyId);
        }

        public async Task<bool> CanAddBody(ClaimsPrincipal user, Guid? organisationId)
        {
            var securityInfo = await GetSecurityInformation(user);

            // Admins can do everything
            if (securityInfo.Roles.Contains(Role.OrganisationRegistryBeheerder) ||
                securityInfo.Roles.Contains(Role.OrgaanBeheerder) ||
                securityInfo.Roles.Contains(Role.Developer))
                return true;

            // Otherwise you can only add to organisations you are allowed
            return HasPermissionsForOrganisation(securityInfo, organisationId);
        }

        public async Task<bool> CanEditBody(ClaimsPrincipal user, Guid bodyId)
        {
            var securityInfo = await GetSecurityInformation(user);

            // Admins can do everything
            if (securityInfo.Roles.Contains(Role.OrganisationRegistryBeheerder) ||
                securityInfo.Roles.Contains(Role.OrgaanBeheerder) ||
                securityInfo.Roles.Contains(Role.Developer))
                return true;

            // Otherwise you can only add to organisations you are allowed
            return HasPermissionsForBody(securityInfo, bodyId);
        }

        public async Task<SecurityInformation> GetSecurityInformation(ClaimsPrincipal user)
        {
            user = user ?? new ClaimsPrincipal();

            var firstName = user.GetClaim(ClaimTypes.GivenName);
            var name = user.GetClaim(ClaimTypes.Surname);
            var userId = user.GetClaim(OrganisationRegistryClaims.ClaimAcmId);
            var isAuthenticated = (bool)user.Identity?.IsAuthenticated;

            var roles = user
                .GetClaims(ClaimTypes.Role)
                .Where(role => _roleMapping.ContainsKey(role))
                .Select(role => _roleMapping[role])
                .ToList();

            var organisationSecurity = await GetOrganisationSecurityInformation(user, isAuthenticated, userId);

            return new SecurityInformation(
                $"{firstName} {name}",
                roles,
                organisationSecurity.OvoNumbers,
                organisationSecurity.OrganisationIds,
                organisationSecurity.BodyIds);
        }

        public async Task<IUser> GetRequiredUser(ClaimsPrincipal? principal)
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

            var securityInformation = await GetSecurityInformation(principal);

            return new User(
                firstName.Value,
                lastName.Value,
                acmId.Value,
                ip?.Value,
                securityInformation.Roles.ToArray(),
                securityInformation.OvoNumbers);
        }

        public async Task<IUser> GetUser(ClaimsPrincipal? principal)
        {
            if (principal == null)
                throw new Exception("Could not determine current user");

            var firstName = principal.FindFirst(ClaimTypes.GivenName);
            var lastName = principal.FindFirst(ClaimTypes.Surname);
            var acmId = principal.FindFirst(OrganisationRegistryClaims.ClaimAcmId);
            var ip = principal.FindFirst(OrganisationRegistryClaims.ClaimIp);

            var securityInformation = await GetSecurityInformation(principal);

            return new User(
                firstName?.Value,
                lastName?.Value,
                acmId?.Value,
                ip?.Value,
                securityInformation.Roles.ToArray(),
                securityInformation.OvoNumbers);
        }

        // TODO: see how we can make SecurityService use IUser everywhere, io ClaimsPrincipal.
        public bool CanUseKeyType(IUser user, Guid keyTypeId)
        {
            if (user.IsInRole(Role.Developer) ||
                user.IsInRole(Role.OrganisationRegistryBeheerder))
                return true;

            if (_configuration.OrafinKeyTypeId.Equals(keyTypeId))
                return user.IsInRole(Role.Orafin) ||
                       user.Organisations.Any(x => x.Equals(_configuration.OrafinOvoCode));
            // todo: instead of checking the organisations now, check them on creation of jwt.

            if (_configuration.VlimpersKeyTypeId.Equals(keyTypeId))
                return user.IsAuthorizedForVlimpersOrganisations;

            return true;
        }

        public bool CanUseLabelType(IUser user, Guid labelTypeId)
        {
            if (user.IsInRole(Role.Developer) ||
                user.IsInRole(Role.OrganisationRegistryBeheerder))
                return true;

            if (_configuration.FormalNameLabelTypeId.Equals(labelTypeId) ||
                _configuration.FormalShortNameLabelTypeId.Equals(labelTypeId))
                return user.IsInRole(Role.VlimpersBeheerder);

            return true;
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

        private async Task<OrganisationSecurityInformation> GetOrganisationSecurityInformation(
            ClaimsPrincipal user,
            bool isAuthenticated,
            string subject)
        {
            var maybeCachedSecurityInfo = isAuthenticated ? _cache.Get(subject) : null;
            if (isAuthenticated && maybeCachedSecurityInfo is OrganisationSecurityInformation cachedSecurityInfo)
            {
                return cachedSecurityInfo;
            }

            if (isAuthenticated)
            {
                var organisations = GetOrganisations(user);
                var organisationSecurity = await GetSecurityInformation(organisations);
                _cache.Set(
                    new CacheItem(subject, organisationSecurity),
                    new CacheItemPolicy
                    {
                        SlidingExpiration = TimeSpan.FromMinutes(1)
                    });
                return organisationSecurity;
            }

            return new OrganisationSecurityInformation();
        }

        private async Task<OrganisationSecurityInformation> GetSecurityInformation(IEnumerable<string> ovoNumbers)
        {
            if (!ovoNumbers.Any())
                return new OrganisationSecurityInformation();

            await using var context = _contextFactory.Create();
            var organisationTrees = (await context
                    .OrganisationTreeList
                    .AsAsyncQueryable()
                    .Where(x => ovoNumbers.Contains(x.OvoNumber))
                    .Select(x => x.OrganisationTree)
                    .ToListAsync())
                .SelectMany(x => x.Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries))
                .Distinct()
                .OrderBy(x => x)
                .ToList();

            var organisationIds = await context
                .OrganisationDetail
                .AsAsyncQueryable()
                .Where(x => organisationTrees.Contains(x.OvoNumber))
                .Select(x => x.Id)
                .Distinct()
                .ToListAsync();

            var bodyIds = await context
                .ActiveBodyOrganisationList
                .AsAsyncQueryable()
                .Where(x => organisationIds.Contains(x.OrganisationId))
                .Select(x => x.BodyId)
                .Distinct()
                .ToListAsync();

            return new OrganisationSecurityInformation(organisationTrees, organisationIds, bodyIds);
        }

        private static IEnumerable<string> GetOrganisations(ClaimsPrincipal user)
        {
            return user.GetClaims(ClaimOrganisation).Select(s => s.ToUpperInvariant());
        }

        private class OrganisationSecurityInformation
        {
            public OrganisationSecurityInformation()
            {
                OvoNumbers = new List<string>();
                OrganisationIds = new List<Guid>();
                BodyIds = new List<Guid>();
            }

            public OrganisationSecurityInformation(
                IList<string> ovoNumbers,
                IList<Guid> organisationIds,
                IList<Guid> bodyIds)
            {
                OvoNumbers = ovoNumbers;
                OrganisationIds = organisationIds;
                BodyIds = bodyIds;
            }

            public IList<string> OvoNumbers { get; }

            public IList<Guid> OrganisationIds { get; }

            public IList<Guid> BodyIds { get; }
        }
    }
}
