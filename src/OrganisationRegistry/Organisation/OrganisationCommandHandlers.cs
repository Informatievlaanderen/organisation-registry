namespace OrganisationRegistry.Organisation
{
    using System;
    using Building;
    using Capacity;
    using Commands;
    using ContactType;
    using FormalFramework;
    using Function;
    using Infrastructure.Commands;
    using Infrastructure.Domain;
    using KeyTypes;
    using LabelType;
    using Location;
    using LocationType;
    using Microsoft.Extensions.Logging;
    using OrganisationClassification;
    using OrganisationClassificationType;
    using OrganisationRelationType;
    using Person;
    using Purpose;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Infrastructure.Authorization;
    using RegulationSubTheme;
    using RegulationTheme;

    public class OrganisationCommandHandlers :
        BaseCommandHandler<OrganisationCommandHandlers>,
        ICommandHandler<CreateOrganisation>,
        ICommandHandler<UpdateOrganisationInfo>,
        ICommandHandler<AddOrganisationKey>,
        ICommandHandler<UpdateOrganisationKey>,
        ICommandHandler<AddOrganisationRegulation>,
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
        ICommandHandler<UpdateOrganisationOpeningHour>,
        ICommandHandler<TerminateOrganisation>
    {
        private readonly IOvoNumberGenerator _ovoNumberGenerator;
        private readonly IUniqueOvoNumberValidator _uniqueOvoNumberValidator;
        private readonly IDateTimeProvider _dateTimeProvider;
        private readonly IOrganisationRegistryConfiguration _organisationRegistryConfiguration;

        public OrganisationCommandHandlers(
            ILogger<OrganisationCommandHandlers> logger,
            ISession session,
            IOvoNumberGenerator ovoNumberGenerator,
            IUniqueOvoNumberValidator uniqueOvoNumberValidator,
            IDateTimeProvider dateTimeProvider,
            IOrganisationRegistryConfiguration organisationRegistryConfiguration) : base(logger, session)
        {
            _ovoNumberGenerator = ovoNumberGenerator;
            _uniqueOvoNumberValidator = uniqueOvoNumberValidator;
            _dateTimeProvider = dateTimeProvider;
            _organisationRegistryConfiguration = organisationRegistryConfiguration;
        }

        public async Task Handle(CreateOrganisation message)
        {
            if (_uniqueOvoNumberValidator.IsOvoNumberTaken(message.OvoNumber))
                throw new OvoNumberNotUniqueException();

            var ovoNumber = string.IsNullOrWhiteSpace(message.OvoNumber)
                ? _ovoNumberGenerator.GenerateNumber()
                : message.OvoNumber;

            var parentOrganisation =
                message.ParentOrganisationId != null
                    ? Session.Get<Organisation>(message.ParentOrganisationId)
                    : null;

            var purposes = message
                .Purposes
                .Select(purposeId => Session.Get<Purpose>(purposeId))
                .ToList();

            var organisation = new Organisation(
                message.OrganisationId,
                message.Name,
                ovoNumber,
                message.ShortName,
                message.Article,
                parentOrganisation,
                message.Description,
                purposes,
                message.ShowOnVlaamseOverheidSites,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)),
                new Period(new ValidFrom(message.OperationalValidFrom), new ValidTo(message.OperationalValidTo)),
                _dateTimeProvider);

            Session.Add(organisation);
            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateOrganisationInfo message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);

            organisation.ThrowIfTerminated(message.User);

            var purposes = message
                .Purposes
                .Select(purposeId => Session.Get<Purpose>(purposeId))
                .ToList();

            organisation.UpdateInfo(
                message.Name,
                message.Article,
                message.Description,
                message.ShortName,
                purposes,
                message.ShowOnVlaamseOverheidSites,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)),
                new Period(new ValidFrom(message.OperationalValidFrom), new ValidTo(message.OperationalValidTo)),
                _dateTimeProvider);

            await Session.Commit(message.User);
        }

        public async Task Handle(AddOrganisationParent message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.ThrowIfTerminated(message.User);

            var parentOrganisation = Session.Get<Organisation>(message.ParentOrganisationId);
            var validity = new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo));

            if (ParentTreeHasOrganisationInIt(organisation, validity, parentOrganisation, new List<Organisation>()))
                throw new CircularRelationException();

            organisation.AddParent(
                message.OrganisationOrganisationParentId,
                parentOrganisation,
                validity,
                _dateTimeProvider);

            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateOrganisationParent message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.ThrowIfTerminated(message.User);

            var parentOrganisation = Session.Get<Organisation>(message.ParentOrganisationId);
            var validity = new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo));

            if (ParentTreeHasOrganisationInIt(organisation, validity, parentOrganisation, new List<Organisation>()))
                throw new CircularRelationException();

            organisation.UpdateParent(
                message.OrganisationOrganisationParentId,
                parentOrganisation,
                validity,
                _dateTimeProvider);

            await Session.Commit(message.User);
        }

        public async Task Handle(AddOrganisationFormalFramework message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.ThrowIfTerminated(message.User);

            var formalFramework = Session.Get<FormalFramework>(message.FormalFrameworkId);
            var parentOrganisation = Session.Get<Organisation>(message.ParentOrganisationId);

            var validity = new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo));

            if (FormalFrameworkTreeHasOrganisationInIt(organisation, formalFramework, validity, parentOrganisation, new List<Organisation>()))
                throw new CircularRelationInFormalFrameworkException();

            organisation.AddFormalFramework(
                message.OrganisationFormalFrameworkId,
                formalFramework,
                parentOrganisation,
                validity,
                _dateTimeProvider);

            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateOrganisationFormalFramework message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.ThrowIfTerminated(message.User);

            var formalFramework = Session.Get<FormalFramework>(message.FormalFrameworkId);
            var parentOrganisation = Session.Get<Organisation>(message.ParentOrganisationId);

            var validity = new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo));

            if (FormalFrameworkTreeHasOrganisationInIt(organisation, formalFramework, validity, parentOrganisation, new List<Organisation>()))
                throw new CircularRelationInFormalFrameworkException();

            organisation.UpdateFormalFramework(
                message.OrganisationFormalFrameworkId,
                formalFramework,
                parentOrganisation,
                validity,
                _dateTimeProvider);

            await Session.Commit(message.User);
        }

        private bool ParentTreeHasOrganisationInIt(
            Organisation organisation, Period validity,
            Organisation parentOrganisation, ICollection<Organisation> alreadyCheckedOrganisations)
        {
            if (Equals(organisation, parentOrganisation))
                return true;

            var parentsInPeriod = parentOrganisation.ParentsInPeriod(validity).ToList();

            if (!parentsInPeriod.Any())
                return false;

            return parentsInPeriod
                .Select(parent => Session.Get<Organisation>(parent.ParentOrganisationId))
                .Where(organisation1 => !alreadyCheckedOrganisations.Contains(organisation1))
                .Any(organisation1 => ParentTreeHasOrganisationInIt(organisation, validity, organisation1, alreadyCheckedOrganisations.Concat(new List<Organisation> { parentOrganisation }).ToList()));
        }

        // Todo: move to organisation
        private bool FormalFrameworkTreeHasOrganisationInIt(
            Organisation organisation, FormalFramework formalFramework, Period validity,
            Organisation parentOrganisation, IEnumerable<Organisation> alreadyCheckedOrganisations)
        {
            if (Equals(organisation, parentOrganisation))
                return true;

            var parentsInPeriodForFormalFramework = parentOrganisation.ParentsInPeriod(formalFramework, validity).ToList();

            if (!parentsInPeriodForFormalFramework.Any())
                return false;

            return parentsInPeriodForFormalFramework
                .Select(parent => Session.Get<Organisation>(parent.ParentOrganisationId))
                .Where(organisation1 => !alreadyCheckedOrganisations.Contains(organisation1))
                .Any(organisation1 => FormalFrameworkTreeHasOrganisationInIt(organisation, formalFramework, validity, organisation1, alreadyCheckedOrganisations.Concat(new List<Organisation> { parentOrganisation }).ToList()));
        }

        public async Task Handle(AddOrganisationKey message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);

            var keyType = Session.Get<KeyType>(message.KeyTypeId);

            organisation.AddKey(
                message.OrganisationKeyId,
                keyType,
                message.KeyValue,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateOrganisationKey message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);

            var keyType = Session.Get<KeyType>(message.KeyTypeId);

            organisation.UpdateKey(
                message.OrganisationKeyId,
                keyType,
                message.Value,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            await Session.Commit(message.User);
        }


        public async Task Handle(AddOrganisationRegulation message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);

            var regulationTheme = message.RegulationThemeId != Guid.Empty ?
                Session.Get<RegulationTheme>(message.RegulationThemeId) : null;

            var regulationSubTheme = message.RegulationSubThemeId != Guid.Empty ?
                Session.Get<RegulationSubTheme>(message.RegulationSubThemeId) : null;

            organisation.AddRegulation(
                message.OrganisationRegulationId,
                regulationTheme,
                regulationSubTheme,
                message.Name,
                message.Url,
                message.Date,
                message.Description,
                message.DescriptionRendered,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateOrganisationRegulation message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);

            var regulationTheme = message.RegulationThemeId != Guid.Empty ?
                Session.Get<RegulationTheme>(message.RegulationThemeId) : null;

            var regulationSubTheme = message.RegulationSubThemeId != Guid.Empty ?
                Session.Get<RegulationSubTheme>(message.RegulationSubThemeId) : null;

            organisation.UpdateRegulation(
                message.OrganisationRegulationId,
                regulationTheme,
                regulationSubTheme,
                message.Name,
                message.Link,
                message.Date,
                message.Description,
                message.DescriptionRendered,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            await Session.Commit(message.User);
        }

        public async Task Handle(AddOrganisationCapacity message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.ThrowIfTerminated(message.User);

            var capacity = Session.Get<Capacity>(message.CapacityId);
            var person = message.PersonId != null ? Session.Get<Person>(message.PersonId) : null;
            var function = message.FunctionId != null ? Session.Get<FunctionType>(message.FunctionId) : null;
            var location = message.LocationId != null ? Session.Get<Location>(message.LocationId) : null;

            var contacts = message.Contacts.Select(contact =>
            {
                var contactType = Session.Get<ContactType>(contact.Key);
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

            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateOrganisationCapacity message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.ThrowIfTerminated(message.User);

            var capacity = Session.Get<Capacity>(message.CapacityId);
            var person = message.PersonId != null ? Session.Get<Person>(message.PersonId) : null;
            var function = message.FunctionTypeId != null ? Session.Get<FunctionType>(message.FunctionTypeId) : null;
            var location = message.LocationId != null ? Session.Get<Location>(message.LocationId) : null;

            var contacts = message.Contacts.Select(contact =>
            {
                var contactType = Session.Get<ContactType>(contact.Key);
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

            await Session.Commit(message.User);
        }

        public async Task Handle(AddOrganisationFunction message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.ThrowIfTerminated(message.User);

            var person = Session.Get<Person>(message.PersonId);
            var function = Session.Get<FunctionType>(message.FunctionTypeId);

            var contacts = message.Contacts.Select(contact =>
            {
                var contactType = Session.Get<ContactType>(contact.Key);
                return new Contact(contactType, contact.Value);
            }).ToList();

            organisation.AddFunction(
                message.OrganisationFunctionId,
                function,
                person,
                contacts,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateOrganisationFunction message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.ThrowIfTerminated(message.User);

            var person = Session.Get<Person>(message.PersonId);
            var function = Session.Get<FunctionType>(message.FunctionTypeId);

            var contacts = message.Contacts.Select(contact =>
            {
                var contactType = Session.Get<ContactType>(contact.Key);
                return new Contact(contactType, contact.Value);
            }).ToList();

            organisation.UpdateFunction(
                message.OrganisationFunctionId,
                function,
                person,
                contacts,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            await Session.Commit(message.User);
        }

        public async Task Handle(AddOrganisationRelation message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.ThrowIfTerminated(message.User);

            var relatedOrganisation = Session.Get<Organisation>(message.RelatedOrganisationId);
            var relation = Session.Get<OrganisationRelationType>(message.RelationTypeId);

            organisation.AddRelation(
                message.OrganisationRelationId,
                relation,
                relatedOrganisation,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateOrganisationRelation message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.ThrowIfTerminated(message.User);

            var relatedOrganisation = Session.Get<Organisation>(message.RelatedOrganisationId);
            var relation = Session.Get<OrganisationRelationType>(message.RelationTypeId);

            organisation.UpdateRelation(
                message.OrganisationRelationId,
                relation,
                relatedOrganisation,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            await Session.Commit(message.User);
        }

        public async Task Handle(AddOrganisationBuilding message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.ThrowIfTerminated(message.User);

            var building = Session.Get<Building>(message.BuildingId);

            organisation.AddBuilding(
                message.OrganisationBuildingId,
                building,
                message.IsMainBuilding,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)),
                _dateTimeProvider);

            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateOrganisationBuilding message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.ThrowIfTerminated(message.User);

            var building = Session.Get<Building>(message.BuildingId);

            organisation.UpdateBuilding(
                message.OrganisationBuildingId,
                building,
                message.IsMainBuilding,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)),
                _dateTimeProvider);

            await Session.Commit(message.User);
        }

        public async Task Handle(AddOrganisationLocation message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.ThrowIfTerminated(message.User);

            var location = Session.Get<Location>(message.LocationId);
            var locationType = message.LocationTypeId != null ? Session.Get<LocationType>(message.LocationTypeId) : null;

            KboV2Guards.ThrowIfRegisteredOffice(_organisationRegistryConfiguration, locationType);

            organisation.AddLocation(
                message.OrganisationLocationId,
                location,
                message.IsMainLocation,
                locationType,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)),
                _dateTimeProvider);

            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateOrganisationLocation message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.ThrowIfTerminated(message.User);

            var location = Session.Get<Location>(message.LocationId);
            var locationType = message.LocationTypeId != null ? Session.Get<LocationType>(message.LocationTypeId) : null;

            KboV2Guards.ThrowIfRegisteredOffice(_organisationRegistryConfiguration, locationType);

            organisation.UpdateLocation(
                message.OrganisationLocationId,
                location,
                message.IsMainLocation,
                locationType,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)),
                _dateTimeProvider);

            await Session.Commit(message.User);
        }

        public async Task Handle(AddOrganisationContact message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.ThrowIfTerminated(message.User);

            var contactType = Session.Get<ContactType>(message.ContactTypeId);

            organisation.AddContact(
                message.OrganisationContactId,
                contactType,
                message.ContactValue,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateOrganisationContact message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.ThrowIfTerminated(message.User);

            var contactType = Session.Get<ContactType>(message.ContactTypeId);

            organisation.UpdateContact(
                message.OrganisationContactId,
                contactType,
                message.Value,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            await Session.Commit(message.User);
        }

        public async Task Handle(AddOrganisationLabel message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.ThrowIfTerminated(message.User);

            var labelType = Session.Get<LabelType>(message.LabelTypeId);

            KboV2Guards.ThrowIfFormalName(_organisationRegistryConfiguration, labelType);

            organisation.AddLabel(
                message.OrganisationLabelId,
                labelType,
                message.LabelValue,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateOrganisationLabel message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.ThrowIfTerminated(message.User);

            var labelType = Session.Get<LabelType>(message.LabelTypeId);

            KboV2Guards.ThrowIfFormalName(_organisationRegistryConfiguration, labelType);

            organisation.UpdateLabel(
                message.OrganisationLabelId,
                labelType,
                message.Value,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            await Session.Commit(message.User);
        }

        public async Task Handle(AddOrganisationOrganisationClassification message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.ThrowIfTerminated(message.User);

            var organisationClassification = Session.Get<OrganisationClassification>(message.OrganisationClassificationId);
            var organisationClassificationType = Session.Get<OrganisationClassificationType>(message.OrganisationClassificationTypeId);

            organisation.AddOrganisationClassification(
                _organisationRegistryConfiguration,
                message.OrganisationOrganisationClassificationId,
                organisationClassificationType,
                organisationClassification,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateOrganisationOrganisationClassification message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.ThrowIfTerminated(message.User);

            var organisationClassification = Session.Get<OrganisationClassification>(message.OrganisationClassificationId);
            var organisationClassificationType = Session.Get<OrganisationClassificationType>(message.OrganisationClassificationTypeId);

            KboV2Guards.ThrowIfLegalForm(_organisationRegistryConfiguration, organisationClassificationType);

            organisation.UpdateOrganisationClassification(
                message.OrganisationOrganisationClassificationId,
                organisationClassificationType,
                organisationClassification,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            await Session.Commit(message.User);
        }

        public async Task Handle(AddOrganisationBankAccount message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.ThrowIfTerminated(message.User);

            var bankAccountNumber = BankAccountNumber.CreateWithExpectedValidity(message.BankAccountNumber, message.IsIban);
            var bankAccountBic = BankAccountBic.CreateWithExpectedValidity(message.Bic, message.IsBic);

            var validity = new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo));

            organisation.AddBankAccount(
                message.OrganisationBankAccountId,
                bankAccountNumber,
                bankAccountBic,
                validity);

            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateOrganisationBankAccount message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.ThrowIfTerminated(message.User);

            var bankAccountNumber = BankAccountNumber.CreateWithExpectedValidity(message.BankAccountNumber, message.IsIban);
            var bankAccountBic = BankAccountBic.CreateWithExpectedValidity(message.Bic, message.IsBic);

            var validity = new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo));

            organisation.UpdateBankAccount(
                message.OrganisationBankAccountId,
                bankAccountNumber,
                bankAccountBic,
                validity);

            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateMainBuilding message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.ThrowIfTerminated(message.User);

            organisation.UpdateMainBuilding(_dateTimeProvider.Today);

            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateMainLocation message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.ThrowIfTerminated(message.User);

            organisation.UpdateMainLocation(_dateTimeProvider.Today);

            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateOrganisationFormalFrameworkParents message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.ThrowIfTerminated(message.User);

            organisation.UpdateOrganisationFormalFrameworkParent(_dateTimeProvider.Today, message.FormalFrameworkId);

            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateCurrentOrganisationParent message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.ThrowIfTerminated(message.User);

            organisation.UpdateCurrentOrganisationParent(_dateTimeProvider.Today);

            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateRelationshipValidities message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.ThrowIfTerminated(message.User);

            organisation.UpdateRelationshipValidities(message.Date);

            await Session.Commit(message.User);
        }

        public async Task Handle(AddOrganisationOpeningHour message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.ThrowIfTerminated(message.User);

            organisation.AddOpeningHour(
                message.OrganisationOpeningHourId,
                message.Opens,
                message.Closes,
                message.DayOfWeek,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            await Session.Commit(message.User);
        }

        public async Task Handle(UpdateOrganisationOpeningHour message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.ThrowIfTerminated(message.User);

            organisation.UpdateOpeningHour(
                message.OrganisationOpeningHourId,
                message.Opens,
                message.Closes,
                message.DayOfWeek,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            await Session.Commit(message.User);
        }

        public async Task Handle(TerminateOrganisation message)
        {
            Guard.RequiresRole(message.User, Role.OrganisationRegistryBeheerder);

            var organisation = Session.Get<Organisation>(message.OrganisationId);

            organisation.TerminateOrganisation(message.DateOfTermination,
                _organisationRegistryConfiguration.OrganisationCapacityTypeIdsToTerminateEndOfNextYear,
                _organisationRegistryConfiguration.OrganisationClassificationTypeIdsToTerminateEndOfNextYear,
                _organisationRegistryConfiguration.FormalFrameworkIdsToTerminateEndOfNextYear,
                _dateTimeProvider,
                message.ForceKboTermination);

            await Session.Commit(message.User);
        }
    }
}
