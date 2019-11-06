namespace OrganisationRegistry.CompensatingActions.Runner
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Autofac.Features.OwnedInstances;
    using Infrastructure.Domain;
    using Microsoft.Extensions.Logging;
    using Organisation;
    using Organisation.Events;
    using SqlServer.Infrastructure;

    public class Runner
    {
        private readonly ILogger<Runner> _logger;
        private readonly Func<Owned<OrganisationRegistryContext>> _contextFactory;
        private readonly ISession _session;

        public Runner(
            ILogger<Runner> logger,
            Func<Owned<OrganisationRegistryContext>> contextFactory,
            ISession session)
        {
            _logger = logger;
            _contextFactory = contextFactory;
            _session = session;
        }

        public bool Run()
        {
            var organisationIds = GetOrganisationStreamIds();
            _logger.LogInformation("{NumberOfOrganisations} organisations selected to possibly fix.", organisationIds.Count);

            var organisationsProcessed = 0;
            double previousPercent = 0;
            foreach (var organisationId in organisationIds)
            {
                var organisation = _session.Get<Organisation>(organisationId);
                var bugFixResult = organisation.Bugfix_20170330_ClearMainLocationFix(new DateTimeProvider());

                if (!string.IsNullOrWhiteSpace(bugFixResult))
                    _logger.LogInformation("Bugfix applied to {Organisation}, {BugfixResult}", organisation.Name, bugFixResult);

                organisationsProcessed++;
                var percent = (float)Math.Floor(organisationsProcessed * 100f / organisationIds.Count);
                if (Math.Abs(percent - previousPercent) >= 10)
                {
                    Console.WriteLine($"{percent}% processed...");
                    previousPercent = percent;
                }
            }

            _session.Commit();

            return true;
        }

        private IList<Guid> GetOrganisationStreamIds()
        {
            using (var context = _contextFactory().Value)
            {
                return context
                    .Events
                    .Where(x => x.Name == typeof(OrganisationLocationAdded).AssemblyQualifiedName)
                    .Select(x => x.Id)
                    .Distinct()
                    .OrderBy(x => x)
                    .ToList();
            }
        }
    }
}
