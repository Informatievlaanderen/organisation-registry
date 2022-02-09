namespace OrganisationRegistry.KboMutations
{
    using Configuration;
    using Info;
    using Infrastructure;
    using Infrastructure.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using SqlServer;

    public class Runner
    {
        private readonly ILogger<Runner> _logger;
        private readonly IKboMutationsFetcher _kboMutationsFetcher;
        private readonly IKboMutationsPersister _kboMutationsPersister;
        private readonly IExternalIpFetcher _externalIpFetcher;
        private readonly IContextFactory _contextFactory;
        private readonly TogglesConfigurationSection _togglesConfiguration;
        private readonly KboMutationsConfiguration _kboMutationsConfiguration;

        public Runner(ILogger<Runner> logger,
            IOptions<TogglesConfigurationSection> togglesConfigurationOptions,
            IOptions<KboMutationsConfiguration> kboMutationsConfigurationOptions,
            IKboMutationsFetcher kboMutationsFetcher,
            IKboMutationsPersister kboMutationsPersister,
            IExternalIpFetcher externalIpFetcher,
            IContextFactory contextFactory)
        {
            _logger = logger;
            _kboMutationsFetcher = kboMutationsFetcher;
            _kboMutationsPersister = kboMutationsPersister;
            _externalIpFetcher = externalIpFetcher;
            _contextFactory = contextFactory;
            _kboMutationsConfiguration = kboMutationsConfigurationOptions.Value;
            _togglesConfiguration = togglesConfigurationOptions.Value;
        }

        public bool Run()
        {
            _logger.LogInformation(
                ProgramInformation.Build(
                    _kboMutationsConfiguration,
                    _externalIpFetcher.Fetch().GetAwaiter().GetResult()));

            if (!_togglesConfiguration.KboMutationsAvailable)
                return false;

            using var context = _contextFactory.Create();
            foreach (var file in _kboMutationsFetcher.GetKboMutationFiles())
            {
                _kboMutationsPersister.Persist(context, file.FullName, file.KboMutations);
                _kboMutationsFetcher.Archive(file);
            }

            return true;
        }
    }
}
