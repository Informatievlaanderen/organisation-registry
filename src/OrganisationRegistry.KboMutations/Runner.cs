namespace OrganisationRegistry.KboMutations
{
    using Configuration;
    using Info;
    using Infrastructure.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    public class Runner
    {
        private readonly ILogger<Runner> _logger;
        private readonly IKboMutationsFetcher _kboMutationsFetcher;
        private readonly IKboMutationsPersister _kboMutationsPersister;
        private readonly TogglesConfiguration _togglesConfiguration;
        private readonly KboMutationsConfiguration _kboMutationsConfiguration;

        public Runner(ILogger<Runner> logger,
            IOptions<TogglesConfiguration> togglesConfigurationOptions,
            IOptions<KboMutationsConfiguration> kboMutationsConfigurationOptions,
            IKboMutationsFetcher kboMutationsFetcher,
            IKboMutationsPersister kboMutationsPersister)
        {
            _logger = logger;
            _kboMutationsFetcher = kboMutationsFetcher;
            _kboMutationsPersister = kboMutationsPersister;
            _kboMutationsConfiguration = kboMutationsConfigurationOptions.Value;
            _togglesConfiguration = togglesConfigurationOptions.Value;
        }

        public bool Run()
        {
            _logger.LogInformation(ProgramInformation.Build(_kboMutationsConfiguration));

            if (!_togglesConfiguration.KboMutationsAvailable)
                return false;

            foreach (var file in _kboMutationsFetcher.GetKboMutationFiles())
            {
                _kboMutationsPersister.Persist(file.FullName, file.KboMutations);
                _kboMutationsFetcher.Archive(file);
            }

            return true;
        }
    }
}
