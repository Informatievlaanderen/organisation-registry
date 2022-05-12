namespace OrganisationRegistry.Organisation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Building;
using Capacity;
using Commands;
using ContactType;
using Exceptions;
using FormalFramework;
using Function;
using Handling;
using Handling.Authorization;
using Infrastructure.Authorization;
using Infrastructure.Commands;
using Infrastructure.Configuration;
using Infrastructure.Domain;
using KeyTypes;
using LabelType;
using Location;
using LocationType;
using Microsoft.Extensions.Logging;
using OrganisationClassification;
using OrganisationClassificationType;
using OrganisationRegistry.Exceptions;
using OrganisationRelationType;
using OrganisationTermination;
using Person;
using Purpose;
using RegulationSubTheme;
using RegulationTheme;

public class OrganisationCommandHandlers :
    BaseCommandHandler<OrganisationCommandHandlers>,
    // ICommandHandler<CreateOrganisation>,
    // ICommandHandler<UpdateOrganisationInfo>,
    // ICommandHandler<UpdateOrganisationInfoNotLimitedToVlimpers>,
    // ICommandHandler<UpdateOrganisationInfoLimitedToVlimpers>,
    // ICommandHandler<AddOrganisationKey>,
    // ICommandHandler<UpdateOrganisationKey>,
    // ICommandHandler<RemoveOrganisationKey>,
    // ICommandHandler<AddOrganisationRegulation>,
    ICommandHandler<UpdateOrganisationRegulation>,
    ICommandHandler<AddOrganisationBuilding>,
    ICommandHandler<UpdateOrganisationBuilding>,
    ICommandHandler<AddOrganisationLocation>,
    ICommandHandler<UpdateOrganisationLocation>,
    ICommandHandler<AddOrganisationContact>,
    ICommandHandler<UpdateOrganisationContact>,
    ICommandHandler<AddOrganisationLabel>,
    ICommandHandler<UpdateOrganisationLabel>,
    ICommandHandler<AddOrganisationOrganisationClassification>,
    ICommandHandler<UpdateOrganisationOrganisationClassification>,
    ICommandHandler<AddOrganisationFunction>,
    ICommandHandler<UpdateOrganisationFunction>,
    ICommandHandler<AddOrganisationCapacity>,
    ICommandHandler<UpdateOrganisationCapacity>,
    ICommandHandler<AddOrganisationParent>,
    ICommandHandler<UpdateOrganisationParent>,
    ICommandHandler<AddOrganisationFormalFramework>,
    ICommandHandler<UpdateOrganisationFormalFramework>,
    ICommandHandler<AddOrganisationBankAccount>,
    ICommandHandler<UpdateOrganisationBankAccount>,
    ICommandHandler<UpdateMainBuilding>,
    ICommandHandler<UpdateMainLocation>,
    ICommandHandler<UpdateOrganisationFormalFrameworkParents>,
    ICommandHandler<UpdateCurrentOrganisationParent>,
    ICommandHandler<UpdateRelationshipValidities>,
    ICommandHandler<AddOrganisationRelation>,
    ICommandHandler<UpdateOrganisationRelation>,
    ICommandHandler<AddOrganisationOpeningHour>,
    ICommandHandler<UpdateOrganisationOpeningHour>
    // ICommandHandler<TerminateOrganisation>
    // ICommandHandler<PlaceUnderVlimpersManagement>
    // ICommandHandler<ReleaseFromVlimpersManagement>
{
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly IOrganisationRegistryConfiguration _organisationRegistryConfiguration;
    private readonly IOvoNumberGenerator _ovoNumberGenerator;
    private readonly ISecurityService _securityService;
    private readonly IUniqueOvoNumberValidator _uniqueOvoNumberValidator;

    public OrganisationCommandHandlers(
        ILogger<OrganisationCommandHandlers> logger,
        ISession session,
        IOvoNumberGenerator ovoNumberGenerator,
        IUniqueOvoNumberValidator uniqueOvoNumberValidator,
        IDateTimeProvider dateTimeProvider,
        IOrganisationRegistryConfiguration organisationRegistryConfiguration,
        ISecurityService securityService) : base(logger, session)
    {
        _ovoNumberGenerator = ovoNumberGenerator;
        _uniqueOvoNumberValidator = uniqueOvoNumberValidator;
        _dateTimeProvider = dateTimeProvider;
        _organisationRegistryConfiguration = organisationRegistryConfiguration;
        _securityService = securityService;
    }

    public Task Handle(AddOrganisationBankAccount message)
        => UpdateHandler<Organisation>.For(message, Session)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(message.User);

                    var bankAccountNumber = BankAccountNumber.CreateWithExpectedValidity(
                        message.BankAccountNumber,
                        message.IsIban);
                    var bankAccountBic = BankAccountBic.CreateWithExpectedValidity(message.Bic, message.IsBic);

                    var validity = new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo));

                    organisation.AddBankAccount(
                        message.OrganisationBankAccountId,
                        bankAccountNumber,
                        bankAccountBic,
                        validity);
                });

    public Task Handle(AddOrganisationBuilding message)
        => UpdateHandler<Organisation>.For(message, Session)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(message.User);

                    var building = session.Get<Building>(message.BuildingId);

                    organisation.AddBuilding(
                        message.OrganisationBuildingId,
                        building,
                        message.IsMainBuilding,
                        new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)),
                        _dateTimeProvider);
                });

    public Task Handle(AddOrganisationCapacity message)
        => UpdateHandler<Organisation>.For(message, Session)
            .WithCapacityPolicy(_organisationRegistryConfiguration, message)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(message.User);

                    var capacity = session.Get<Capacity>(message.CapacityId);
                    var person = message.PersonId is { } ? session.Get<Person>(message.PersonId) : null;
                    var function = message.FunctionId is { } ? session.Get<FunctionType>(message.FunctionId) : null;
                    var location = message.LocationId is { } ? session.Get<Location>(message.LocationId) : null;

                    var contacts = message.Contacts.Select(
                        contact =>
                        {
                            var contactType = session.Get<ContactType>(contact.Key);
                            return new Contact(contactType, contact.Value);
                        }).ToList();

                    organisation.AddCapacity(
                        message.OrganisationCapacityId,
                        capacity,
                        person,
                        function,
                        location,
                        contacts,
                        new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)),
                        _dateTimeProvider);
                });

    public Task Handle(AddOrganisationContact message)
        => UpdateHandler<Organisation>.For(message, Session)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(message.User);

                    var contactType = session.Get<ContactType>(message.ContactTypeId);

                    organisation.AddContact(
                        message.OrganisationContactId,
                        contactType,
                        message.ContactValue,
                        new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));
                });

    public Task Handle(AddOrganisationFormalFramework message)
        => UpdateHandler<Organisation>.For(message, Session)
            .WithPolicy(
                organisation => new FormalFrameworkPolicy(
                    () => organisation.State.OvoNumber,
                    message.FormalFrameworkId,
                    _organisationRegistryConfiguration))
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(message.User);

                    var formalFramework = session.Get<FormalFramework>(message.FormalFrameworkId);
                    var parentOrganisation = session.Get<Organisation>(message.ParentOrganisationId);

                    var validity = new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo));

                    if (FormalFrameworkTreeHasOrganisationInIt(
                            organisation,
                            formalFramework,
                            validity,
                            parentOrganisation,
                            new List<Organisation>()))
                        throw new CircularRelationInFormalFramework();

                    organisation.AddFormalFramework(
                        message.OrganisationFormalFrameworkId,
                        formalFramework,
                        parentOrganisation,
                        validity,
                        _dateTimeProvider);
                });

    public Task Handle(AddOrganisationFunction message)
        => UpdateHandler<Organisation>.For(message, Session)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(message.User);

                    var person = session.Get<Person>(message.PersonId);
                    var function = session.Get<FunctionType>(message.FunctionTypeId);

                    var contacts = message.Contacts.Select(
                        contact =>
                        {
                            var contactType = session.Get<ContactType>(contact.Key);
                            return new Contact(contactType, contact.Value);
                        }).ToList();

                    organisation.AddFunction(
                        message.OrganisationFunctionId,
                        function,
                        person,
                        contacts,
                        new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));
                });

    // public Task Handle(AddOrganisationKey message)
    //     => UpdateHandler<Organisation>.For(message, Session)
    //         .WithKeyPolicy(_organisationRegistryConfiguration, message)
    //         .Handle(
    //             session =>
    //             {
    //                 var organisation = session.Get<Organisation>(message.OrganisationId);
    //
    //                 var keyType = session.Get<KeyType>(message.KeyTypeId);
    //
    //                 if (keyType.IsRemoved)
    //                     throw new CannotUseRemovedParameter("Informatiesysteem", keyType.Name);
    //
    //                 organisation.AddKey(
    //                     message.OrganisationKeyId,
    //                     keyType,
    //                     message.KeyValue,
    //                     new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)),
    //                     keyTypeId => _securityService.CanUseKeyType(message.User, keyTypeId));
    //             });

    public Task Handle(AddOrganisationLabel message)
        => UpdateHandler<Organisation>.For(message, Session)
            .WithLabelPolicy(_organisationRegistryConfiguration, message)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(message.User);

                    var labelType = session.Get<LabelType>(message.LabelTypeId);

                    KboV2Guards.ThrowIfFormalName(_organisationRegistryConfiguration, labelType);

                    organisation.AddLabel(
                        message.OrganisationLabelId,
                        labelType,
                        message.LabelValue,
                        new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)),
                        labelTypeId => _securityService.CanUseLabelType(message.User, labelTypeId));
                });

    public Task Handle(AddOrganisationLocation message)
        => UpdateHandler<Organisation>.For(message, Session)
            .RequiresBeheerderForOrganisationRegardlessOfVlimpers()
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(message.User);

                    var location = session.Get<Location>(message.LocationId);
                    var locationType = message.LocationTypeId != null
                        ? session.Get<LocationType>(message.LocationTypeId)
                        : null;

                    KboV2Guards.ThrowIfRegisteredOffice(_organisationRegistryConfiguration, locationType);

                    organisation.AddLocation(
                        message.OrganisationLocationId,
                        location,
                        message.IsMainLocation,
                        locationType,
                        new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)),
                        Source.Wegwijs,
                        _dateTimeProvider);
                });

    public Task Handle(AddOrganisationOpeningHour message)
        => UpdateHandler<Organisation>.For(message, Session)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(message.User);

                    organisation.AddOpeningHour(
                        message.OrganisationOpeningHourId,
                        message.Opens,
                        message.Closes,
                        message.DayOfWeek,
                        new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));
                });

    public Task Handle(AddOrganisationOrganisationClassification message)
        => UpdateHandler<Organisation>.For(message, Session)
            .WithOrganisationClassificationTypePolicy(_organisationRegistryConfiguration, message)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(message.User);

                    var organisationClassification =
                        session.Get<OrganisationClassification>(message.OrganisationClassificationId);
                    var organisationClassificationType =
                        session.Get<OrganisationClassificationType>(message.OrganisationClassificationTypeId);

                    organisation.AddOrganisationClassification(
                        _organisationRegistryConfiguration,
                        message.OrganisationOrganisationClassificationId,
                        organisationClassificationType,
                        organisationClassification,
                        new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));
                });

    public Task Handle(AddOrganisationParent message)
        => UpdateHandler<Organisation>.For(message, Session)
            .WithVlimpersPolicy()
            .Handle(
                session =>
                {
                    var parentOrganisation = session.Get<Organisation>(message.ParentOrganisationId);
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(message.User);

                    var validity = new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo));

                    ThrowIfCircularRelationshipDetected(organisation, validity, parentOrganisation);

                    organisation.AddParent(
                        message.OrganisationOrganisationParentId,
                        parentOrganisation,
                        validity,
                        _dateTimeProvider);
                });

    // public Task Handle(AddOrganisationRegulation message)
    //     => UpdateHandler<Organisation>.For(message, Session)
    //         .WithPolicy(_ => new RegulationPolicy())
    //         .Handle(
    //             session =>
    //             {
    //                 var organisation = session.Get<Organisation>(message.OrganisationId);
    //                 organisation.ThrowIfTerminated(message.User);
    //
    //                 var regulationTheme = message.RegulationThemeId != Guid.Empty
    //                     ? session.Get<RegulationTheme>(message.RegulationThemeId)
    //                     : null;
    //
    //                 var regulationSubTheme = message.RegulationSubThemeId != Guid.Empty
    //                     ? session.Get<RegulationSubTheme>(message.RegulationSubThemeId)
    //                     : null;
    //
    //                 organisation.AddRegulation(
    //                     message.OrganisationRegulationId,
    //                     regulationTheme,
    //                     regulationSubTheme,
    //                     message.Name,
    //                     message.Url,
    //                     new WorkRulesUrl(message.WorkRulesUrl),
    //                     message.Date,
    //                     message.Description,
    //                     message.DescriptionRendered,
    //                     new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));
    //             });

    public Task Handle(AddOrganisationRelation message)
        => UpdateHandler<Organisation>.For(message, Session)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(message.User);

                    var relatedOrganisation = session.Get<Organisation>(message.RelatedOrganisationId);
                    var relation = session.Get<OrganisationRelationType>(message.RelationTypeId);

                    organisation.AddRelation(
                        message.OrganisationRelationId,
                        relation,
                        relatedOrganisation,
                        new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));
                });
    //
    // public Task Handle(CreateOrganisation message)
    //     => message.ParentOrganisationId == null ? CreateTopLevelOrganisation(message) : CreateDaughter(message);

    // public Task Handle(PlaceUnderVlimpersManagement message)
    //     => UpdateHandler<Organisation>.For(message, Session)
    //         .RequiresAdmin()
    //         .Handle(
    //             session =>
    //             {
    //                 var organisation = session.Get<Organisation>(message.OrganisationId);
    //
    //                 organisation.PlaceUnderVlimpersManagement();
    //             });

    // public Task Handle(ReleaseFromVlimpersManagement message)
    //     => UpdateHandler<Organisation>.For(message, Session)
    //         .RequiresAdmin()
    //         .Handle(
    //             session =>
    //             {
    //                 var organisation = session.Get<Organisation>(message.OrganisationId);
    //
    //                 organisation.ReleaseFromVlimpersManagement();
    //             });

    // public Task Handle(RemoveOrganisationKey message)
    //     => UpdateHandler<Organisation>.For(message, Session)
    //         .RequiresOneOfRole(Role.AutomatedTask)
    //         .Handle(
    //             session =>
    //             {
    //                 var organisation = session.Get<Organisation>(message.OrganisationId);
    //
    //                 organisation.RemoveOrganisationKey(message.OrganisationKeyId);
    //             });

    // public Task Handle(TerminateOrganisation message)
    //     => UpdateHandler<Organisation>.For(message, Session)
    //         .WithVlimpersOnlyPolicy()
    //         .Handle(
    //             session =>
    //             {
    //                 var organisation = session.Get<Organisation>(message.OrganisationId);
    //
    //                 var fieldsToTerminateConfig = new OrganisationTerminationCalculator.FieldsToTerminateConfig(
    //                     _organisationRegistryConfiguration.Kbo.FormalFrameworkIdsToTerminateEndOfNextYear
    //                         ?.FirstOrDefault() ?? Guid.Empty,
    //                     _organisationRegistryConfiguration.Kbo.OrganisationCapacityIdsToTerminateEndOfNextYear
    //                         ?.FirstOrDefault() ?? Guid.Empty,
    //                     _organisationRegistryConfiguration.Kbo.OrganisationClassificationTypeIdsToTerminateEndOfNextYear
    //                         ?.FirstOrDefault() ?? Guid.Empty,
    //                     _organisationRegistryConfiguration.VlimpersKeyTypeId);
    //
    //                 organisation.TerminateOrganisation(
    //                     message.DateOfTermination,
    //                     fieldsToTerminateConfig,
    //                     _dateTimeProvider,
    //                     message.ForceKboTermination);
    //             });

    public Task Handle(UpdateCurrentOrganisationParent message)
        => UpdateHandler<Organisation>.For(message, Session)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(message.User);

                    organisation.UpdateCurrentOrganisationParent(_dateTimeProvider.Today);
                });

    public Task Handle(UpdateMainBuilding message)
        => UpdateHandler<Organisation>.For(message, Session)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(message.User);

                    organisation.UpdateMainBuilding(_dateTimeProvider.Today);
                });

    public Task Handle(UpdateMainLocation message)
        => UpdateHandler<Organisation>.For(message, Session)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(message.User);

                    organisation.UpdateMainLocation(_dateTimeProvider.Today);
                });

    public Task Handle(UpdateOrganisationBankAccount message)
        => UpdateHandler<Organisation>.For(message, Session)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(message.User);

                    var bankAccountNumber = BankAccountNumber.CreateWithExpectedValidity(
                        message.BankAccountNumber,
                        message.IsIban);
                    var bankAccountBic = BankAccountBic.CreateWithExpectedValidity(message.Bic, message.IsBic);

                    var validity = new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo));

                    organisation.UpdateBankAccount(
                        message.OrganisationBankAccountId,
                        bankAccountNumber,
                        bankAccountBic,
                        validity);
                });

    public Task Handle(UpdateOrganisationBuilding message)
        => UpdateHandler<Organisation>.For(message, Session)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(message.User);

                    var building = session.Get<Building>(message.BuildingId);

                    organisation.UpdateBuilding(
                        message.OrganisationBuildingId,
                        building,
                        message.IsMainBuilding,
                        new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)),
                        _dateTimeProvider);
                });

    public Task Handle(UpdateOrganisationCapacity message)
        => UpdateHandler<Organisation>.For(message, Session)
            .WithCapacityPolicy(_organisationRegistryConfiguration, message)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(message.User);

                    var capacity = session.Get<Capacity>(message.CapacityId);
                    var person = message.PersonId is { } ? session.Get<Person>(message.PersonId) : null;
                    var function = message.FunctionTypeId is { }
                        ? session.Get<FunctionType>(message.FunctionTypeId)
                        : null;
                    var location = message.LocationId is { } ? session.Get<Location>(message.LocationId) : null;

                    var contacts = message.Contacts.Select(
                        contact =>
                        {
                            var contactType = session.Get<ContactType>(contact.Key);
                            return new Contact(contactType, contact.Value);
                        }).ToList();

                    organisation.UpdateCapacity(
                        message.OrganisationCapacityId,
                        capacity,
                        person,
                        function,
                        location,
                        contacts,
                        new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)),
                        _dateTimeProvider);
                });

    public Task Handle(UpdateOrganisationContact message)
        => UpdateHandler<Organisation>.For(message, Session)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(message.User);

                    var contactType = session.Get<ContactType>(message.ContactTypeId);

                    organisation.UpdateContact(
                        message.OrganisationContactId,
                        contactType,
                        message.Value,
                        new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));
                });

    public Task Handle(UpdateOrganisationFormalFramework message)
        => UpdateHandler<Organisation>.For(message, Session)
            .WithPolicy(
                organisation => new FormalFrameworkPolicy(
                    () => organisation.State.OvoNumber,
                    message.FormalFrameworkId,
                    _organisationRegistryConfiguration))
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(message.User);

                    var formalFramework = session.Get<FormalFramework>(message.FormalFrameworkId);
                    var parentOrganisation = session.Get<Organisation>(message.ParentOrganisationId);

                    var validity = new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo));

                    if (FormalFrameworkTreeHasOrganisationInIt(
                            organisation,
                            formalFramework,
                            validity,
                            parentOrganisation,
                            new List<Organisation>()))
                        throw new CircularRelationInFormalFramework();

                    organisation.UpdateFormalFramework(
                        message.OrganisationFormalFrameworkId,
                        formalFramework,
                        parentOrganisation,
                        validity,
                        _dateTimeProvider);
                });

    public Task Handle(UpdateOrganisationFormalFrameworkParents message)
        => UpdateHandler<Organisation>.For(message, Session)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(message.User);

                    organisation.UpdateOrganisationFormalFrameworkParent(
                        _dateTimeProvider.Today,
                        message.FormalFrameworkId);
                });

    public Task Handle(UpdateOrganisationFunction message)
        => UpdateHandler<Organisation>.For(message, Session)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(message.User);

                    var person = session.Get<Person>(message.PersonId);
                    var function = session.Get<FunctionType>(message.FunctionTypeId);

                    var contacts = message.Contacts.Select(
                        contact =>
                        {
                            var contactType = session.Get<ContactType>(contact.Key);
                            return new Contact(contactType, contact.Value);
                        }).ToList();

                    organisation.UpdateFunction(
                        message.OrganisationFunctionId,
                        function,
                        person,
                        contacts,
                        new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));
                });

    // public Task Handle(UpdateOrganisationInfo message)
    //     => UpdateHandler<Organisation>.For(message, Session)
    //         .RequiresBeheerderForOrganisationButNotUnderVlimpersManagement()
    //         .Handle(
    //             session =>
    //             {
    //                 var organisation = session.Get<Organisation>(message.OrganisationId);
    //                 organisation.ThrowIfTerminated(message.User);
    //
    //                 var purposes = message
    //                     .Purposes
    //                     .Select(purposeId => session.Get<Purpose>(purposeId))
    //                     .ToList();
    //
    //                 organisation.UpdateInfo(
    //                     message.Name,
    //                     message.Article,
    //                     message.Description,
    //                     message.ShortName,
    //                     purposes,
    //                     message.ShowOnVlaamseOverheidSites,
    //                     new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)),
    //                     new Period(
    //                         new ValidFrom(message.OperationalValidFrom),
    //                         new ValidTo(message.OperationalValidTo)),
    //                     _dateTimeProvider);
    //             });

    // public Task Handle(UpdateOrganisationInfoLimitedToVlimpers message)
    //     => UpdateHandler<Organisation>.For(message, Session)
    //         .WithVlimpersOnlyPolicy()
    //         .Handle(
    //             session =>
    //             {
    //                 var organisation = session.Get<Organisation>(message.OrganisationId);
    //                 organisation.ThrowIfTerminated(message.User);
    //
    //                 organisation.UpdateVlimpersOrganisationInfo(
    //                     message.Article,
    //                     message.Name,
    //                     message.ShortName,
    //                     new Period(
    //                         message.ValidFrom,
    //                         message.ValidTo),
    //                     new Period(
    //                         message.OperationalValidFrom,
    //                         message.OperationalValidTo),
    //                     _dateTimeProvider);
    //             });

    // public Task Handle(UpdateOrganisationInfoNotLimitedToVlimpers message)
    //     => UpdateHandler<Organisation>.For(message, Session)
    //         .RequiresBeheerderForOrganisationRegardlessOfVlimpers()
    //         .Handle(
    //             session =>
    //             {
    //                 var organisation = session.Get<Organisation>(message.OrganisationId);
    //                 organisation.ThrowIfTerminated(message.User);
    //
    //                 var purposes = message
    //                     .Purposes
    //                     .Select(purposeId => session.Get<Purpose>(purposeId))
    //                     .ToList();
    //
    //                 organisation.UpdateInfoNotLimitedByVlimpers(
    //                     message.Description,
    //                     purposes,
    //                     message.ShowOnVlaamseOverheidSites);
    //             });

    // public Task Handle(UpdateOrganisationKey message)
    //     => UpdateHandler<Organisation>.For(message, Session)
    //         .WithKeyPolicy(_organisationRegistryConfiguration, message)
    //         .Handle(
    //             session =>
    //             {
    //                 var organisation = session.Get<Organisation>(message.OrganisationId);
    //
    //                 var keyType = session.Get<KeyType>(message.KeyTypeId);
    //
    //                 organisation.UpdateKey(
    //                     message.OrganisationKeyId,
    //                     keyType,
    //                     message.Value,
    //                     new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)),
    //                     keyTypeId => _securityService.CanUseKeyType(message.User, keyTypeId));
    //             });

    public Task Handle(UpdateOrganisationLabel message)
        => UpdateHandler<Organisation>.For(message, Session)
            .WithLabelPolicy(_organisationRegistryConfiguration, message)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(message.User);

                    var labelType = session.Get<LabelType>(message.LabelTypeId);

                    KboV2Guards.ThrowIfFormalName(_organisationRegistryConfiguration, labelType);

                    organisation.UpdateLabel(
                        message.OrganisationLabelId,
                        labelType,
                        message.Value,
                        new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));
                });

    public Task Handle(UpdateOrganisationLocation message)
        => UpdateHandler<Organisation>.For(message, Session)
            .RequiresBeheerderForOrganisationRegardlessOfVlimpers()
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(message.User);

                    var location = session.Get<Location>(message.LocationId);
                    var locationType = message.LocationTypeId != null
                        ? session.Get<LocationType>(message.LocationTypeId)
                        : null;

                    organisation.UpdateLocation(
                        message.OrganisationLocationId,
                        location,
                        message.IsMainLocation,
                        locationType,
                        new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)),
                        message.Source,
                        _dateTimeProvider);
                });

    public Task Handle(UpdateOrganisationOpeningHour message)
        => UpdateHandler<Organisation>.For(message, Session)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(message.User);

                    organisation.UpdateOpeningHour(
                        message.OrganisationOpeningHourId,
                        message.Opens,
                        message.Closes,
                        message.DayOfWeek,
                        new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));
                });

    public Task Handle(UpdateOrganisationOrganisationClassification message)
        => UpdateHandler<Organisation>.For(message, Session)
            .WithOrganisationClassificationTypePolicy(_organisationRegistryConfiguration, message)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(message.User);

                    var organisationClassification =
                        session.Get<OrganisationClassification>(message.OrganisationClassificationId);
                    var organisationClassificationType =
                        session.Get<OrganisationClassificationType>(message.OrganisationClassificationTypeId);

                    KboV2Guards.ThrowIfLegalForm(_organisationRegistryConfiguration, organisationClassificationType);

                    organisation.UpdateOrganisationClassification(
                        message.OrganisationOrganisationClassificationId,
                        organisationClassificationType,
                        organisationClassification,
                        new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));
                });

    public Task Handle(UpdateOrganisationParent message)
        => UpdateHandler<Organisation>.For(message, Session)
            .WithVlimpersPolicy()
            .Handle(
                session =>
                {
                    var parentOrganisation = session.Get<Organisation>(message.ParentOrganisationId);
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(message.User);

                    var validity = new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo));

                    if (ParentTreeHasOrganisationInIt(organisation, validity, parentOrganisation))
                        throw new CircularRelationshipDetected();

                    organisation.UpdateParent(
                        message.OrganisationOrganisationParentId,
                        parentOrganisation,
                        validity,
                        _dateTimeProvider);
                });

    public Task Handle(UpdateOrganisationRegulation message)
        => UpdateHandler<Organisation>.For(message, Session)
            .WithPolicy(_ => new RegulationPolicy())
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(message.User);

                    var regulationTheme = message.RegulationThemeId != Guid.Empty
                        ? session.Get<RegulationTheme>(message.RegulationThemeId)
                        : null;

                    var regulationSubTheme = message.RegulationSubThemeId != Guid.Empty
                        ? session.Get<RegulationSubTheme>(message.RegulationSubThemeId)
                        : null;

                    organisation.UpdateRegulation(
                        message.OrganisationRegulationId,
                        regulationTheme,
                        regulationSubTheme,
                        message.Name,
                        message.Link,
                        new WorkRulesUrl(message.WorkRulesUrl),
                        message.Date,
                        message.Description,
                        message.DescriptionRendered,
                        new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));
                });

    public Task Handle(UpdateOrganisationRelation message)
        => UpdateHandler<Organisation>.For(message, Session)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(message.User);

                    var relatedOrganisation = session.Get<Organisation>(message.RelatedOrganisationId);
                    var relation = session.Get<OrganisationRelationType>(message.RelationTypeId);

                    organisation.UpdateRelation(
                        message.OrganisationRelationId,
                        relation,
                        relatedOrganisation,
                        new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));
                });

    public Task Handle(UpdateRelationshipValidities message)
        => UpdateHandler<Organisation>.For(message, Session)
            .Handle(
                session =>
                {
                    var organisation = session.Get<Organisation>(message.OrganisationId);
                    organisation.ThrowIfTerminated(message.User);

                    organisation.UpdateRelationshipValidities(message.Date);
                });

    // private Task CreateDaughter(CreateOrganisation message)
    // {
    //     return Handler.For(message.User, Session)
    //         .WithVlimpersPolicy(Session.Get<Organisation>(message.ParentOrganisationId))
    //         .Handle(
    //             session =>
    //             {
    //                 var parentOrganisation =
    //                     message.ParentOrganisationId != null
    //                         ? session.Get<Organisation>(message.ParentOrganisationId)
    //                         : null;
    //
    //                 parentOrganisation?.ThrowIfUnauthorizedForVlimpers(message.User);
    //
    //                 if (_uniqueOvoNumberValidator.IsOvoNumberTaken(message.OvoNumber))
    //                     throw new OvoNumberNotUnique();
    //
    //                 var ovoNumber = string.IsNullOrWhiteSpace(message.OvoNumber)
    //                     ? _ovoNumberGenerator.GenerateNumber()
    //                     : message.OvoNumber;
    //
    //                 var purposes = message
    //                     .Purposes
    //                     .Select(purposeId => session.Get<Purpose>(purposeId))
    //                     .ToList();
    //
    //                 var organisation = Organisation.Create(
    //                     message.OrganisationId,
    //                     message.Name,
    //                     ovoNumber,
    //                     message.ShortName,
    //                     message.Article,
    //                     parentOrganisation,
    //                     message.Description,
    //                     purposes,
    //                     message.ShowOnVlaamseOverheidSites,
    //                     new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)),
    //                     new Period(
    //                         new ValidFrom(message.OperationalValidFrom),
    //                         new ValidTo(message.OperationalValidTo)),
    //                     _dateTimeProvider);
    //
    //                 session.Add(organisation);
    //             });
    // }


    // private Task CreateTopLevelOrganisation(CreateOrganisation message)
    // {
    //     return Handler.For(message.User, Session)
    //         .RequiresAdmin()
    //         .Handle(
    //             session =>
    //             {
    //                 if (_uniqueOvoNumberValidator.IsOvoNumberTaken(message.OvoNumber))
    //                     throw new OvoNumberNotUnique();
    //
    //                 var ovoNumber = string.IsNullOrWhiteSpace(message.OvoNumber)
    //                     ? _ovoNumberGenerator.GenerateNumber()
    //                     : message.OvoNumber;
    //
    //                 var purposes = message
    //                     .Purposes
    //                     .Select(purposeId => session.Get<Purpose>(purposeId))
    //                     .ToList();
    //
    //                 var organisation = Organisation.Create(
    //                     message.OrganisationId,
    //                     message.Name,
    //                     ovoNumber,
    //                     message.ShortName,
    //                     message.Article,
    //                     null,
    //                     message.Description,
    //                     purposes,
    //                     message.ShowOnVlaamseOverheidSites,
    //                     new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)),
    //                     new Period(
    //                         new ValidFrom(message.OperationalValidFrom),
    //                         new ValidTo(message.OperationalValidTo)),
    //                     _dateTimeProvider);
    //
    //                 session.Add(organisation);
    //             });
    // }


    private void ThrowIfCircularRelationshipDetected(
        Organisation organisation,
        Period validity,
        Organisation parentOrganisation)
    {
        var parentTreeHasOrganisationInIt =
            ParentTreeHasOrganisationInIt(
                organisation,
                validity,
                parentOrganisation);

        if (parentTreeHasOrganisationInIt)
            throw new CircularRelationshipDetected();
    }

    private bool ParentTreeHasOrganisationInIt(
        Organisation organisation,
        Period validity,
        Organisation parentOrganisation,
        ICollection<Organisation>? alreadyCheckedOrganisations = null)
    {
        alreadyCheckedOrganisations ??= new List<Organisation>();

        if (Equals(organisation, parentOrganisation))
            return true;

        var parentsInPeriod = parentOrganisation.ParentsInPeriod(validity).ToList();

        if (!parentsInPeriod.Any())
            return false;

        return parentsInPeriod
            .Select(parent => Session.Get<Organisation>(parent.ParentOrganisationId))
            .Where(organisation1 => !alreadyCheckedOrganisations.Contains(organisation1))
            .Any(
                organisation1 => ParentTreeHasOrganisationInIt(
                    organisation,
                    validity,
                    organisation1,
                    alreadyCheckedOrganisations.Concat(new List<Organisation> { parentOrganisation }).ToList()));
    }

    // Todo: move to organisation
    private bool FormalFrameworkTreeHasOrganisationInIt(
        Organisation organisation,
        FormalFramework formalFramework,
        Period validity,
        Organisation parentOrganisation,
        IEnumerable<Organisation> alreadyCheckedOrganisations)
    {
        if (Equals(organisation, parentOrganisation))
            return true;

        var parentsInPeriodForFormalFramework = parentOrganisation.ParentsInPeriod(formalFramework, validity).ToList();

        if (!parentsInPeriodForFormalFramework.Any())
            return false;

        return parentsInPeriodForFormalFramework
            .Select(parent => Session.Get<Organisation>(parent.ParentOrganisationId))
            .Where(organisation1 => !alreadyCheckedOrganisations.Contains(organisation1))
            .Any(
                organisation1 => FormalFrameworkTreeHasOrganisationInIt(
                    organisation,
                    formalFramework,
                    validity,
                    organisation1,
                    alreadyCheckedOrganisations.Concat(new List<Organisation> { parentOrganisation }).ToList()));
    }
}
