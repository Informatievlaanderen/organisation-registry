namespace OrganisationRegistry.Organisation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Commands;
using Exceptions;
using Import;
using Infrastructure;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Domain;
using LabelType;
using Location;
using Location.Commands;
using LocationType;
using Microsoft.Extensions.Logging;
using OrganisationClassification;
using OrganisationClassificationType;
using Purpose;
using IOrganisationRegistryConfiguration = Infrastructure.Configuration.IOrganisationRegistryConfiguration;

public class KboOrganisationCommandHandlers :
    BaseCommandHandler<KboOrganisationCommandHandlers>,
    ICommandEnvelopeHandler<CreateOrganisationFromKbo>,
    ICommandEnvelopeHandler<CoupleOrganisationToKbo>,
    ICommandEnvelopeHandler<CancelCouplingWithKbo>,
    ICommandEnvelopeHandler<SyncOrganisationWithKbo>,
    ICommandEnvelopeHandler<SyncOrganisationTerminationWithKbo>
{
    private readonly IOrganisationRegistryConfiguration _organisationRegistryConfiguration;
    private readonly IOvoNumberGenerator _ovoNumberGenerator;
    private readonly IUniqueOvoNumberValidator _uniqueOvoNumberValidator;
    private readonly IUniqueKboValidator _uniqueKboValidator;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IKboOrganisationRetriever _kboOrganisationRetriever;
    private readonly IKboOrganisationClassificationRetriever _organisationClassificationRetriever;
    private readonly IKboLocationRetriever _locationRetriever;

    public KboOrganisationCommandHandlers(
        ILogger<KboOrganisationCommandHandlers> logger,
        IOrganisationRegistryConfiguration organisationRegistryConfiguration,
        ISession session,
        IOvoNumberGenerator ovoNumberGenerator,
        IUniqueOvoNumberValidator uniqueOvoNumberValidator,
        IUniqueKboValidator uniqueKboValidator,
        IDateTimeProvider dateTimeProvider,
        IKboOrganisationRetriever kboOrganisationRetriever,
        IKboOrganisationClassificationRetriever organisationClassificationRetriever,
        IKboLocationRetriever locationRetriever) : base(logger, session)
    {
        _organisationRegistryConfiguration = organisationRegistryConfiguration;
        _ovoNumberGenerator = ovoNumberGenerator;
        _uniqueOvoNumberValidator = uniqueOvoNumberValidator;
        _uniqueKboValidator = uniqueKboValidator;
        _dateTimeProvider = dateTimeProvider;
        _kboOrganisationRetriever = kboOrganisationRetriever;
        _organisationClassificationRetriever = organisationClassificationRetriever;
        _locationRetriever = locationRetriever;
    }

    private Organisation? GetParentOrganisation(CreateOrganisationFromKbo message)
        => message.ParentOrganisationId is { } parentOrganisationId
            ? Session.Get<Organisation>(parentOrganisationId)
            : null;

    private string GetOvoNumber(CreateOrganisationFromKbo message)
        => message.OvoNumber is { } ovoNumber && ovoNumber.IsNotEmptyOrWhiteSpace()
            ? ovoNumber
            : _ovoNumberGenerator.GenerateNumber();

    private async Task SyncWithKbo(OrganisationId organisationId, IUser user, Guid? kboSyncItemId)
    {
        var registeredOfficeLocationType =
            Session.Get<LocationType>(_organisationRegistryConfiguration.Kbo.KboV2RegisteredOfficeLocationTypeId);

        var formalNameLabelType = Session.Get<LabelType>(_organisationRegistryConfiguration.Kbo.KboV2FormalNameLabelTypeId);

        var legalFormOrganisationClassificationType =
            Session.Get<OrganisationClassificationType>(_organisationRegistryConfiguration
                .Kbo.KboV2LegalFormOrganisationClassificationTypeId);

        var organisation = Session.Get<Organisation>(organisationId);

        if (organisation.KboState.KboNumber == null)
            throw new OrganisationHasNoKboNumber(organisationId);

        var kboOrganisationResult =
            await _kboOrganisationRetriever.RetrieveOrganisation(user, organisation.KboState.KboNumber!);

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

    private KboRegisteredOffice? GetOrAddLocations(IMagdaAddress? address)
    {
        if (address == null)
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
            location = new Location(new LocationId(Guid.NewGuid()), null, new Address(
                address.Street, address.ZipCode, address.City, address.Country));

            Session.Add(location);
        }
        else
        {
            location = Session.Get<Location>(existingLocationId.Value);
        }

        return location;
    }

    private void AddLabel(Organisation organisation, IMagdaOrganisationResponse kboOrganisation)
    {
        var labelType = Session.Get<LabelType>(_organisationRegistryConfiguration.Kbo.KboV2FormalNameLabelTypeId);

        organisation.AddKboFormalNameLabel(
            Guid.NewGuid(),
            labelType,
            kboOrganisation.FormalName.Value,
            new Period(
                new ValidFrom(kboOrganisation.ValidFrom),
                new ValidTo()));
    }

    private void AddAddresses(Organisation organisation,
        KboRegisteredOffice? address,
        LocationType registeredOfficeLocationType)
    {
        if (address == null)
            return;

        organisation.AddKboRegisteredOfficeLocation(
            Guid.NewGuid(),
            address.Location,
            registeredOfficeLocationType,
            new Period(
                new ValidFrom(address.ValidFrom),
                new ValidTo(address.ValidTo)));
    }

    private void AddLegalForm(Organisation organisation, IMagdaLegalForm? legalForm,
        OrganisationClassificationType legalFormOrganisationClassificationType)
    {
        if (legalForm == null)
            return;

        var organisationClassificationId =
            _organisationClassificationRetriever
                .FetchOrganisationClassificationForLegalFormCode(legalForm.Code);

        if (organisationClassificationId == null)
            return;

        var organisationClassification =
            Session.Get<OrganisationClassification>(organisationClassificationId.Value);

        organisation.AddKboLegalFormOrganisationClassification(
            Guid.NewGuid(),
            legalFormOrganisationClassificationType,
            organisationClassification,
            new Period(
                new ValidFrom(legalForm.ValidFrom),
                new ValidTo()));
    }

    private static void AddBankAccounts(Organisation organisation, IEnumerable<IMagdaBankAccount> bankAccounts)
    {
        foreach (var bankAccount in bankAccounts)
        {
            organisation.AddKboBankAccount(
                Guid.NewGuid(),
                BankAccountNumber.CreateWithUnknownValidity(bankAccount.AccountNumber),
                BankAccountBic.CreateWithUnknownValidity(bankAccount.Bic),
                new Period(
                    new ValidFrom(bankAccount.ValidFrom),
                    new ValidTo(bankAccount.ValidTo)));
        }
    }

    public async Task Handle(ICommandEnvelope<CreateOrganisationFromKboNumber> envelope)
        => await CreateFromKbo(envelope.Command, envelope.User);

    public async Task Handle(ICommandEnvelope<CreateOrganisationFromKbo> envelope)
        => await CreateFromKbo(envelope.Command, envelope.User);

    private async Task CreateFromKbo(CreateOrganisationFromKbo command, IUser user)
    {
        var registeredOfficeLocationType =
            Session.Get<LocationType>(_organisationRegistryConfiguration.Kbo.KboV2RegisteredOfficeLocationTypeId);

        var legalFormOrganisationClassificationType = Session.Get<OrganisationClassificationType>(_organisationRegistryConfiguration.Kbo.KboV2LegalFormOrganisationClassificationTypeId);

        if (_uniqueOvoNumberValidator.IsOvoNumberTaken(command.OvoNumber))
            throw new OvoNumberNotUnique();

        var ovoNumber = GetOvoNumber(command);

        var kboOrganisationResult =
            _kboOrganisationRetriever.RetrieveOrganisation(user, command.KboNumber).GetAwaiter().GetResult();

        if (kboOrganisationResult.HasErrors)
            throw new KboOrganisationNotFound(kboOrganisationResult.ErrorMessages);

        var kboOrganisation = kboOrganisationResult.Value;

        if (_uniqueKboValidator.IsKboNumberTaken(command.KboNumber))
            throw new KboNumberNotUnique();

        if (kboOrganisation.Termination != null)
            throw new CannotCreateOrganisationBecauseKboOrganisationTerminated();

        var parentOrganisation = GetParentOrganisation(command);

        var purposes = command
            .Purposes?
            .Select(purposeId => Session.Get<Purpose>(purposeId))
            .ToList();

        var location = GetOrAddLocations(kboOrganisation.Address);

        await Session.Commit(user);

        var organisation = Organisation.CreateFromKbo(
            command,
            kboOrganisation,
            ovoNumber,
            parentOrganisation,
            purposes ?? new List<Purpose>(),
            _dateTimeProvider);

        Session.Add(organisation);

        AddBankAccounts(organisation, kboOrganisation.BankAccounts);

        AddLegalForm(organisation, kboOrganisation.LegalForm, legalFormOrganisationClassificationType);

        AddAddresses(organisation, location, registeredOfficeLocationType);

        AddLabel(organisation, kboOrganisation);

        await Session.Commit(user);
    }

    public async Task Handle(ICommandEnvelope<CoupleOrganisationToKbo> envelope)
    {
        var registeredOfficeLocationType =
            Session.Get<LocationType>(_organisationRegistryConfiguration.Kbo.KboV2RegisteredOfficeLocationTypeId);

        var legalFormOrganisationClassificationType = Session.Get<OrganisationClassificationType>(_organisationRegistryConfiguration.Kbo.KboV2LegalFormOrganisationClassificationTypeId);

        var kboOrganisationResult =
            _kboOrganisationRetriever.RetrieveOrganisation(envelope.User, envelope.Command.KboNumber).GetAwaiter().GetResult();

        if (kboOrganisationResult.HasErrors)
            throw new KboOrganisationNotFound(kboOrganisationResult.ErrorMessages);

        var kboOrganisation = kboOrganisationResult.Value;

        if (kboOrganisation.Termination != null)
            throw new CannotCoupleOrganisationBecauseKboOrganisationTerminated();

        var location = GetOrAddLocations(kboOrganisation.Address);

        var organisation = Session.Get<Organisation>(envelope.Command.OrganisationId);
        organisation.ThrowIfTerminated(envelope.User);

        organisation.CoupleToKbo(envelope.Command.KboNumber, _dateTimeProvider);

        if (_uniqueKboValidator.IsKboNumberTaken(envelope.Command.KboNumber))
            throw new KboNumberNotUnique();

        organisation.UpdateInfoFromKbo(kboOrganisation.FormalName.Value, kboOrganisation.ShortName.Value);

        AddBankAccounts(organisation, kboOrganisation.BankAccounts);

        AddLegalForm(organisation, kboOrganisation.LegalForm, legalFormOrganisationClassificationType);

        AddAddresses(organisation, location, registeredOfficeLocationType);

        AddLabel(organisation, kboOrganisation);

        await Session.Commit(envelope.User);
    }

    public async Task Handle(ICommandEnvelope<CancelCouplingWithKbo> envelope)
    {
        var organisation = Session.Get<Organisation>(envelope.Command.OrganisationId);

        organisation.CancelCouplingWithKbo();

        await Session.Commit(envelope.User);
    }

    public async Task Handle(ICommandEnvelope<SyncOrganisationWithKbo> envelope)
        => await SyncWithKbo(envelope.Command.OrganisationId, envelope.User, envelope.Command.KboSyncItemId);

    public async Task Handle(ICommandEnvelope<SyncOrganisationTerminationWithKbo> envelope)
    {
        await SyncWithKbo(envelope.Command.OrganisationId, envelope.User, null);

        var organisation = Session.Get<Organisation>(envelope.Command.OrganisationId);

        organisation.TerminateOrganisationBasedOnKboTermination();

        await Session.Commit(envelope.User);
    }
}

public class KboRegisteredOffice
{
    public Location Location { get; }
    public DateTime? ValidFrom { get; }
    public DateTime? ValidTo { get; }

    public KboRegisteredOffice(Location location, DateTime? validFrom, DateTime? validTo)
    {
        Location = location;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}
