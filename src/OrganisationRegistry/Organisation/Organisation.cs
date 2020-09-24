namespace OrganisationRegistry.Organisation
{
    using Building;
    using Capacity;
    using ContactType;
    using Events;
    using FormalFramework;
    using Function;
    using Infrastructure.Domain;
    using Infrastructure.Events;
    using KeyTypes;
    using LabelType;
    using Location;
    using LocationType;
    using OrganisationClassification;
    using OrganisationClassificationType;
    using OrganisationRelationType;
    using Person;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Commands;
    using Purpose = Purpose.Purpose;

    public partial class Organisation : AggregateRoot
    {
        public string Name { get; private set; }
        public string OvoNumber { get; private set; }
        private string _shortName;
        private string _description;
        private Period _validity;
        private bool _showOnVlaamseOverheidSites;
        private bool _isActive;
        private List<Events.Purpose> _purposes;

        private readonly List<OrganisationKey> _organisationKeys;
        private readonly List<OrganisationContact> _organisationContacts;
        private readonly List<OrganisationLabel> _organisationLabels;
        private readonly List<OrganisationOrganisationClassification> _organisationOrganisationClassifications;
        private readonly List<OrganisationFunction> _organisationFunctionTypes;
        private readonly List<OrganisationRelation> _organisationRelations;
        private readonly List<OrganisationCapacity> _organisationCapacities;
        private readonly List<OrganisationParent> _organisationParents;
        private readonly List<OrganisationFormalFramework> _organisationFormalFrameworks;
        private readonly List<OrganisationBankAccount> _organisationBankAccounts;
        private readonly List<OrganisationOpeningHour> _organisationOpeningHours;
        private readonly Dictionary<Guid, OrganisationFormalFramework> _organisationFormalFrameworkParentsPerFormalFramework;
        private readonly OrganisationBuildings _organisationBuildings;
        private readonly OrganisationLocations _organisationLocations;
        private OrganisationBuilding _mainOrganisationBuilding;
        private OrganisationLocation _mainOrganisationLocation;
        private OrganisationParent _currentOrganisationParent;

        private OrganisationLocation? _kboRegisteredOffice;
        private OrganisationLabel? _kboFormalNameLabel;
        private OrganisationOrganisationClassification? _kboLegalFormOrganisationClassification;
        private readonly List<OrganisationBankAccount> _kboBankAccounts;
        private string _nameBeforeKboCoupling;
        private string _shortNameBeforeKboCoupling;
        private KboTermination? TerminationInKbo { get; set; }

        public KboNumber? KboNumber { get; private set; }
        public bool CoupledToKboFromCreation { get; set; }


        private bool HasKboNumber => KboNumber != null;

        private Organisation()
        {
            _organisationKeys = new List<OrganisationKey>();
            _organisationContacts = new List<OrganisationContact>();
            _organisationLabels = new List<OrganisationLabel>();
            _organisationFunctionTypes = new List<OrganisationFunction>();
            _organisationRelations = new List<OrganisationRelation>();
            _organisationCapacities = new List<OrganisationCapacity>();
            _organisationParents = new List<OrganisationParent>();
            _organisationFormalFrameworks = new List<OrganisationFormalFramework>();
            _organisationOrganisationClassifications = new List<OrganisationOrganisationClassification>();
            _organisationBuildings = new OrganisationBuildings();
            _organisationLocations = new OrganisationLocations();
            _organisationFormalFrameworkParentsPerFormalFramework = new Dictionary<Guid, OrganisationFormalFramework>();
            _organisationBankAccounts = new List<OrganisationBankAccount>();
            _organisationOpeningHours = new List<OrganisationOpeningHour>();
            _kboBankAccounts = new List<OrganisationBankAccount>();
        }

        public Organisation(
            OrganisationId id,
            string name,
            string ovoNumber,
            string shortName,
            Organisation parentOrganisation,
            string description,
            List<Purpose> purposes,
            bool showOnVlaamseOverheidSites,
            Period validity,
            IDateTimeProvider dateTimeProvider) : this()
        {
            ApplyChange(new OrganisationCreated(
                id,
                name,
                ovoNumber,
                shortName,
                description,
                purposes.Select(x => new Events.Purpose(x.Id, x.Name)).ToList(),
                showOnVlaamseOverheidSites,
                validity.Start,
                validity.End));

            if (validity.OverlapsWith(dateTimeProvider.Today))
                ApplyChange(new OrganisationBecameActive(Id));

            if (parentOrganisation == null)
                return;

            ApplyChange(new OrganisationParentAdded(
                Id,
                Id,
                parentOrganisation.Id,
                parentOrganisation.Name,
                null,
                null));

            ApplyChange(new ParentAssignedToOrganisation(
                Id,
                parentOrganisation.Id,
                Id));
        }

        public static Organisation CreateFromKbo(
            CreateKboOrganisation message,
            IMagdaOrganisationResponse kboOrganisation,
            string ovoNumber,
            Organisation parentOrganisation,
            List<Purpose> purposes,
            IDateTimeProvider dateTimeProvider)
        {
            return new Organisation(
                message.OrganisationId,
                message.KboNumber,
                kboOrganisation.FormalName.Value,
                ovoNumber,
                kboOrganisation.ShortName.Value,
                parentOrganisation,
                message.Description,
                purposes,
                message.ShowOnVlaamseOverheidSites,
                new Period(new ValidFrom(kboOrganisation.ValidFrom), new ValidTo(message.ValidTo)),
                dateTimeProvider);
        }

        private Organisation(
            OrganisationId id,
            KboNumber kboNumber,
            string name,
            string ovoNumber,
            string shortName,
            Organisation parentOrganisation,
            string description,
            List<Purpose> purposes,
            bool showOnVlaamseOverheidSites,
            Period validity,
            IDateTimeProvider dateTimeProvider) : this()
        {
            ApplyChange(new OrganisationCreatedFromKbo(
                id,
                kboNumber.ToDigitsOnly(),
                name,
                ovoNumber,
                shortName,
                description,
                purposes.Select(x => new Events.Purpose(x.Id, x.Name)).ToList(),
                showOnVlaamseOverheidSites,
                validity.Start,
                validity.End));

            if (validity.OverlapsWith(dateTimeProvider.Today))
                ApplyChange(new OrganisationBecameActive(Id));

            if (parentOrganisation == null)
                return;

            ApplyChange(new OrganisationParentAdded(
                Id,
                Id,
                parentOrganisation.Id,
                parentOrganisation.Name,
                null,
                null));

            ApplyChange(new ParentAssignedToOrganisation(
                Id,
                parentOrganisation.Id,
                Id));
        }

        // TODO: discuss => Can we make name a Value Object, and put validation there as well?

        public void UpdateInfo(
            string name,
            string description,
            string shortName,
            List<Purpose> purposes,
            bool showOnVlaamseOverheidSites,
            Period validity,
            IDateTimeProvider dateTimeProvider)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

            if (HasKboNumber)
            {
                KboV2Guards.ThrowIfChanged(Name, name);
                KboV2Guards.ThrowIfChanged(_shortName, shortName);
            }

            ApplyChange(new OrganisationInfoUpdated(
                Id,
                name,
                description,
                OvoNumber,
                shortName,
                purposes.Select(x => new Events.Purpose(x.Id, x.Name)).ToList(),
                showOnVlaamseOverheidSites,
                validity.Start,
                validity.End,
                Name,
                _description,
                _shortName,
                _purposes,
                _showOnVlaamseOverheidSites,
                _validity.Start,
                _validity.End));

            var validityOverlapsWithToday = validity.OverlapsWith(dateTimeProvider.Today);
            if (_isActive && !validityOverlapsWithToday)
                ApplyChange(new OrganisationBecameInactive(Id));

            if (!_isActive && validityOverlapsWithToday)
                ApplyChange(new OrganisationBecameActive(Id));
        }

        public void MarkTerminationFound(IMagdaTermination magdaTermination)
        {
            if (!HasKboNumber)
                throw new OrganisationNotCoupledWithKbo();

            var termination = KboTermination.FromMagda(magdaTermination);

            if (TerminationInKbo != null && termination.Equals(TerminationInKbo.Value))
                return;

            ApplyChange(new OrganisationTerminationFoundInKbo(Id, KboNumber!.ToDigitsOnly(), termination.Date, termination.Code, termination.Reason));
        }

        public void MarkAsSynced(Guid? kboSyncItemId)
        {
            if (kboSyncItemId.HasValue)
                ApplyChange(new OrganisationSyncedFromKbo(Id, kboSyncItemId));
            else
                ApplyChange(new OrganisationManuallySyncedWithKbo(Id));
        }

        public void UpdateInfoFromKbo(
            string kboOrganisationName,
            string kboOrganisationShortName)
        {
            if (kboOrganisationName == Name &&
                kboOrganisationShortName == _shortName)
                return;

            ApplyChange(new OrganisationInfoUpdatedFromKbo(
                Id,
                OvoNumber,
                kboOrganisationName,
                kboOrganisationShortName,
                Name,
                _shortName));
        }

        public void AddParent(
            Guid organisationOrganisationParentId,
            Organisation parentOrganisation,
            Period validity,
            IDateTimeProvider dateTimeProvider)
        {
            if (_organisationParents
                .Where(organisationParent => organisationParent.OrganisationOrganisationParentId != organisationOrganisationParentId)
                .Any(organisationParent => organisationParent.Validity.OverlapsWith(validity)))
                throw new OrganisationAlreadyCoupledToParentInThisPeriodException();

            ApplyChange(new OrganisationParentAdded(
                Id,
                organisationOrganisationParentId,
                parentOrganisation.Id,
                parentOrganisation.Name,
                validity.Start,
                validity.End));

            CheckIfCurrentParentChanged(
                new OrganisationParent(
                    organisationOrganisationParentId,
                    parentOrganisation.Id,
                    parentOrganisation.Name,
                    validity),
                dateTimeProvider.Today);
        }

        public void UpdateParent(
            Guid organisationOrganisationParentId,
            Organisation parentOrganisation,
            Period validity,
            IDateTimeProvider dateTimeProvider)
        {
            if (_organisationParents
                .Where(organisationParent => organisationParent.OrganisationOrganisationParentId != organisationOrganisationParentId)
                .Any(organisationParent => organisationParent.Validity.OverlapsWith(validity)))
                throw new OrganisationAlreadyCoupledToParentInThisPeriodException();

            var previousParentOrganisation = _organisationParents.Single(parent => parent.OrganisationOrganisationParentId == organisationOrganisationParentId);

            ApplyChange(new OrganisationParentUpdated(
                Id,
                organisationOrganisationParentId,
                parentOrganisation.Id,
                parentOrganisation.Name,
                validity.Start,
                validity.End,
                previousParentOrganisation.ParentOrganisationId,
                previousParentOrganisation.ParentOrganisationName,
                previousParentOrganisation.Validity.Start,
                previousParentOrganisation.Validity.End));

            CheckIfCurrentParentChanged(
                new OrganisationParent(
                    organisationOrganisationParentId,
                    parentOrganisation.Id,
                    parentOrganisation.Name,
                    validity),
                dateTimeProvider.Today);
        }

        public void AddFormalFramework(
            Guid organisationFormalFrameworkId,
            FormalFramework formalFramework,
            Organisation parentOrganisation,
            Period validity,
            IDateTimeProvider dateTimeProvider)
        {
            if (_organisationFormalFrameworks
                .Where(organisationFormalFramework => organisationFormalFramework.OrganisationFormalFrameworkId != organisationFormalFrameworkId)
                .Where(organisationFormalFramework => organisationFormalFramework.FormalFrameworkId == formalFramework.Id)
                .Any(organisationParent => organisationParent.Validity.OverlapsWith(validity)))
                throw new OrganisationAlreadyCoupledToFormalFrameworkParentInThisPeriodException();

            ApplyChange(new OrganisationFormalFrameworkAdded(
                Id,
                organisationFormalFrameworkId,
                formalFramework.Id,
                formalFramework.Name,
                parentOrganisation.Id,
                parentOrganisation.Name,
                validity.Start,
                validity.End));

            CheckIfCurrentFormalFrameworkParentChanged(
                new OrganisationFormalFramework(
                    organisationFormalFrameworkId,
                    formalFramework.Id,
                    formalFramework.Name,
                    parentOrganisation.Id,
                    parentOrganisation.Name,
                    validity),
                dateTimeProvider.Today);
        }

        public void UpdateFormalFramework(
            Guid organisationFormalFrameworkId,
            FormalFramework formalFramework,
            Organisation parentOrganisation,
            Period validity,
            IDateTimeProvider dateTimeProvider)
        {
            if (_organisationFormalFrameworks
                .Where(x => x.OrganisationFormalFrameworkId != organisationFormalFrameworkId)
                .Where(x => x.FormalFrameworkId == formalFramework.Id)
                .Any(organisationParent => organisationParent.Validity.OverlapsWith(validity)))
                throw new OrganisationAlreadyCoupledToFormalFrameworkParentInThisPeriodException();

            var previousParentOrganisation =
                _organisationFormalFrameworks
                    .Single(x => x.OrganisationFormalFrameworkId == organisationFormalFrameworkId);

            ApplyChange(new OrganisationFormalFrameworkUpdated(
                Id,
                organisationFormalFrameworkId,
                formalFramework.Id,
                formalFramework.Name,
                parentOrganisation.Id,
                parentOrganisation.Name,
                validity.Start,
                validity.End,
                previousParentOrganisation.ParentOrganisationId,
                previousParentOrganisation.ParentOrganisationName,
                previousParentOrganisation.Validity.Start,
                previousParentOrganisation.Validity.End));

            CheckIfCurrentFormalFrameworkParentChanged(
                new OrganisationFormalFramework(
                    organisationFormalFrameworkId,
                    formalFramework.Id,
                    formalFramework.Name,
                    parentOrganisation.Id,
                    parentOrganisation.Name,
                    validity),
                dateTimeProvider.Today);
        }

        public void AddKey(
            Guid organisationKeyId,
            KeyType keyType,
            string value,
            Period validity)
        {
            if (_organisationKeys
                .Where(organisationKey => organisationKey.KeyTypeId == keyType.Id)
                .Where(organisationKey => organisationKey.OrganisationKeyId != organisationKeyId)
                .Any(organisationKey => organisationKey.Validity.OverlapsWith(validity)))
                throw new KeyAlreadyCoupledToInThisPeriodException();

            ApplyChange(new OrganisationKeyAdded(
                Id,
                organisationKeyId,
                keyType.Id,
                keyType.Name,
                value,
                validity.Start,
                validity.End));
        }

        public void UpdateKey(
            Guid organisationKeyId,
            KeyType keyType,
            string value,
            Period validity)
        {
            if (_organisationKeys
                .Where(organisationKey => organisationKey.KeyTypeId == keyType.Id)
                .Where(organisationKey => organisationKey.OrganisationKeyId != organisationKeyId)
                .Any(organisationKey => organisationKey.Validity.OverlapsWith(validity)))
                throw new KeyAlreadyCoupledToInThisPeriodException();

            var previousOrganisationKey =
                _organisationKeys.Single(key => key.OrganisationKeyId == organisationKeyId);

            ApplyChange(new OrganisationKeyUpdated(
                Id,
                organisationKeyId,
                keyType.Id,
                keyType.Name,
                value,
                validity.Start,
                validity.End,
                previousOrganisationKey.KeyTypeId,
                previousOrganisationKey.KeyTypeName,
                previousOrganisationKey.Value,
                previousOrganisationKey.Validity.Start,
                previousOrganisationKey.Validity.Start));
        }

        public void AddFunction(
            Guid organisationFunctionId,
            FunctionType functionType,
            Person person,
            List<Contact> contacts,
            Period validity)
        {
            if (functionType == null)
                throw new ArgumentNullException(nameof(functionType));

            ApplyChange(new OrganisationFunctionAdded(
                Id,
                organisationFunctionId,
                functionType.Id,
                functionType.Name,
                person.Id,
                person.FullName,
                contacts.ToDictionary(x => x.ContactType.Id, x => x.Value),
                validity.Start,
                validity.End));
        }

        public void UpdateFunction(
            Guid organisationFunctionId,
            FunctionType functionType,
            Person person,
            List<Contact> contacts,
            Period validity)
        {
            if (functionType == null)
                throw new ArgumentNullException(nameof(functionType));

            var previousFunctionType = _organisationFunctionTypes.Single(organisationFunctionType =>
                organisationFunctionType.OrganisationFunctionId == organisationFunctionId);

            var newOrganisationFunctionType =
                new OrganisationFunction(
                    organisationFunctionId,
                    Id,
                    functionType.Id,
                    functionType.Name,
                    person.Id,
                    person.FullName,
                    contacts.ToDictionary(x => x.ContactType.Id, x => x.Value),
                    validity);

            ApplyChange(new OrganisationFunctionUpdated(
                Id,
                newOrganisationFunctionType.OrganisationFunctionId,
                newOrganisationFunctionType.FunctionId,
                newOrganisationFunctionType.FunctionName,
                newOrganisationFunctionType.PersonId,
                newOrganisationFunctionType.PersonName,
                newOrganisationFunctionType.Contacts,
                newOrganisationFunctionType.Validity.Start,
                newOrganisationFunctionType.Validity.End,
                previousFunctionType.FunctionId,
                previousFunctionType.FunctionName,
                previousFunctionType.PersonId,
                previousFunctionType.PersonName,
                previousFunctionType.Contacts,
                previousFunctionType.Validity.Start,
                previousFunctionType.Validity.End));
        }

        public void AddRelation(
            Guid organisationRelationId,
            OrganisationRelationType relation,
            Organisation relatedOrganisation,
            Period period)
        {
            if (relation == null)
                throw new ArgumentNullException(nameof(relation));

            if (_organisationRelations
                .Where(organisationRelation => organisationRelation.OrganisationRelationId != organisationRelationId)
                .Where(organisationRelation => organisationRelation.RelatedOrganisationId == relatedOrganisation.Id)
                .Any(organisationParent => organisationParent.Validity.OverlapsWith(period)))
                throw new OrganisationAlreadyLinkedToOrganisationInThisPeriodException();

            if (Id == relatedOrganisation.Id)
                throw new OrganisationCannotBeLinkedToItselfException();

            ApplyChange(new OrganisationRelationAdded(
                Id,
                Name,
                organisationRelationId,
                relatedOrganisation.Id,
                relatedOrganisation.Name,
                relation.Id,
                relation.Name,
                relation.InverseName,
                period.Start,
                period.End));
        }

        public void UpdateRelation(
            Guid organisationRelationId,
            OrganisationRelationType relation,
            Organisation relatedOrganisation,
            Period period)
        {
            if (relation == null)
                throw new ArgumentNullException(nameof(relation));

            if (_organisationRelations
                .Where(organisationRelation => organisationRelation.OrganisationRelationId != organisationRelationId)
                .Where(organisationRelation => organisationRelation.RelatedOrganisationId == relatedOrganisation.Id)
                .Any(organisationParent => organisationParent.Validity.OverlapsWith(period)))
                throw new OrganisationAlreadyLinkedToOrganisationInThisPeriodException();

            if (Id == relatedOrganisation.Id)
                throw new OrganisationCannotBeLinkedToItselfException();

            var previousOrganisationRelation = _organisationRelations.Single(organisationRelation =>
                organisationRelation.OrganisationRelationId == organisationRelationId);

            var newOrganisationRelation =
                new OrganisationRelation(
                    organisationRelationId,
                    Id,
                    Name,
                    relation.Id,
                    relation.Name,
                    relation.InverseName,
                    relatedOrganisation.Id,
                    relatedOrganisation.Name,
                    period);

            ApplyChange(new OrganisationRelationUpdated(
                Id,
                Name,
                newOrganisationRelation.OrganisationRelationId,
                newOrganisationRelation.RelatedOrganisationId,
                newOrganisationRelation.RelatedOrganisationName,
                newOrganisationRelation.RelationId,
                newOrganisationRelation.RelationName,
                newOrganisationRelation.RelationInverseName,
                newOrganisationRelation.Validity.Start,
                newOrganisationRelation.Validity.End,
                previousOrganisationRelation.RelatedOrganisationId,
                previousOrganisationRelation.RelatedOrganisationName,
                previousOrganisationRelation.RelationId,
                previousOrganisationRelation.RelationName,
                previousOrganisationRelation.RelationInverseName,
                previousOrganisationRelation.Validity.Start,
                previousOrganisationRelation.Validity.End));
        }

        public void AddCapacity(
            Guid organisationCapacityId,
            Capacity capacity,
            Person person,
            FunctionType functionType,
            Location location,
            List<Contact> contacts,
            Period validity,
            IDateTimeProvider dateTimeProvider)
        {
            ApplyChange(new OrganisationCapacityAdded(
                Id,
                organisationCapacityId,
                capacity.Id,
                capacity.Name,
                person?.Id,
                person?.FullName,
                functionType?.Id,
                functionType?.Name,
                location?.Id,
                location?.FormattedAddress,
                contacts.ToDictionary(x => x.ContactType.Id, x => x.Value),
                validity.Start,
                validity.End));

            if (validity.OverlapsWith(dateTimeProvider.Today))
                ApplyChange(new OrganisationCapacityBecameActive(
                    Id,
                    organisationCapacityId,
                    capacity.Id,
                    person?.Id,
                    functionType?.Id,
                    validity.Start));
        }

        public void UpdateCapacity(
            Guid organisationCapacityId,
            Capacity capacity,
            Person person,
            FunctionType functionType,
            Location location,
            List<Contact> contacts,
            Period validity,
            IDateTimeProvider dateTimeProvider)
        {
            var previousCapacity = _organisationCapacities.Single(organisationCapacity =>
                organisationCapacity.OrganisationCapacityId == organisationCapacityId);

            var newOrganisationCapacity =
                new OrganisationCapacity(
                    organisationCapacityId,
                    Id,
                    capacity.Id,
                    capacity.Name,
                    person?.Id,
                    person?.FullName,
                    functionType?.Id,
                    functionType?.Name,
                    location?.Id,
                    location?.FormattedAddress,
                    contacts.ToDictionary(x => x.ContactType.Id, x => x.Value),
                    validity,
                    previousCapacity.IsActive
                );

            ApplyChange(new OrganisationCapacityUpdated(
                Id,
                newOrganisationCapacity.OrganisationCapacityId,
                newOrganisationCapacity.CapacityId,
                newOrganisationCapacity.CapacityName,
                newOrganisationCapacity.PersonId,
                newOrganisationCapacity.PersonName,
                newOrganisationCapacity.FunctionTypeId,
                newOrganisationCapacity.FunctionTypeName,
                newOrganisationCapacity.LocationId,
                newOrganisationCapacity.LocationName,
                newOrganisationCapacity.Contacts,
                newOrganisationCapacity.Validity.Start,
                newOrganisationCapacity.Validity.End,
                previousCapacity.CapacityId,
                previousCapacity.CapacityName,
                previousCapacity.PersonId,
                previousCapacity.PersonName,
                previousCapacity.FunctionTypeId,
                previousCapacity.FunctionTypeName,
                previousCapacity.LocationId,
                previousCapacity.LocationName,
                previousCapacity.Contacts,
                previousCapacity.Validity.Start,
                previousCapacity.Validity.End,
                previousCapacity.IsActive));

            if (newOrganisationCapacity.ShouldBecomeActive(dateTimeProvider.Today))
                ApplyChange(new OrganisationCapacityBecameActive(
                    Id,
                    organisationCapacityId,
                    newOrganisationCapacity.CapacityId,
                    newOrganisationCapacity.PersonId,
                    newOrganisationCapacity.FunctionTypeId,
                    newOrganisationCapacity.Validity.Start));

            if (newOrganisationCapacity.ShouldBecomeInactive(dateTimeProvider.Today))
                ApplyChange(new OrganisationCapacityBecameInactive(
                    Id,
                    organisationCapacityId,
                    newOrganisationCapacity.CapacityId,
                    newOrganisationCapacity.PersonId,
                    newOrganisationCapacity.FunctionTypeId,
                    newOrganisationCapacity.Validity.End));
        }

        public void AddOrganisationClassification(
            IOrganisationRegistryConfiguration organisationRegistryConfiguration,
            Guid organisationOrganisationClassificationId,
            OrganisationClassificationType organisationClassificationType,
            OrganisationClassification organisationClassification,
            Period validity)
        {
            KboV2Guards.ThrowIfLegalForm(organisationRegistryConfiguration, organisationClassificationType);

            if (_organisationOrganisationClassifications
                .Where(x => x.OrganisationClassificationTypeId == organisationClassificationType.Id)
                .Where(x => x.OrganisationOrganisationClassificationId != organisationOrganisationClassificationId)
                .Any(x => x.Validity.OverlapsWith(validity)))
                throw new OrganisationClassificationTypeAlreadyCoupledToInThisPeriodException();

            ApplyChange(new OrganisationOrganisationClassificationAdded(
                Id,
                organisationOrganisationClassificationId,
                organisationClassificationType.Id,
                organisationClassificationType.Name,
                organisationClassification.Id,
                organisationClassification.Name,
                validity.Start,
                validity.End));
        }

        public void AddKboLegalFormOrganisationClassification(Guid organisationOrganisationClassificationId, OrganisationClassificationType organisationClassificationType, OrganisationClassification organisationClassification, Period validity)
        {
            ApplyChange(
                new KboLegalFormOrganisationOrganisationClassificationAdded(
                    Id,
                    organisationOrganisationClassificationId,
                    organisationClassificationType.Id,
                    organisationClassificationType.Name,
                    organisationClassification.Id, organisationClassification.Name,
                    validity.Start,
                    validity.End));
        }

        public void UpdateKboLegalFormOrganisationClassification(
            IKboOrganisationClassificationRetriever organisationClassificationRetriever,
            OrganisationClassificationType legalFormOrganisationClassificationType,
            IMagdaLegalForm? newLegalForm,
            Func<Guid, OrganisationClassification> getOrganisationClassification)
        {
            var newLegalFormClassificationId = newLegalForm == null
                ? null
                : organisationClassificationRetriever
                    .FetchOrganisationClassificationForLegalFormCode(newLegalForm.Code);

            if (newLegalFormClassificationId == _kboLegalFormOrganisationClassification?.OrganisationClassificationId)
                return;

            if (_kboLegalFormOrganisationClassification != null)
                ApplyChange(
                    new KboLegalFormOrganisationOrganisationClassificationRemoved(
                        Id,
                        _kboLegalFormOrganisationClassification.OrganisationOrganisationClassificationId,
                        _kboLegalFormOrganisationClassification.OrganisationClassificationTypeId,
                        _kboLegalFormOrganisationClassification.OrganisationClassificationTypeName,
                        _kboLegalFormOrganisationClassification.OrganisationClassificationId,
                        _kboLegalFormOrganisationClassification.OrganisationClassificationName,
                        _kboLegalFormOrganisationClassification.Validity.Start,
                        _kboLegalFormOrganisationClassification.Validity.End,
                        _kboLegalFormOrganisationClassification.Validity.End));

            if (newLegalFormClassificationId != null)
            {
                var newLegalFormOrganisationClassification =
                    getOrganisationClassification(newLegalFormClassificationId.Value);

                ApplyChange(
                    new KboLegalFormOrganisationOrganisationClassificationAdded(
                        Id,
                        Guid.NewGuid(),
                        legalFormOrganisationClassificationType.Id,
                        legalFormOrganisationClassificationType.Name,
                        newLegalFormOrganisationClassification.Id,
                        newLegalFormOrganisationClassification.Name,
                        newLegalForm!.ValidFrom,
                        new ValidTo()
                    ));
            }
        }

        public void UpdateOrganisationClassification(
            Guid organisationOrganisationClassificationId,
            OrganisationClassificationType organisationClassificationType,
            OrganisationClassification organisationClassification,
            Period validity)
        {
            if (organisationClassification == null)
                throw new ArgumentNullException(nameof(organisationClassification));

            if (_organisationOrganisationClassifications
                .Where(organisationOrganisationClassification => organisationOrganisationClassification.OrganisationClassificationTypeId == organisationClassificationType.Id)
                .Where(organisationOrganisationClassification => organisationOrganisationClassification.OrganisationOrganisationClassificationId != organisationOrganisationClassificationId)
                .Any(organisationOrganisationClassification => organisationOrganisationClassification.Validity.OverlapsWith(validity)))
                throw new OrganisationClassificationTypeAlreadyCoupledToInThisPeriodException();

            var previousOrganisationOrganisationClassification =
                _organisationOrganisationClassifications.Single(classification => classification.OrganisationOrganisationClassificationId == organisationOrganisationClassificationId);

            ApplyChange(new OrganisationOrganisationClassificationUpdated(
                Id,
                organisationOrganisationClassificationId,
                organisationClassificationType.Id,
                organisationClassificationType.Name,
                organisationClassification.Id,
                organisationClassification.Name,
                validity.Start,
                validity.End,
                previousOrganisationOrganisationClassification.OrganisationClassificationTypeId,
                previousOrganisationOrganisationClassification.OrganisationClassificationTypeName,
                previousOrganisationOrganisationClassification.OrganisationClassificationId,
                previousOrganisationOrganisationClassification.OrganisationClassificationName,
                previousOrganisationOrganisationClassification.Validity.Start,
                previousOrganisationOrganisationClassification.Validity.End));
        }

        public void AddContact(
            Guid organisationContactId,
            ContactType contactType,
            string contactValue,
            Period validity)
        {
            ApplyChange(new OrganisationContactAdded(
                Id,
                organisationContactId,
                contactType.Id,
                contactType.Name,
                contactValue,
                validity.Start,
                validity.End));
        }

        public void UpdateContact(
            Guid organisationContactId,
            ContactType contactType,
            string contactValue,
            Period validity)
        {
            var previousContact =
                _organisationContacts.Single(contact => contact.OrganisationContactId == organisationContactId);

            ApplyChange(new OrganisationContactUpdated(
                Id,
                organisationContactId,
                contactType.Id, contactType.Name,
                contactValue,
                validity.Start, validity.End,
                previousContact.ContactTypeId,
                previousContact.ContactTypeName,
                previousContact.Value,
                previousContact.Validity.Start,
                previousContact.Validity.End));
        }

        public void AddLabel(
            Guid organisationLabelId,
            LabelType labelType,
            string labelValue,
            Period validity)
        {
            if (_organisationLabels
                .Where(organisationLabel => organisationLabel.LabelTypeId == labelType.Id)
                .Where(organisationLabel => organisationLabel.OrganisationLabelId != organisationLabelId)
                .Any(organisationLabel => organisationLabel.Validity.OverlapsWith(validity)))
                throw new LabelAlreadyCoupledToInThisPeriodException();

            ApplyChange(new OrganisationLabelAdded(
                Id,
                organisationLabelId,
                labelType.Id,
                labelType.Name,
                labelValue,
                validity.Start,
                validity.End));
        }

        public void AddKboFormalNameLabel(
            Guid organisationLabelId,
            LabelType labelType,
            string labelValue,
            Period validity)
        {
            ApplyChange(new KboFormalNameLabelAdded(
                Id,
                organisationLabelId,
                labelType.Id,
                labelType.Name,
                labelValue,
                validity.Start,
                validity.End));
        }

        public void UpdateKboFormalNameLabel(IMagdaName kboFormalName, LabelType formalNameLabelType)
        {
            if (_kboFormalNameLabel.Value == kboFormalName.Value)
                return;

            ApplyChange(
                new KboFormalNameLabelRemoved(
                    Id,
                    _kboFormalNameLabel.OrganisationLabelId,
                    _kboFormalNameLabel.LabelTypeId,
                    _kboFormalNameLabel.LabelTypeName,
                    _kboFormalNameLabel.Value,
                    _kboFormalNameLabel.Validity.Start,
                    kboFormalName.ValidFrom,
                    _kboFormalNameLabel.Validity.End));

            ApplyChange(
                new KboFormalNameLabelAdded(
                    Id,
                    Guid.NewGuid(),
                    formalNameLabelType.Id,
                    formalNameLabelType.Name,
                    kboFormalName.Value,
                    kboFormalName.ValidFrom,
                    new ValidTo()));
        }

        public void UpdateLabel(
            Guid organisationLabelId,
            LabelType labelType,
            string labelValue,
            Period validity)
        {
            if (_organisationLabels
                .Where(organisationLabel => organisationLabel.LabelTypeId == labelType.Id)
                .Where(organisationLabel => organisationLabel.OrganisationLabelId != organisationLabelId)
                .Any(organisationLabel => organisationLabel.Validity.OverlapsWith(validity)))
                throw new LabelAlreadyCoupledToInThisPeriodException();

            var previousLabel = _organisationLabels.Single(label => label.OrganisationLabelId == organisationLabelId);

            ApplyChange(new OrganisationLabelUpdated(
                Id,
                organisationLabelId,
                labelType.Id,
                labelType.Name,
                labelValue,
                validity.Start,
                validity.End,
                previousLabel.LabelTypeId,
                previousLabel.LabelTypeName,
                previousLabel.Value,
                previousLabel.Validity.Start,
                previousLabel.Validity.End));
        }

        public void AddBuilding(
            Guid organisationBuildingId,
            Building building,
            bool isMainBuilding,
            Period validity,
            IDateTimeProvider dateTimeProvider)
        {
            var organisationBuilding =
                new OrganisationBuilding(
                    organisationBuildingId,
                    Id,
                    building.Id,
                    building.Name,
                    isMainBuilding,
                    validity);

            if (_organisationBuildings.AlreadyHasTheSameOrganisationAndBuildingInTheSamePeriod(organisationBuilding))
                throw new BuildingAlreadyCoupledToInThisPeriodException();

            if (organisationBuilding.IsMainBuilding && _organisationBuildings.OrganisationAlreadyHasAMainBuildingInTheSamePeriod(organisationBuilding))
                throw new OrganisationAlreadyHasAMainBuildingInThisPeriodException();

            ApplyChange(new OrganisationBuildingAdded(
                Id,
                organisationBuildingId,
                building.Id,
                building.Name,
                isMainBuilding,
                validity.Start,
                validity.End));

            CheckIfMainBuildingChanged(organisationBuilding, dateTimeProvider.Today);
        }

        public void UpdateBuilding(
            Guid organisationBuildingId,
            Building building,
            bool isMainBuilding,
            Period validity,
            IDateTimeProvider dateTimeProvider)
        {
            var organisationBuilding =
                new OrganisationBuilding(
                    organisationBuildingId,
                    Id,
                    building.Id,
                    building.Name,
                    isMainBuilding,
                    validity);

            if (_organisationBuildings.AlreadyHasTheSameOrganisationAndBuildingInTheSamePeriod(organisationBuilding))
                throw new BuildingAlreadyCoupledToInThisPeriodException();

            if (organisationBuilding.IsMainBuilding && _organisationBuildings.OrganisationAlreadyHasAMainBuildingInTheSamePeriod(organisationBuilding))
                throw new OrganisationAlreadyHasAMainBuildingInThisPeriodException();

            var previousOrganisationBuilding =
                _organisationBuildings.Single(x => x.OrganisationBuildingId == organisationBuildingId);

            ApplyChange(new OrganisationBuildingUpdated(
                Id,
                organisationBuildingId,
                building.Id,
                building.Name,
                isMainBuilding,
                validity.Start,
                validity.End,
                previousOrganisationBuilding.BuildingId,
                previousOrganisationBuilding.BuildingName,
                previousOrganisationBuilding.IsMainBuilding,
                previousOrganisationBuilding.Validity.Start,
                previousOrganisationBuilding.Validity.End));

            CheckIfMainBuildingChanged(organisationBuilding, dateTimeProvider.Today);
        }

        public void AddLocation(
            Guid organisationLocationId,
            Location location,
            bool isMainLocation,
            LocationType locationType,
            Period validity,
            IDateTimeProvider dateTimeProvider)
        {
            var organisationLocation =
                new OrganisationLocation(
                    organisationLocationId,
                    Id,
                    location.Id,
                    location.FormattedAddress,
                    isMainLocation,
                    locationType?.Id,
                    locationType?.Name,
                    validity);

            if (_organisationLocations.AlreadyHasTheSameOrganisationAndLocationInTheSamePeriod(organisationLocation))
                throw new LocationAlreadyCoupledToInThisPeriodException();

            if (organisationLocation.IsMainLocation && _organisationLocations.OrganisationAlreadyHasAMainLocationInTheSamePeriod(organisationLocation))
                throw new OrganisationAlreadyHasAMainLocationInThisPeriodException();

            ApplyChange(new OrganisationLocationAdded(
                Id,
                organisationLocationId,
                location.Id,
                location.FormattedAddress,
                isMainLocation,
                locationType?.Id,
                locationType?.Name,
                validity.Start,
                validity.End));

            CheckIfMainLocationChanged(organisationLocation, dateTimeProvider.Today);
        }

        public void AddKboRegisteredOfficeLocation(
            Guid organisationLocationId,
            Location location,
            LocationType locationType,
            Period validity,
            IDateTimeProvider dateTimeProvider)
        {
            ApplyChange(new KboRegisteredOfficeOrganisationLocationAdded(
                Id,
                organisationLocationId,
                location.Id,
                location.FormattedAddress,
                false,
                locationType?.Id,
                locationType?.Name,
                validity.Start,
                validity.End));
        }

        public void UpdateKboRegisteredOfficeLocations(
            KboRegisteredOffice newKboRegisteredOffice,
            LocationType registeredOfficeLocationType)
        {
            if (newKboRegisteredOffice?.Location?.Id == _kboRegisteredOffice?.LocationId)
                return;

            if (_kboRegisteredOffice != null)
                ApplyChange(
                    new KboRegisteredOfficeOrganisationLocationRemoved(
                        Id,
                        _kboRegisteredOffice.OrganisationLocationId,
                        _kboRegisteredOffice.LocationId,
                        _kboRegisteredOffice.FormattedAddress,
                        _kboRegisteredOffice.IsMainLocation,
                        _kboRegisteredOffice.LocationTypeId,
                        _kboRegisteredOffice.LocationTypeName,
                        _kboRegisteredOffice.Validity.Start,
                        _kboRegisteredOffice.Validity.End,
                        _kboRegisteredOffice.Validity.End));

            if (newKboRegisteredOffice != null)
                ApplyChange(
                    new KboRegisteredOfficeOrganisationLocationAdded(
                        Id,
                        Guid.NewGuid(),
                        newKboRegisteredOffice.Location.Id,
                        newKboRegisteredOffice.Location.FormattedAddress,
                        false,
                        registeredOfficeLocationType.Id,
                        registeredOfficeLocationType.Name,
                        newKboRegisteredOffice.ValidFrom,
                        new ValidTo()));
        }

        public void UpdateLocation(
            Guid organisationLocationId,
            Location location,
            bool isMainLocation,
            LocationType locationType,
            Period validity,
            IDateTimeProvider dateTimeProvider)
        {
            var organisationLocation =
                new OrganisationLocation(
                    organisationLocationId,
                    Id,
                    location.Id,
                    location.FormattedAddress,
                    isMainLocation,
                    locationType?.Id,
                    locationType?.Name,
                    validity);

            if (organisationLocation == null)
                throw new ArgumentNullException(nameof(organisationLocation));

            if (_organisationLocations.AlreadyHasTheSameOrganisationAndLocationInTheSamePeriod(organisationLocation))
                throw new LocationAlreadyCoupledToInThisPeriodException();

            if (organisationLocation.IsMainLocation && _organisationLocations.OrganisationAlreadyHasAMainLocationInTheSamePeriod(organisationLocation))
                throw new OrganisationAlreadyHasAMainLocationInThisPeriodException();

            var previousLocation =
                _organisationLocations.Single(x => x.OrganisationLocationId == organisationLocationId);

            ApplyChange(new OrganisationLocationUpdated(
                Id,
                organisationLocationId,
                location.Id,
                location.FormattedAddress,
                isMainLocation,
                locationType?.Id,
                locationType?.Name,
                validity.Start,
                validity.End,
                previousLocation.LocationId,
                previousLocation.FormattedAddress,
                previousLocation.IsMainLocation,
                previousLocation.LocationTypeId,
                previousLocation.LocationTypeName,
                previousLocation.Validity.Start,
                previousLocation.Validity.End));

            CheckIfMainLocationChanged(organisationLocation, dateTimeProvider.Today);
        }

        public void AddOpeningHour(
            Guid organisationOpeningHourId,
            TimeSpan opens,
            TimeSpan closes,
            DayOfWeek? dayOfWeek,
            Period validity)
        {
            ApplyChange(new OrganisationOpeningHourAdded(
                Id,
                organisationOpeningHourId,
                opens,
                closes,
                dayOfWeek,
                validity.Start,
                validity.End));
        }

        public void UpdateOpeningHour(
            Guid organisationOpeningHourId,
            TimeSpan opens,
            TimeSpan closes,
            DayOfWeek? dayOfWeek,
            Period validity)
        {
            var previousOpeningHour = _organisationOpeningHours.Single(label => label.OrganisationOpeningHourId == organisationOpeningHourId);

            ApplyChange(new OrganisationOpeningHourUpdated(
                Id,
                organisationOpeningHourId,
                opens,
                previousOpeningHour.Opens,
                closes,
                previousOpeningHour.Closes,
                dayOfWeek,
                previousOpeningHour.DayOfWeek,
                validity.Start,
                previousOpeningHour.Validity?.Start,
                validity.End,
                previousOpeningHour.Validity?.End));
        }

        public void AddBankAccount(
            Guid organisationBankAccountId,
            BankAccountNumber bankAccountNumber,
            BankAccountBic bankAccountBic,
            Period validity)
        {
            ApplyChange(new OrganisationBankAccountAdded(
                Id,
                organisationBankAccountId,
                bankAccountNumber.Number,
                bankAccountNumber.IsValidIban,
                bankAccountBic.Bic,
                bankAccountBic.IsValidBic,
                validity.Start,
                validity.End));
        }

        public void UpdateBankAccount(
            Guid organisationBankAccountId,
            BankAccountNumber bankAccountNumber,
            BankAccountBic bankAccountBic,
            Period validity)
        {
            var previousBankAccount =
                _organisationBankAccounts.Single(bankAccount => bankAccount.OrganisationBankAccountId == organisationBankAccountId);

            ApplyChange(new OrganisationBankAccountUpdated(
                Id,
                organisationBankAccountId,
                bankAccountNumber.Number,
                bankAccountNumber.IsValidIban,
                bankAccountBic.Bic,
                bankAccountBic.IsValidBic,
                validity.Start,
                validity.End,
                previousBankAccount.BankAccountNumber,
                previousBankAccount.IsIban,
                previousBankAccount.Bic,
                previousBankAccount.IsBic,
                previousBankAccount.Validity.Start,
                previousBankAccount.Validity.End));
        }

        public void AddKboBankAccount(
            Guid organisationBankAccountId,
            BankAccountNumber bankAccountNumber,
            BankAccountBic bankAccountBic,
            Period validity)
        {
            ApplyChange(new KboOrganisationBankAccountAdded(
                Id,
                organisationBankAccountId,
                bankAccountNumber.Number,
                bankAccountNumber.IsValidIban,
                bankAccountBic.Bic,
                bankAccountBic.IsValidBic,
                validity.Start,
                validity.End));
        }

        public void UpdateKboBankAccount(List<IMagdaBankAccount> kboOrganisationBankAccounts)
        {
            var events = new List<IEvent>();
            foreach (var kboBankAccount in _kboBankAccounts)
            {
                if (!kboOrganisationBankAccounts.Any(x =>
                    kboBankAccount.BankAccountNumber == x.AccountNumber &&
                    kboBankAccount.Bic == x.Bic &&
                    kboBankAccount.Validity.Start == x.ValidFrom &&
                    kboBankAccount.Validity.End == x.ValidTo))
                {
                    events.Add(
                        new KboOrganisationBankAccountRemoved(
                        Id,
                        kboBankAccount.OrganisationBankAccountId,
                        kboBankAccount.BankAccountNumber,
                        kboBankAccount.IsIban,
                        kboBankAccount.Bic,
                        kboBankAccount.IsBic,
                        kboBankAccount.Validity.Start,
                        kboBankAccount.Validity.End));
                }
            }

            foreach (var magdaBankAccount in kboOrganisationBankAccounts)
            {
                if (!_kboBankAccounts.Any(x =>
                    magdaBankAccount.AccountNumber == x.BankAccountNumber &&
                    magdaBankAccount.Bic == x.Bic &&
                    magdaBankAccount.ValidFrom == x.Validity.Start &&
                    magdaBankAccount.ValidTo == x.Validity.End))
                {
                    var bankAccountNr = BankAccountNumber.CreateWithUnknownValidity(magdaBankAccount.AccountNumber);
                    var bankAccountBic = BankAccountBic.CreateWithUnknownValidity(magdaBankAccount.Bic);

                    events.Add(new KboOrganisationBankAccountAdded(
                        Id,
                        Guid.NewGuid(),
                        bankAccountNr.Number,
                        bankAccountNr.IsValidIban,
                        bankAccountBic.Bic,
                        bankAccountBic.IsValidBic,
                        magdaBankAccount.ValidFrom,
                        magdaBankAccount.ValidTo));
                }
            }
            events.ForEach(ApplyChange);
        }

        public void UpdateMainBuilding(DateTime today)
        {
            var events = new List<IEvent>();

            if (_mainOrganisationBuilding != null && !_mainOrganisationBuilding.Validity.OverlapsWith(today))
                events.Add(new MainBuildingClearedFromOrganisation(Id, _mainOrganisationBuilding.BuildingId));

            var newMainOrganisationBuilding = _organisationBuildings.TryFindMainOrganisationBuildingValidFor(today);

            if (newMainOrganisationBuilding != null && !Equals(newMainOrganisationBuilding, _mainOrganisationBuilding))
                events.Add(new MainBuildingAssignedToOrganisation(Id, newMainOrganisationBuilding.BuildingId, newMainOrganisationBuilding.OrganisationBuildingId));

            // WHY: applying the MainBuildingClearedFromOrganisation would cause the mainBuilding to be cleared,
            // which makes it harder to compare with the newMainOrganisationBuilding.
            events.ForEach(ApplyChange);
        }

        public void UpdateMainLocation(DateTime today)
        {
            var events = new List<IEvent>();

            if (_mainOrganisationLocation != null && !_mainOrganisationLocation.Validity.OverlapsWith(today))
                events.Add(new MainLocationClearedFromOrganisation(Id, _mainOrganisationLocation.LocationId));

            var newMainOrganisationLocation = _organisationLocations.TryFindMainOrganisationLocationValidFor(today);

            if (newMainOrganisationLocation != null && !Equals(newMainOrganisationLocation, _mainOrganisationLocation))
                events.Add(new MainLocationAssignedToOrganisation(Id, newMainOrganisationLocation.LocationId, newMainOrganisationLocation.OrganisationLocationId));

            // WHY: applying the MainLocationClearedFromOrganisation would cause the mainLocation to be cleared,
            // which makes it harder to compare with the newMainOrganisationLocation.
            events.ForEach(ApplyChange);
        }

        public void UpdateCurrentOrganisationParent(DateTime today)
        {
            var events = new List<IEvent>();

            if (_currentOrganisationParent != null && !_currentOrganisationParent.Validity.OverlapsWith(today))
                events.Add(new ParentClearedFromOrganisation(Id, _currentOrganisationParent.ParentOrganisationId));

            var newOrganisationParent = _organisationParents.SingleOrDefault(parent => parent.Validity.OverlapsWith(today));

            if (newOrganisationParent != null && !Equals(newOrganisationParent, _currentOrganisationParent))
                events.Add(new ParentAssignedToOrganisation(Id, newOrganisationParent.ParentOrganisationId, newOrganisationParent.OrganisationOrganisationParentId));

            // WHY: applying the ParentClearedFromOrganisation would cause the organisationParent to be cleared,
            // which makes it harder to compare with the newOrganisation.
            events.ForEach(ApplyChange);
        }

        public void UpdateOrganisationFormalFrameworkParent(
            DateTime today,
            Guid formalFrameworkId)
        {
            var parentForFormalFramework =
                _organisationFormalFrameworkParentsPerFormalFramework.ContainsKey(formalFrameworkId)
                    ? _organisationFormalFrameworkParentsPerFormalFramework[formalFrameworkId]
                    : null;

            if (parentForFormalFramework != null &&
                !parentForFormalFramework.Validity.OverlapsWith(today))
            {
                ApplyChange(new FormalFrameworkClearedFromOrganisation(
                    parentForFormalFramework.OrganisationFormalFrameworkId,
                    Id,
                    parentForFormalFramework.FormalFrameworkId,
                    parentForFormalFramework.ParentOrganisationId));
            }

            if (!_organisationFormalFrameworkParentsPerFormalFramework.ContainsKey(formalFrameworkId))
            {
                var organisationFormalFramework =
                    _organisationFormalFrameworks.SingleOrDefault(framework =>
                        framework.FormalFrameworkId == formalFrameworkId &&
                        framework.Validity.OverlapsWith(today));

                if (organisationFormalFramework != null)
                    ApplyChange(new FormalFrameworkAssignedToOrganisation(
                        Id,
                        organisationFormalFramework.FormalFrameworkId,
                        organisationFormalFramework.ParentOrganisationId,
                        organisationFormalFramework.OrganisationFormalFrameworkId));
            }
        }

        public void UpdateRelationshipValidities(DateTime date)
        {
            foreach (var organisationCapacity in _organisationCapacities)
            {
                if (organisationCapacity.ShouldBecomeActive(date))
                    ApplyChange(new OrganisationCapacityBecameActive(
                        Id,
                        organisationCapacity.OrganisationCapacityId,
                        organisationCapacity.CapacityId,
                        organisationCapacity.PersonId,
                        organisationCapacity.FunctionTypeId,
                        organisationCapacity.Validity.Start));

                else if (organisationCapacity.ShouldBecomeInactive(date))
                    ApplyChange(new OrganisationCapacityBecameInactive(
                        Id,
                        organisationCapacity.OrganisationCapacityId,
                        organisationCapacity.CapacityId,
                        organisationCapacity.PersonId,
                        organisationCapacity.FunctionTypeId,
                        organisationCapacity.Validity.End));
            }
        }

        public void CoupleToKbo(
            KboNumber kboNumber,
            IDateTimeProvider dateTimeProvider)
        {
            if (HasKboNumber)
                throw new OrganisationAlreadyCoupledWithKbo();

            ApplyChange(new OrganisationCoupledWithKbo(
                Id,
                kboNumber.ToDigitsOnly(),
                Name,
                OvoNumber,
                dateTimeProvider.Today));
        }

        public void CancelCouplingWithKbo()
        {
            if (!HasKboNumber)
                throw new OrganisationNotCoupledWithKbo();

            if (CoupledToKboFromCreation)
                throw new CannotCancelCouplingWithOrganisationCreatedFromKbo();

            ApplyChange(new OrganisationCouplingWithKboCancelled(
                Id,
                KboNumber!.ToDigitsOnly(),
                _nameBeforeKboCoupling,
                _shortNameBeforeKboCoupling,
                Name,
                _shortName,
                OvoNumber,
                _kboLegalFormOrganisationClassification?.OrganisationOrganisationClassificationId,
                _kboFormalNameLabel?.OrganisationLabelId,
                _kboRegisteredOffice?.OrganisationLocationId,
                _kboBankAccounts.Select(account => account.OrganisationBankAccountId).ToList()));
        }

        public void TerminateKboCoupling()
        {
            if (!HasKboNumber)
                throw new OrganisationNotCoupledWithKbo();

            if (!TerminationInKbo.HasValue)
                throw new KboOrganisationNotTerminatedException();

            ApplyChange(
                new OrganisationTerminationSyncedWithKbo(
                    Id,
                    KboNumber!.ToDigitsOnly(),
                    Name,
                    OvoNumber,
                    TerminationInKbo.Value.Date,
                    _kboLegalFormOrganisationClassification?.OrganisationOrganisationClassificationId,
                    _kboFormalNameLabel?.OrganisationLabelId,
                    _kboRegisteredOffice?.Validity.End.IsInfinite ?? false ? _kboRegisteredOffice.OrganisationLocationId : (Guid?) null,
                    _kboBankAccounts
                        .Where(account => account.Validity.End.IsInfinite)
                        .Select(account => account.OrganisationBankAccountId).ToList()));
        }

        private void CheckIfCurrentParentChanged(
            OrganisationParent organisationParent,
            DateTime today)
        {
            if (_currentOrganisationParent == null && organisationParent.Validity.OverlapsWith(today))
            {
                ApplyChange(new ParentAssignedToOrganisation(
                    Id,
                    organisationParent.ParentOrganisationId,
                    organisationParent.OrganisationOrganisationParentId));
            }
            else if (!Equals(_currentOrganisationParent, organisationParent) && organisationParent.Validity.OverlapsWith(today))
            {
                if (_currentOrganisationParent != null)
                    ApplyChange(new ParentClearedFromOrganisation(
                        Id,
                        _currentOrganisationParent.ParentOrganisationId));

                ApplyChange(new ParentAssignedToOrganisation(
                    Id,
                    organisationParent.ParentOrganisationId,
                    organisationParent.OrganisationOrganisationParentId));
            }
            else if (Equals(_currentOrganisationParent, organisationParent) && !organisationParent.Validity.OverlapsWith(today))
            {
                ApplyChange(new ParentClearedFromOrganisation(
                    Id,
                    organisationParent.ParentOrganisationId));
            }
        }

        private void CheckIfCurrentFormalFrameworkParentChanged(
            OrganisationFormalFramework organisationFormalFramework,
            DateTime today)
        {
            var currentFormalFrameworkParent =
                _organisationFormalFrameworkParentsPerFormalFramework.ContainsKey(organisationFormalFramework.FormalFrameworkId) ?
                    _organisationFormalFrameworkParentsPerFormalFramework[organisationFormalFramework.FormalFrameworkId] :
                    null;

            if (currentFormalFrameworkParent == null && organisationFormalFramework.Validity.OverlapsWith(today))
            {
                ApplyChange(new FormalFrameworkAssignedToOrganisation(
                    Id,
                    organisationFormalFramework.FormalFrameworkId,
                    organisationFormalFramework.ParentOrganisationId,
                    organisationFormalFramework.OrganisationFormalFrameworkId));
            }
            else if (!Equals(currentFormalFrameworkParent, organisationFormalFramework) && organisationFormalFramework.Validity.OverlapsWith(today))
            {
                if (currentFormalFrameworkParent != null)
                {
                    ApplyChange(new FormalFrameworkClearedFromOrganisation(
                        currentFormalFrameworkParent.OrganisationFormalFrameworkId,
                        Id,
                        currentFormalFrameworkParent.FormalFrameworkId,
                        currentFormalFrameworkParent.ParentOrganisationId));
                }

                ApplyChange(new FormalFrameworkAssignedToOrganisation(
                    Id,
                    organisationFormalFramework.FormalFrameworkId,
                    organisationFormalFramework.ParentOrganisationId,
                    organisationFormalFramework.OrganisationFormalFrameworkId));
            }
            else if (Equals(currentFormalFrameworkParent, organisationFormalFramework) && !organisationFormalFramework.Validity.OverlapsWith(today))
            {
                ApplyChange(new FormalFrameworkClearedFromOrganisation(
                    organisationFormalFramework.OrganisationFormalFrameworkId,
                    Id,
                    currentFormalFrameworkParent.FormalFrameworkId,
                    currentFormalFrameworkParent.ParentOrganisationId));
            }
        }

        private void CheckIfMainBuildingChanged(
            OrganisationBuilding organisationBuilding,
            DateTime today)
        {
            if (Equals(_mainOrganisationBuilding, organisationBuilding) &&
                (!organisationBuilding.IsMainBuilding || !organisationBuilding.Validity.OverlapsWith(today)))
            {
                ApplyChange(new MainBuildingClearedFromOrganisation(
                    Id,
                    organisationBuilding.BuildingId));
            }
            else if (Equals(_mainOrganisationBuilding, organisationBuilding) &&
                     !organisationBuilding.IsMainBuilding &&
                     organisationBuilding.Validity.OverlapsWith(today))
            {
                ApplyChange(new MainBuildingClearedFromOrganisation(
                    Id,
                    organisationBuilding.BuildingId));
            }
            else if (!Equals(_mainOrganisationBuilding, organisationBuilding) &&
                     organisationBuilding.IsMainBuilding &&
                     organisationBuilding.Validity.OverlapsWith(today))
            {
                if (_mainOrganisationBuilding != null)
                    ApplyChange(new MainBuildingClearedFromOrganisation(
                        Id,
                        organisationBuilding.BuildingId));

                ApplyChange(new MainBuildingAssignedToOrganisation(
                    Id,
                    organisationBuilding.BuildingId,
                    organisationBuilding.OrganisationBuildingId));
            }
        }

        private void CheckIfMainLocationChanged(
            OrganisationLocation organisationLocation,
            DateTime today)
        {
            if (Equals(_mainOrganisationLocation, organisationLocation) &&
                (!organisationLocation.IsMainLocation || !organisationLocation.Validity.OverlapsWith(today)))
            {
                ApplyChange(new MainLocationClearedFromOrganisation(
                    Id,
                    organisationLocation.LocationId));
            }
            else if (!Equals(_mainOrganisationLocation, organisationLocation) &&
                     organisationLocation.IsMainLocation &&
                     organisationLocation.Validity.OverlapsWith(today))
            {
                if (_mainOrganisationLocation != null)
                    ApplyChange(new MainLocationClearedFromOrganisation(
                        Id,
                        organisationLocation.LocationId));

                ApplyChange(new MainLocationAssignedToOrganisation(
                    Id,
                    organisationLocation.LocationId,
                    organisationLocation.OrganisationLocationId));
            }
        }

        private void Apply(OrganisationCreated @event)
        {
            Id = @event.OrganisationId;
            Name = @event.Name;
            _shortName = @event.ShortName;
            OvoNumber = @event.OvoNumber;
            _description = @event.Description;
            _purposes = @event.Purposes;
            _showOnVlaamseOverheidSites = @event.ShowOnVlaamseOverheidSites;
            _validity = new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo));
        }

        private void Apply(OrganisationCreatedFromKbo @event)
        {
            Id = @event.OrganisationId;
            Name = @event.Name;
            _shortName = @event.ShortName;
            OvoNumber = @event.OvoNumber;
            _description = @event.Description;
            _purposes = @event.Purposes;
            _showOnVlaamseOverheidSites = @event.ShowOnVlaamseOverheidSites;
            _validity = new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo));
            KboNumber = new KboNumber(@event.KboNumber);
            CoupledToKboFromCreation = true;
        }

        private void Apply(OrganisationInfoUpdatedFromKbo @event)
        {
            Name = @event.Name;
            _shortName = @event.ShortName;
        }

        private void Apply(OrganisationBecameActive @event)
        {
            _isActive = true;
        }

        private void Apply(OrganisationBecameInactive @event)
        {
            _isActive = false;
        }

        private void Apply(OrganisationInfoUpdated @event)
        {
            Name = @event.Name;
            _description = @event.Description;
            _shortName = @event.ShortName;
            _purposes = @event.Purposes;
            _showOnVlaamseOverheidSites = @event.ShowOnVlaamseOverheidSites;
            _validity = new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo));
        }

        private void Apply(OrganisationKeyAdded @event)
        {
            _organisationKeys.Add(new OrganisationKey(
                @event.OrganisationKeyId,
                @event.OrganisationId,
                @event.KeyTypeId,
                @event.KeyTypeName,
                @event.Value,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));
        }

        private void Apply(OrganisationParentAdded @event)
        {
            _organisationParents.Add(new OrganisationParent(
                @event.OrganisationOrganisationParentId,
                @event.ParentOrganisationId,
                @event.ParentOrganisationName,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));
        }

        private void Apply(OrganisationFormalFrameworkAdded @event)
        {
            _organisationFormalFrameworks.Add(new OrganisationFormalFramework(
                @event.OrganisationFormalFrameworkId,
                @event.FormalFrameworkId,
                @event.FormalFrameworkName,
                @event.ParentOrganisationId,
                @event.ParentOrganisationName,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));
        }

        private void Apply(OrganisationContactAdded @event)
        {
            _organisationContacts.Add(new OrganisationContact(
                @event.OrganisationContactId,
                @event.OrganisationId,
                @event.ContactTypeId,
                @event.ContactTypeName,
                @event.Value,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));
        }

        private void Apply(OrganisationFunctionAdded @event)
        {
            _organisationFunctionTypes.Add(new OrganisationFunction(
                @event.OrganisationFunctionId,
                @event.OrganisationId,
                @event.FunctionId,
                @event.FunctionName,
                @event.PersonId,
                @event.PersonFullName,
                @event.Contacts,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));
        }

        private void Apply(OrganisationRelationAdded @event)
        {
            _organisationRelations.Add(new OrganisationRelation(
                @event.OrganisationRelationId,
                @event.OrganisationId,
                @event.OrganisationName,
                @event.RelationId,
                @event.RelationName,
                @event.RelationInverseName,
                @event.RelatedOrganisationId,
                @event.RelatedOrganisationName,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));
        }

        private void Apply(OrganisationCapacityAdded @event)
        {
            _organisationCapacities.Add(new OrganisationCapacity(
                @event.OrganisationCapacityId,
                @event.OrganisationId,
                @event.CapacityId,
                @event.CapacityName,
                @event.PersonId,
                @event.PersonFullName,
                @event.FunctionId,
                @event.FunctionName,
                @event.LocationId,
                @event.LocationName,
                @event.Contacts,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo)),
                false));
        }

        private void Apply(OrganisationOrganisationClassificationAdded @event)
        {
            _organisationOrganisationClassifications.Add(new OrganisationOrganisationClassification(
                @event.OrganisationOrganisationClassificationId,
                @event.OrganisationId,
                @event.OrganisationClassificationTypeId,
                @event.OrganisationClassificationTypeName,
                @event.OrganisationClassificationId,
                @event.OrganisationClassificationName,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));
        }

        private void Apply(KboLegalFormOrganisationOrganisationClassificationAdded @event)
        {
            _kboLegalFormOrganisationClassification = new OrganisationOrganisationClassification(
                @event.OrganisationOrganisationClassificationId,
                @event.OrganisationId,
                @event.OrganisationClassificationTypeId,
                @event.OrganisationClassificationTypeName,
                @event.OrganisationClassificationId,
                @event.OrganisationClassificationName,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo)));
        }

        private void Apply(KboLegalFormOrganisationOrganisationClassificationRemoved @event)
        {
            _kboLegalFormOrganisationClassification = null;
        }

        private void Apply(OrganisationLabelAdded @event)
        {
            _organisationLabels.Add(new OrganisationLabel(
                @event.OrganisationLabelId,
                @event.OrganisationId,
                @event.LabelTypeId,
                @event.LabelTypeName,
                @event.Value,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));
        }

        private void Apply(KboFormalNameLabelAdded @event)
        {
            _kboFormalNameLabel = new OrganisationLabel(
                @event.OrganisationLabelId,
                @event.OrganisationId,
                @event.LabelTypeId,
                @event.LabelTypeName,
                @event.Value,
                new Period(
                new ValidFrom(@event.ValidFrom),
                new ValidTo(@event.ValidTo)));
        }

        private void Apply(KboFormalNameLabelRemoved @event)
        {
            _kboFormalNameLabel = null;
        }

        private void Apply(OrganisationBuildingAdded @event)
        {
            var organisationBuilding = new OrganisationBuilding(
                @event.OrganisationBuildingId,
                @event.OrganisationId,
                @event.BuildingId,
                @event.BuildingName,
                @event.IsMainBuilding,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo)));

            _organisationBuildings.Add(organisationBuilding);
        }

        private void Apply(OrganisationOpeningHourAdded @event)
        {
            _organisationOpeningHours.Add(new OrganisationOpeningHour(
                @event.OrganisationOpeningHourId,
                @event.OrganisationId,
                @event.Opens,
                @event.Closes,
                @event.DayOfWeek,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));
        }

        private void Apply(OrganisationBuildingUpdated @event)
        {
            var organisationBuilding = new OrganisationBuilding(
                @event.OrganisationBuildingId,
                @event.OrganisationId,
                @event.BuildingId,
                @event.BuildingName,
                @event.IsMainBuilding,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo)));

            var oldOrganisationBuilding = _organisationBuildings.Single(building => building.OrganisationBuildingId == @event.OrganisationBuildingId);
            _organisationBuildings.Remove(oldOrganisationBuilding);
            _organisationBuildings.Add(organisationBuilding);
        }

        private void Apply(OrganisationLocationAdded @event)
        {
            _organisationLocations.Add(
                new OrganisationLocation(
                    @event.OrganisationLocationId,
                    @event.OrganisationId,
                    @event.LocationId,
                    @event.LocationFormattedAddress,
                    @event.IsMainLocation,
                    @event.LocationTypeId,
                    @event.LocationTypeName,
                    new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));
        }

        private void Apply(KboRegisteredOfficeOrganisationLocationAdded @event)
        {
            _kboRegisteredOffice = new OrganisationLocation(
                @event.OrganisationLocationId,
                @event.OrganisationId,
                @event.LocationId,
                @event.LocationFormattedAddress,
                @event.IsMainLocation,
                @event.LocationTypeId,
                @event.LocationTypeName,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo)));
        }

        private void Apply(KboRegisteredOfficeOrganisationLocationRemoved @event)
        {
            _kboRegisteredOffice = null;
        }

        private void Apply(OrganisationLocationUpdated @event)
        {
            var organisationLocation = new OrganisationLocation(
                @event.OrganisationLocationId,
                @event.OrganisationId,
                @event.LocationId,
                @event.LocationFormattedAddress,
                @event.IsMainLocation,
                @event.LocationTypeId,
                @event.LocationTypeName,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo)));

            var oldOrganisationLocation = _organisationLocations.Single(location => location.OrganisationLocationId == @event.OrganisationLocationId);
            _organisationLocations.Remove(oldOrganisationLocation);
            _organisationLocations.Add(organisationLocation);
        }

        private void Apply(OrganisationKeyUpdated @event)
        {
            _organisationKeys.Remove(_organisationKeys.Single(ob => ob.OrganisationKeyId == @event.OrganisationKeyId));
            _organisationKeys.Add(
                new OrganisationKey(
                    @event.OrganisationKeyId,
                    @event.OrganisationId,
                    @event.KeyTypeId,
                    @event.KeyTypeName,
                    @event.Value,
                    new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));
        }

        private void Apply(OrganisationParentUpdated @event)
        {
            var newOrganisationParent = new OrganisationParent(
                @event.OrganisationOrganisationParentId,
                @event.ParentOrganisationId,
                @event.ParentOrganisationName,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo)));

            _organisationParents.Remove(newOrganisationParent);
            _organisationParents.Add(newOrganisationParent);

            if (Equals(_currentOrganisationParent, newOrganisationParent))
                _currentOrganisationParent = newOrganisationParent; // update the values
        }

        private void Apply(OrganisationFormalFrameworkUpdated @event)
        {
            var newOrganisationFormalFramework = new OrganisationFormalFramework(
                @event.OrganisationFormalFrameworkId,
                @event.FormalFrameworkId,
                @event.FormalFrameworkName,
                @event.ParentOrganisationId,
                @event.ParentOrganisationName,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo)));

            _organisationFormalFrameworks.Remove(newOrganisationFormalFramework);
            _organisationFormalFrameworks.Add(newOrganisationFormalFramework);

            if (_organisationFormalFrameworkParentsPerFormalFramework.ContainsKey(newOrganisationFormalFramework.FormalFrameworkId) &&
                Equals(_organisationFormalFrameworkParentsPerFormalFramework[newOrganisationFormalFramework.FormalFrameworkId], newOrganisationFormalFramework))
            {
                _organisationFormalFrameworkParentsPerFormalFramework[newOrganisationFormalFramework.FormalFrameworkId] = newOrganisationFormalFramework; // update the values
            }
        }

        private void Apply(OrganisationContactUpdated @event)
        {
            _organisationContacts.Remove(_organisationContacts.Single(ob => ob.OrganisationContactId == @event.OrganisationContactId));
            _organisationContacts.Add(
                new OrganisationContact(
                    @event.OrganisationContactId,
                    @event.OrganisationId,
                    @event.ContactTypeId,
                    @event.ContactTypeName,
                    @event.Value,
                    new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));
        }

        private void Apply(OrganisationFunctionUpdated @event)
        {
            _organisationFunctionTypes.Remove(_organisationFunctionTypes.Single(ob => ob.OrganisationFunctionId == @event.OrganisationFunctionId));
            _organisationFunctionTypes.Add(
                new OrganisationFunction(
                    @event.OrganisationFunctionId,
                    @event.OrganisationId,
                    @event.FunctionId,
                    @event.FunctionName,
                    @event.PersonId,
                    @event.PersonFullName,
                    @event.Contacts,
                    new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));
        }

        private void Apply(OrganisationRelationUpdated @event)
        {
            _organisationRelations.Remove(_organisationRelations.Single(ob => ob.OrganisationRelationId == @event.OrganisationRelationId));
            _organisationRelations.Add(
                new OrganisationRelation(
                    @event.OrganisationRelationId,
                    @event.OrganisationId,
                    @event.OrganisationName,
                    @event.RelationId,
                    @event.RelationName,
                    @event.RelationInverseName,
                    @event.RelatedOrganisationId,
                    @event.RelatedOrganisationName,
                    new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));
        }

        private void Apply(OrganisationCapacityUpdated @event)
        {
            _organisationCapacities.Remove(_organisationCapacities.Single(ob => ob.OrganisationCapacityId == @event.OrganisationCapacityId));
            _organisationCapacities.Add(
                new OrganisationCapacity(
                    @event.OrganisationCapacityId,
                    @event.OrganisationId,
                    @event.CapacityId,
                    @event.CapacityName,
                    @event.PersonId,
                    @event.PersonFullName,
                    @event.FunctionId,
                    @event.FunctionName,
                    @event.LocationId,
                    @event.LocationName,
                    @event.Contacts,
                    new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo)),
                    @event.PreviouslyActive));
        }

        private void Apply(OrganisationOrganisationClassificationUpdated @event)
        {
            _organisationOrganisationClassifications.Remove(_organisationOrganisationClassifications.Single(ob => ob.OrganisationOrganisationClassificationId == @event.OrganisationOrganisationClassificationId));
            _organisationOrganisationClassifications.Add(
                new OrganisationOrganisationClassification(
                    @event.OrganisationOrganisationClassificationId,
                    @event.OrganisationId,
                    @event.OrganisationClassificationTypeId,
                    @event.OrganisationClassificationTypeName,
                    @event.OrganisationClassificationId,
                    @event.OrganisationClassificationName,
                    new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));
        }

        private void Apply(OrganisationLabelUpdated @event)
        {
            _organisationLabels.Remove(_organisationLabels.Single(ob => ob.OrganisationLabelId == @event.OrganisationLabelId));
            _organisationLabels.Add(
                new OrganisationLabel(
                    @event.OrganisationLabelId,
                    @event.OrganisationId,
                    @event.LabelTypeId,
                    @event.LabelTypeName,
                    @event.Value,
                    new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));
        }

        private void Apply(OrganisationBankAccountAdded @event)
        {
            _organisationBankAccounts.Add(
                new OrganisationBankAccount(
                    @event.OrganisationBankAccountId,
                    @event.OrganisationId,
                    @event.BankAccountNumber,
                    @event.IsIban,
                    @event.Bic,
                    @event.IsBic,
                    new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));
        }

        private void Apply(KboOrganisationBankAccountAdded @event)
        {
            _kboBankAccounts.Add(
                new OrganisationBankAccount(
                    @event.OrganisationBankAccountId,
                    @event.OrganisationId,
                    @event.BankAccountNumber,
                    @event.IsIban,
                    @event.Bic,
                    @event.IsBic,
                    new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));
        }

        private void Apply(KboOrganisationBankAccountRemoved @event)
        {
            _kboBankAccounts.Remove(_kboBankAccounts.Single(account =>
                account.OrganisationBankAccountId == @event.OrganisationBankAccountId));
        }

        private void Apply(OrganisationBankAccountUpdated @event)
        {
            var organisationBankAccount = new OrganisationBankAccount(
                @event.OrganisationBankAccountId,
                @event.OrganisationId,
                @event.BankAccountNumber,
                @event.IsIban,
                @event.Bic,
                @event.IsBic,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo)));

            var oldOrganisationBankAccount =
                _organisationBankAccounts
                    .Single(bankAccount => bankAccount.OrganisationBankAccountId == @event.OrganisationBankAccountId);

            _organisationBankAccounts.Remove(oldOrganisationBankAccount);
            _organisationBankAccounts.Add(organisationBankAccount);
        }

        private void Apply(OrganisationOpeningHourUpdated @event)
        {
            _organisationOpeningHours.Remove(_organisationOpeningHours.Single(ob => ob.OrganisationOpeningHourId == @event.OrganisationOpeningHourId));
            _organisationOpeningHours.Add(
                new OrganisationOpeningHour(
                    @event.OrganisationOpeningHourId,
                    @event.OrganisationId,
                    @event.Opens,
                    @event.Closes,
                    @event.DayOfWeek,
                    new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));
        }

        private void Apply(ParentAssignedToOrganisation @event)
        {
            _currentOrganisationParent = _organisationParents.Single(ob => ob.OrganisationOrganisationParentId == @event.OrganisationOrganisationParentId);
        }

        private void Apply(ParentClearedFromOrganisation @event)
        {
            _currentOrganisationParent = null;
        }

        private void Apply(FormalFrameworkAssignedToOrganisation @event)
        {
            _organisationFormalFrameworkParentsPerFormalFramework[@event.FormalFrameworkId] =
                _organisationFormalFrameworks.Single(off => off.OrganisationFormalFrameworkId == @event.OrganisationFormalFrameworkId);
        }

        private void Apply(FormalFrameworkClearedFromOrganisation @event)
        {
            _organisationFormalFrameworkParentsPerFormalFramework.Remove(@event.FormalFrameworkId);
        }

        private void Apply(MainBuildingAssignedToOrganisation @event)
        {
            _mainOrganisationBuilding = _organisationBuildings.Single(ob => ob.OrganisationBuildingId == @event.OrganisationBuildingId);
        }

        private void Apply(MainBuildingClearedFromOrganisation @event)
        {
            _mainOrganisationBuilding = null;
        }

        private void Apply(MainLocationAssignedToOrganisation @event)
        {
            _mainOrganisationLocation = _organisationLocations.Single(ob => ob.OrganisationLocationId == @event.OrganisationLocationId);
        }

        private void Apply(MainLocationClearedFromOrganisation @event)
        {
            _mainOrganisationLocation = null;
        }

        private void Apply(OrganisationCapacityBecameActive @event)
        {
            _organisationCapacities
                .Single(ob => ob.OrganisationCapacityId == @event.OrganisationCapacityId)
                .IsActive = true;
        }

        private void Apply(OrganisationCapacityBecameInactive @event)
        {
            _organisationCapacities
                .Single(ob => ob.OrganisationCapacityId == @event.OrganisationCapacityId)
                .IsActive = false;
        }

        private void Apply(OrganisationCoupledWithKbo @event)
        {
            KboNumber = new KboNumber(@event.KboNumber);
            _nameBeforeKboCoupling = Name;
            _shortNameBeforeKboCoupling = _shortName;
            CoupledToKboFromCreation = false;
        }

        private void Apply(OrganisationCouplingWithKboCancelled @event)
        {
            KboNumber = null;
            _shortName = _shortNameBeforeKboCoupling;
            Name = _nameBeforeKboCoupling;

            _kboBankAccounts.Clear();
            _kboRegisteredOffice = null;
            _kboFormalNameLabel = null;
            _kboLegalFormOrganisationClassification = null;
        }

        private void Apply(OrganisationTerminationFoundInKbo @event)
        {
            TerminationInKbo = new KboTermination(@event.TerminationDate, @event.TerminationCode, @event.TerminationReason);
        }

        private void Apply(OrganisationTerminationSyncedWithKbo @event)
        {
            KboNumber = null;
            TerminationInKbo = null;

            _kboBankAccounts.Clear();
            _kboRegisteredOffice = null;
            _kboFormalNameLabel = null;
            _kboLegalFormOrganisationClassification = null;
        }

        public IEnumerable<OrganisationParent> ParentsInPeriod(Period validity)
        {
            return _organisationParents.Where(parent => parent.Validity.OverlapsWith(validity));
        }

        public IEnumerable<OrganisationFormalFramework> ParentsInPeriod(FormalFramework formalFramework, Period validity)
        {
            return _organisationFormalFrameworks
                .Where(parent => parent.Validity.OverlapsWith(validity))
                .Where(parent => parent.FormalFrameworkId == formalFramework.Id);
        }
    }
}
