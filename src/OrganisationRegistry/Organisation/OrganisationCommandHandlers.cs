namespace OrganisationRegistry.Organisation
{
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

    public class OrganisationCommandHandlers :
        BaseCommandHandler<OrganisationCommandHandlers>,
        ICommandHandler<CreateOrganisation>,
        ICommandHandler<UpdateOrganisationInfo>,
        ICommandHandler<AddOrganisationKey>,
        ICommandHandler<UpdateOrganisationKey>,
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

        public void Handle(CreateOrganisation message)
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
                parentOrganisation,
                message.Description,
                purposes,
                message.ShowOnVlaamseOverheidSites,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)),
                _dateTimeProvider);

            Session.Add(organisation);
            Session.Commit();
        }

        public void Handle(UpdateOrganisationInfo message)
        {
            var purposes = message
                .Purposes
                .Select(purposeId => Session.Get<Purpose>(purposeId))
                .ToList();

            var organisation = Session.Get<Organisation>(message.OrganisationId);

            organisation.UpdateInfo(
                message.Name,
                message.Description,
                message.ShortName,
                purposes,
                message.ShowOnVlaamseOverheidSites,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)),
                _dateTimeProvider);

            Session.Commit();
        }

        public void Handle(AddOrganisationParent message)
        {
            var parentOrganisation = Session.Get<Organisation>(message.ParentOrganisationId);
            var organisation = Session.Get<Organisation>(message.OrganisationId);

            var validity = new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo));

            if (ParentTreeHasOrganisationInIt(organisation, validity, parentOrganisation, new List<Organisation>()))
                throw new CircularRelationException();

            organisation.AddParent(
                message.OrganisationOrganisationParentId,
                parentOrganisation,
                validity,
                _dateTimeProvider);

            Session.Commit();
        }

        public void Handle(UpdateOrganisationParent message)
        {
            var parentOrganisation = Session.Get<Organisation>(message.ParentOrganisationId);
            var organisation = Session.Get<Organisation>(message.OrganisationId);

            var validity = new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo));

            if (ParentTreeHasOrganisationInIt(organisation, validity, parentOrganisation, new List<Organisation>()))
                throw new CircularRelationException();

            organisation.UpdateParent(
                message.OrganisationOrganisationParentId,
                parentOrganisation,
                validity,
                _dateTimeProvider);

            Session.Commit();
        }

        public void Handle(AddOrganisationFormalFramework message)
        {
            var formalFramework = Session.Get<FormalFramework>(message.FormalFrameworkId);
            var parentOrganisation = Session.Get<Organisation>(message.ParentOrganisationId);
            var organisation = Session.Get<Organisation>(message.OrganisationId);

            var validity = new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo));

            if (FormalFrameworkTreeHasOrganisationInIt(organisation, formalFramework, validity, parentOrganisation, new List<Organisation>()))
                throw new CircularRelationInFormalFrameworkException();

            organisation.AddFormalFramework(
                message.OrganisationFormalFrameworkId,
                formalFramework,
                parentOrganisation,
                validity,
                _dateTimeProvider);

            Session.Commit();
        }

        public void Handle(UpdateOrganisationFormalFramework message)
        {
            var formalFramework = Session.Get<FormalFramework>(message.FormalFrameworkId);
            var parentOrganisation = Session.Get<Organisation>(message.ParentOrganisationId);
            var organisation = Session.Get<Organisation>(message.OrganisationId);

            var validity = new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo));

            if (FormalFrameworkTreeHasOrganisationInIt(organisation, formalFramework, validity, parentOrganisation, new List<Organisation>()))
                throw new CircularRelationInFormalFrameworkException();

            organisation.UpdateFormalFramework(
                message.OrganisationFormalFrameworkId,
                formalFramework,
                parentOrganisation,
                validity,
                _dateTimeProvider);

            Session.Commit();
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

        public void Handle(AddOrganisationKey message)
        {
            var keyType = Session.Get<KeyType>(message.KeyTypeId);
            var organisation = Session.Get<Organisation>(message.OrganisationId);

            organisation.AddKey(
                message.OrganisationKeyId,
                keyType,
                message.KeyValue,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            Session.Commit();
        }

        public void Handle(UpdateOrganisationKey message)
        {
            var keyType = Session.Get<KeyType>(message.KeyTypeId);
            var organisation = Session.Get<Organisation>(message.OrganisationId);

            organisation.UpdateKey(
                message.OrganisationKeyId,
                keyType,
                message.Value,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            Session.Commit();
        }

        public void Handle(AddOrganisationCapacity message)
        {
            var capacity = Session.Get<Capacity>(message.CapacityId);
            var person = message.PersonId != null ? Session.Get<Person>(message.PersonId) : null;
            var function = message.FunctionId != null ? Session.Get<FunctionType>(message.FunctionId) : null;
            var location = message.LocationId != null ? Session.Get<Location>(message.LocationId) : null;
            var organisation = Session.Get<Organisation>(message.OrganisationId);

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

            Session.Commit();
        }

        public void Handle(UpdateOrganisationCapacity message)
        {
            var capacity = Session.Get<Capacity>(message.CapacityId);
            var person = message.PersonId != null ? Session.Get<Person>(message.PersonId) : null;
            var function = message.FunctionTypeId != null ? Session.Get<FunctionType>(message.FunctionTypeId) : null;
            var location = message.LocationId != null ? Session.Get<Location>(message.LocationId) : null;
            var organisation = Session.Get<Organisation>(message.OrganisationId);

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

            Session.Commit();
        }

        public void Handle(AddOrganisationFunction message)
        {
            var person = Session.Get<Person>(message.PersonId);
            var function = Session.Get<FunctionType>(message.FunctionTypeId);
            var organisation = Session.Get<Organisation>(message.OrganisationId);

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

            Session.Commit();
        }

        public void Handle(UpdateOrganisationFunction message)
        {
            var person = Session.Get<Person>(message.PersonId);
            var function = Session.Get<FunctionType>(message.FunctionTypeId);
            var organisation = Session.Get<Organisation>(message.OrganisationId);

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

            Session.Commit();
        }

        public void Handle(AddOrganisationRelation message)
        {
            var relatedOrganisation = Session.Get<Organisation>(message.RelatedOrganisationId);
            var relation = Session.Get<OrganisationRelationType>(message.RelationTypeId);
            var organisation = Session.Get<Organisation>(message.OrganisationId);

            organisation.AddRelation(
                message.OrganisationRelationId,
                relation,
                relatedOrganisation,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            Session.Commit();
        }

        public void Handle(UpdateOrganisationRelation message)
        {
            var relatedOrganisation = Session.Get<Organisation>(message.RelatedOrganisationId);
            var relation = Session.Get<OrganisationRelationType>(message.RelationTypeId);
            var organisation = Session.Get<Organisation>(message.OrganisationId);

            organisation.UpdateRelation(
                message.OrganisationRelationId,
                relation,
                relatedOrganisation,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            Session.Commit();
        }

        public void Handle(AddOrganisationBuilding message)
        {
            var building = Session.Get<Building>(message.BuildingId);
            var organisation = Session.Get<Organisation>(message.OrganisationId);

            organisation.AddBuilding(
                message.OrganisationBuildingId,
                building,
                message.IsMainBuilding,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)),
                _dateTimeProvider);

            Session.Commit();
        }

        public void Handle(UpdateOrganisationBuilding message)
        {
            var building = Session.Get<Building>(message.BuildingId);
            var organisation = Session.Get<Organisation>(message.OrganisationId);

            organisation.UpdateBuilding(
                message.OrganisationBuildingId,
                building,
                message.IsMainBuilding,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)),
                _dateTimeProvider);

            Session.Commit();
        }

        public void Handle(AddOrganisationLocation message)
        {
            var location = Session.Get<Location>(message.LocationId);
            var locationType = message.LocationTypeId != null ? Session.Get<LocationType>(message.LocationTypeId) : null;
            var organisation = Session.Get<Organisation>(message.OrganisationId);

            KboV2Guards.ThrowIfRegisteredOffice(_organisationRegistryConfiguration, locationType);

            organisation.AddLocation(
                message.OrganisationLocationId,
                location,
                message.IsMainLocation,
                locationType,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)),
                _dateTimeProvider);

            Session.Commit();
        }

        public void Handle(UpdateOrganisationLocation message)
        {
            var location = Session.Get<Location>(message.LocationId);
            var locationType = message.LocationTypeId != null ? Session.Get<LocationType>(message.LocationTypeId) : null;
            var organisation = Session.Get<Organisation>(message.OrganisationId);

            KboV2Guards.ThrowIfRegisteredOffice(_organisationRegistryConfiguration, locationType);

            organisation.UpdateLocation(
                message.OrganisationLocationId,
                location,
                message.IsMainLocation,
                locationType,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)),
                _dateTimeProvider);

            Session.Commit();
        }

        public void Handle(AddOrganisationContact message)
        {
            var contactType = Session.Get<ContactType>(message.ContactTypeId);
            var organisation = Session.Get<Organisation>(message.OrganisationId);

            organisation.AddContact(
                message.OrganisationContactId,
                contactType,
                message.ContactValue,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            Session.Commit();
        }

        public void Handle(UpdateOrganisationContact message)
        {
            var contactType = Session.Get<ContactType>(message.ContactTypeId);
            var organisation = Session.Get<Organisation>(message.OrganisationId);

            organisation.UpdateContact(
                message.OrganisationContactId,
                contactType,
                message.Value,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            Session.Commit();
        }

        public void Handle(AddOrganisationLabel message)
        {
            var labelType = Session.Get<LabelType>(message.LabelTypeId);
            var organisation = Session.Get<Organisation>(message.OrganisationId);

            KboV2Guards.ThrowIfFormalName(_organisationRegistryConfiguration, labelType);

            organisation.AddLabel(
                message.OrganisationLabelId,
                labelType,
                message.LabelValue,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            Session.Commit();
        }

        public void Handle(UpdateOrganisationLabel message)
        {
            var labelType = Session.Get<LabelType>(message.LabelTypeId);
            var organisation = Session.Get<Organisation>(message.OrganisationId);

            KboV2Guards.ThrowIfFormalName(_organisationRegistryConfiguration, labelType);

            organisation.UpdateLabel(
                message.OrganisationLabelId,
                labelType,
                message.Value,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            Session.Commit();
        }

        public void Handle(AddOrganisationOrganisationClassification message)
        {
            var organisationClassification = Session.Get<OrganisationClassification>(message.OrganisationClassificationId);
            var organisationClassificationType = Session.Get<OrganisationClassificationType>(message.OrganisationClassificationTypeId);
            var organisation = Session.Get<Organisation>(message.OrganisationId);

            organisation.AddOrganisationClassification(
                _organisationRegistryConfiguration,
                message.OrganisationOrganisationClassificationId,
                organisationClassificationType,
                organisationClassification,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            Session.Commit();
        }

        public void Handle(UpdateOrganisationOrganisationClassification message)
        {
            var organisationClassification = Session.Get<OrganisationClassification>(message.OrganisationClassificationId);
            var organisationClassificationType = Session.Get<OrganisationClassificationType>(message.OrganisationClassificationTypeId);
            var organisation = Session.Get<Organisation>(message.OrganisationId);

            KboV2Guards.ThrowIfLegalForm(_organisationRegistryConfiguration, organisationClassificationType);

            organisation.UpdateOrganisationClassification(
                message.OrganisationOrganisationClassificationId,
                organisationClassificationType,
                organisationClassification,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            Session.Commit();
        }

        public void Handle(AddOrganisationBankAccount message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);

            var bankAccountNumber = new BankAccountNumber(message.BankAccountNumber, message.IsIban);
            var bankAccountBic = new BankAccountBic(message.Bic, message.IsBic);

            var validity = new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo));

            organisation.AddBankAccount(
                message.OrganisationBankAccountId,
                bankAccountNumber,
                bankAccountBic,
                validity);

            Session.Commit();
        }

        public void Handle(UpdateOrganisationBankAccount message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);

            var bankAccountNumber = new BankAccountNumber(message.BankAccountNumber, message.IsIban);
            var bankAccountBic = new BankAccountBic(message.Bic, message.IsBic);

            var validity = new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo));

            organisation.UpdateBankAccount(
                message.OrganisationBankAccountId,
                bankAccountNumber,
                bankAccountBic,
                validity);

            Session.Commit();
        }

        public void Handle(UpdateMainBuilding message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.UpdateMainBuilding(_dateTimeProvider.Today);

            Session.Commit();
        }

        public void Handle(UpdateMainLocation message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.UpdateMainLocation(_dateTimeProvider.Today);

            Session.Commit();
        }

        public void Handle(UpdateOrganisationFormalFrameworkParents message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.UpdateOrganisationFormalFrameworkParent(_dateTimeProvider.Today, message.FormalFrameworkId);

            Session.Commit();
        }

        public void Handle(UpdateCurrentOrganisationParent message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.UpdateCurrentOrganisationParent(_dateTimeProvider.Today);

            Session.Commit();
        }

        public void Handle(UpdateRelationshipValidities message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);
            organisation.UpdateRelationshipValidities(message.Date);

            Session.Commit();
        }

        public void Handle(AddOrganisationOpeningHour message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);

            organisation.AddOpeningHour(
                message.OrganisationOpeningHourId,
                message.Opens,
                message.Closes,
                message.DayOfWeek,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            Session.Commit();
        }

        public void Handle(UpdateOrganisationOpeningHour message)
        {
            var organisation = Session.Get<Organisation>(message.OrganisationId);

            organisation.UpdateOpeningHour(
                message.OrganisationOpeningHourId,
                message.Opens,
                message.Closes,
                message.DayOfWeek,
                new Period(new ValidFrom(message.ValidFrom), new ValidTo(message.ValidTo)));

            Session.Commit();
        }
    }
}
