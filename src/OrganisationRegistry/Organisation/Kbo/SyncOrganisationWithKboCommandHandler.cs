namespace OrganisationRegistry.Organisation.Kbo;

using System;
using System.Threading.Tasks;
using Commands;
using Exceptions;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Configuration;
using Infrastructure.Domain;
using LabelType;
using Location;
using Location.Commands;
using LocationType;
using Microsoft.Extensions.Logging;
using OrganisationClassification;
using OrganisationClassificationType;

public class SyncOrganisationWithKboCommandHandler
    : BaseCommandHandler<SyncOrganisationWithKboCommandHandler>,
        ICommandEnvelopeHandler<SyncOrganisationWithKbo>
{
    private readonly IOrganisationRegistryConfiguration _organisationRegistryConfiguration;
    private readonly IKboOrganisationRetriever _kboOrganisationRetriever;
    private readonly IKboOrganisationClassificationRetriever _organisationClassificationRetriever;
    private readonly IKboLocationRetriever _locationRetriever;

    public SyncOrganisationWithKboCommandHandler(
        ILogger<SyncOrganisationWithKboCommandHandler> logger,
        IOrganisationRegistryConfiguration organisationRegistryConfiguration,
        ISession session,
        IKboOrganisationRetriever kboOrganisationRetriever,
        IKboOrganisationClassificationRetriever organisationClassificationRetriever,
        IKboLocationRetriever locationRetriever) : base(logger, session)
    {
        _organisationRegistryConfiguration = organisationRegistryConfiguration;
        _kboOrganisationRetriever = kboOrganisationRetriever;
        _organisationClassificationRetriever = organisationClassificationRetriever;
        _locationRetriever = locationRetriever;
    }

    public async Task Handle(ICommandEnvelope<SyncOrganisationWithKbo> envelope)
        => await SyncWithKbo(envelope.Command.OrganisationId, envelope.User, envelope.Command.KboSyncItemId);

    private async Task SyncWithKbo(OrganisationId organisationId, IUser user, Guid? kboSyncItemId)
    {
        var registeredOfficeLocationType =
            Session.Get<LocationType>(_organisationRegistryConfiguration.Kbo.KboV2RegisteredOfficeLocationTypeId);

        var formalNameLabelType = Session.Get<LabelType>(_organisationRegistryConfiguration.Kbo.KboV2FormalNameLabelTypeId);

        var legalFormOrganisationClassificationType =
            Session.Get<OrganisationClassificationType>(
                _organisationRegistryConfiguration
                    .Kbo.KboV2LegalFormOrganisationClassificationTypeId);

        var organisation = Session.Get<Organisation>(organisationId);

        var kboOrganisationResult =
            await _kboOrganisationRetriever.RetrieveOrganisation(user, organisation.KboState.KboNumber);

        if (kboOrganisationResult.HasErrors)
            throw new KboOrganisationNotFound(kboOrganisationResult.ErrorMessages);

        var kboOrganisation = kboOrganisationResult.Value;

        var location = GetOrAddLocations(kboOrganisation.Address);

        await Session.Commit(user);

        // IMPORTANT: Need to re-Get the organisation, otherwise the Session will not properly handle the events.
        organisation = Session.Get<Organisation>(organisationId);

        organisation.UpdateInfoFromKbo(kboOrganisation.FormalName.Value, kboOrganisation.ShortName.Value);

        organisation.UpdateKboRegisteredOfficeLocations(location, registeredOfficeLocationType);

        organisation.UpdateKboFormalNameLabel(kboOrganisation.FormalName, formalNameLabelType);

        organisation.UpdateKboLegalFormOrganisationClassification(
            _organisationClassificationRetriever,
            legalFormOrganisationClassificationType,
            kboOrganisation.LegalForm,
            guid => Session.Get<OrganisationClassification>(guid));

        organisation.UpdateKboBankAccount(kboOrganisation.BankAccounts);

        if (kboOrganisation.Termination != null)
            organisation.MarkTerminationFound(kboOrganisation.Termination);

        organisation.MarkAsSynced(kboSyncItemId);

        await Session.Commit(user);
    }

    private KboRegisteredOffice? GetOrAddLocations(IMagdaAddress? maybeAddress)
    {
        if (maybeAddress is not { } address)
            return null;

        var location = AddOrGetLocation(address);
        return new KboRegisteredOffice(location, address.ValidFrom, address.ValidTo);
    }

    private Location AddOrGetLocation(IMagdaAddress address)
    {
        var existingLocationId = _locationRetriever.RetrieveLocation(address);
        Location location;
        if (!existingLocationId.HasValue)
        {
            location = new Location(
                new LocationId(Guid.NewGuid()),
                null,
                new Address(
                    address.Street,
                    address.ZipCode,
                    address.City,
                    address.Country));

            Session.Add(location);
        }
        else
        {
            location = Session.Get<Location>(existingLocationId.Value);
        }

        return location;
    }
}
