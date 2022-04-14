namespace OrganisationRegistry.Api.Security
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
    using Microsoft.EntityFrameworkCore;
    using OrganisationRegistry.Configuration;
    using OrganisationRegistry.Infrastructure;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Security;
    using SqlServer;

    public class SecurityService : ISecurityService
    {
        private const string ClaimOrganisation = "urn:be:vlaanderen:wegwijs:organisation";

        private readonly ICache<OrganisationSecurityInformation> _cache;
        private readonly IOrganisationRegistryConfiguration _configuration;

        private readonly IContextFactory _contextFactory;

        private readonly Dictionary<string, Role> _roleMapping = new()
        {
            { Roles.AlgemeenBeheerder, Role.AlgemeenBeheerder },
            { Roles.VlimpersBeheerder, Role.VlimpersBeheerder },
            { Roles.DecentraalBeheerder, Role.DecentraalBeheerder },
            { Roles.RegelgevingBeheerder, Role.RegelgevingBeheerder },
            { Roles.OrgaanBeheerder, Role.OrgaanBeheerder },
            { Roles.Developer, Role.Developer },
            { Roles.AutomatedTask, Role.AutomatedTask }
        };

        public SecurityService(
            IContextFactory contextFactory,
            IOrganisationRegistryConfiguration configuration,
            ICache<OrganisationSecurityInformation> cache)
        {
            _contextFactory = contextFactory;
            _configuration = configuration;
            _cache = cache;
        }

        public async Task<bool> CanAddOrganisation(ClaimsPrincipal user, Guid? parentOrganisationId)
        {
            var securityInfo = await GetSecurityInformation(user);

            // Admins can do everything
            if (securityInfo.Roles.Contains(Role.AlgemeenBeheerder) ||
                securityInfo.Roles.Contains(Role.Developer))
                return true;

            // Otherwise you can only add to parents you are allowed
            return HasPermissionsForOrganisation(securityInfo, parentOrganisationId);
        }

        public async Task<bool> CanEditOrganisation(ClaimsPrincipal user, Guid organisationId)
        {
            var securityInfo = await GetSecurityInformation(user);

            // Admins can do everything
            if (securityInfo.Roles.Contains(Role.AlgemeenBeheerder) ||
                securityInfo.Roles.Contains(Role.Developer))
                return true;

            // Otherwise you can only edit what you are allowed
            return HasPermissionsForOrganisation(securityInfo, organisationId);
        }

        public async Task<bool> CanEditDelegation(ClaimsPrincipal user, Guid? organisationId, Guid? bodyId)
        {
            var securityInfo = await GetSecurityInformation(user);

            // Admins can do everything
            if (securityInfo.Roles.Contains(Role.AlgemeenBeheerder) ||
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
            if (securityInfo.Roles.Contains(Role.AlgemeenBeheerder) ||
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
            if (securityInfo.Roles.Contains(Role.AlgemeenBeheerder) ||
                securityInfo.Roles.Contains(Role.OrgaanBeheerder) ||
                securityInfo.Roles.Contains(Role.Developer))
                return true;

            // Otherwise you can only add to organisations you are allowed
            return HasPermissionsForBody(securityInfo, bodyId);
        }

        public async Task<SecurityInformation> GetSecurityInformation(ClaimsPrincipal? user)
        {
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
                return SecurityInformation.None();

            var firstName = user.GetRequiredClaim(ClaimTypes.GivenName);
            var name = user.GetRequiredClaim(ClaimTypes.Surname);
            var acmId = user.GetRequiredClaim(AcmIdmConstants.Claims.AcmId);

            var roles = user
                .GetClaims(ClaimTypes.Role)
                .Where(role => _roleMapping.ContainsKey(role))
                .Select(role => _roleMapping[role])
                .ToImmutableArray();

            var organisationSecurityInformation = await _cache.GetOrAdd(
                acmId,
                async () =>
                {
                    var organisations = GetOrganisations(user);
                    return await GetSecurityInformation(organisations);
                });

            return new SecurityInformation(
                $"{firstName} {name}",
                roles,
                organisationSecurityInformation.OvoNumbers,
                organisationSecurityInformation.OrganisationIds,
                organisationSecurityInformation.BodyIds);
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

            var acmId = principal.FindFirst(AcmIdmConstants.Claims.AcmId);
            if (acmId == null)
                throw new Exception("Could not determine current user's acm id");

            var ip = principal.FindFirst(AcmIdmConstants.Claims.Ip);

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
            if (principal == null || principal.Identity == null)
                throw new Exception("Could not determine current user");

            if (!principal.Identity.IsAuthenticated)
                return WellknownUsers.Nobody;

            var firstName = principal.GetRequiredClaim(ClaimTypes.GivenName);
            var lastName = principal.GetRequiredClaim(ClaimTypes.Surname);
            var acmId = principal.GetRequiredClaim(AcmIdmConstants.Claims.AcmId);
            var ip = principal.GetOptionalClaim(AcmIdmConstants.Claims.Ip);

            var securityInformation = await GetSecurityInformation(principal);

            return new User(
                firstName,
                lastName,
                acmId,
                ip,
                securityInformation.Roles.ToArray(),
                securityInformation.OvoNumbers);
        }

        // TODO: see how we can make SecurityService use IUser everywhere, io ClaimsPrincipal.
        public bool CanUseKeyType(IUser user, Guid keyTypeId)
        {
            if (user.IsInRole(Role.Developer) ||
                user.IsInRole(Role.AlgemeenBeheerder))
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
                user.IsInRole(Role.AlgemeenBeheerder))
                return true;

            if (_configuration.FormalNameLabelTypeId.Equals(labelTypeId) ||
                _configuration.FormalShortNameLabelTypeId.Equals(labelTypeId))
                return user.IsInRole(Role.VlimpersBeheerder);

            return true;
        }

        public void ExpireUserCache(string acmId)
        {
            _cache.Expire(acmId);
        }

        private static bool HasPermissionsForOrganisation(SecurityInformation securityInfo, Guid? organisationId)
        {
            if (!organisationId.HasValue)
                return false;

            return
                securityInfo.Roles.Contains(Role.DecentraalBeheerder) &&
                securityInfo.OrganisationIds.Contains(organisationId.Value);
        }

        private static bool HasPermissionsForBody(SecurityInformation securityInfo, Guid? bodyId)
        {
            if (!bodyId.HasValue)
                return false;

            return
                securityInfo.Roles.Contains(Role.DecentraalBeheerder) &&
                securityInfo.BodyIds.Contains(bodyId.Value);
        }

        private async Task<OrganisationSecurityInformation> GetSecurityInformation(ImmutableArray<string> ovoNumbers)
        {
            if (!ovoNumbers.Any())
                return new OrganisationSecurityInformation();

            await using var context = _contextFactory.Create();
            var organisationTrees =
                (await context
                    .OrganisationTreeList
                    .AsAsyncQueryable()
                    .Where(x => ovoNumbers.Contains(x.OvoNumber))
                    .Select(x => x.OrganisationTree ?? string.Empty)
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

        private static ImmutableArray<string> GetOrganisations(ClaimsPrincipal user)
        {
            return user.GetClaims(ClaimOrganisation)
                .Select(s => s.ToUpperInvariant())
                .ToImmutableArray();
        }
    }
}
