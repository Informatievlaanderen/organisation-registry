namespace OrganisationRegistry.Organisation
{
    using Building;
    using Capacity;
    using ContactType;
    using RegulationTheme;
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
    using Exceptions;
    using Infrastructure.Authorization;
    using OrganisationTermination;
    using RegulationSubTheme;
    using State;
    using IOrganisationRegistryConfiguration = Configuration.IOrganisationRegistryConfiguration;
    using Purpose = Purpose.Purpose;

    public partial class Organisation : AggregateRoot
    {
        private List<Events.Purpose> _purposes = null!;

        public readonly OrganisationState State;
        private OrganisationBuilding? _mainOrganisationBuilding;
        private OrganisationLocation? _mainOrganisationLocation;
        private OrganisationParent? _currentOrganisationParent;

        public KboState KboState { get; }

        private DateTime? _dateOfTermination;
        public bool IsTerminated => _dateOfTermination != null;

        public bool CoupledToKboFromCreation { get; set; }

        private bool HasKboNumber => KboState.KboNumber != null;

        private Organisation()
        {
            State = new OrganisationState();
            KboState = new KboState();
        }

        private Organisation(
            OrganisationId id,
            string name,
            string ovoNumber,
            string shortName,
            Article article,
            Organisation? parentOrganisation,
            string description,
            IEnumerable<Purpose> purposes,
            bool showOnVlaamseOverheidSites,
            Period validity,
            Period operationalValidity,
            IDateTimeProvider dateTimeProvider) : this()
        {
            ApplyChange(new OrganisationCreated(
                id,
                name,
                ovoNumber,
                shortName,
                article,
                description,
                purposes.Select(x => new Events.Purpose(x.Id,
                        x.Name))
                    .ToList(),
                showOnVlaamseOverheidSites,
                validity.Start,
                validity.End,
                operationalValidity.Start,
                operationalValidity.End));

            if (validity.OverlapsWith(dateTimeProvider.Today))
                ApplyChange(new OrganisationBecameActive(Id));

            if (parentOrganisation == null)
                return;

            ApplyChange(new OrganisationParentAdded(
                Id,
                Id,
                parentOrganisation.Id,
                parentOrganisation.State.Name,
                null,
                null));

            ApplyChange(new ParentAssignedToOrganisation(
                Id,
                parentOrganisation.Id,
                Id));

            if(parentOrganisation.State.UnderVlimpersManagement)
                ApplyChange(new OrganisationPlacedUnderVlimpersManagement(Id));
        }

        public static Organisation Create(OrganisationId id,
            string name,
            string ovoNumber,
            string shortName,
            Article article,
            Organisation? parentOrganisation,
            string description,
            IEnumerable<Purpose> purposes,
            bool showOnVlaamseOverheidSites,
            Period validity,
            Period operationalValidity,
            IDateTimeProvider dateTimeProvider)
        {
            return new Organisation(id,
                name,
                ovoNumber,
                shortName,
                article,
                parentOrganisation,
                description,
                purposes,
                showOnVlaamseOverheidSites,
                validity,
                operationalValidity,
                dateTimeProvider);
        }

        public static Organisation CreateFromKbo(
            CreateOrganisationFromKbo message,
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
                message.Article,
                parentOrganisation,
                message.Description,
                purposes,
                message.ShowOnVlaamseOverheidSites,
                new Period(new ValidFrom(kboOrganisation.ValidFrom), new ValidTo(message.ValidTo)),
                new Period(new ValidFrom(message.OperationalValidFrom), new ValidTo(message.OperationalValidTo)),
                dateTimeProvider);
        }

        private Organisation(
            OrganisationId id,
            KboNumber kboNumber,
            string name,
            string ovoNumber,
            string shortName,
            Article article,
            Organisation? parentOrganisation,
            string description,
            IEnumerable<Purpose> purposes,
            bool showOnVlaamseOverheidSites,
            Period validity,
            Period operationalValidity,
            IDateTimeProvider dateTimeProvider) : this()
        {
            ApplyChange(new OrganisationCreatedFromKbo(
                id,
                kboNumber.ToDigitsOnly(),
                name,
                ovoNumber,
                shortName,
                article,
                description,
                purposes.Select(x => new Events.Purpose(x.Id, x.Name)).ToList(),
                showOnVlaamseOverheidSites,
                validity.Start,
                validity.End,
                operationalValidity.Start,
                operationalValidity.End));

            if (validity.OverlapsWith(dateTimeProvider.Today))
                ApplyChange(new OrganisationBecameActive(Id));

            if (parentOrganisation == null)
                return;

            ApplyChange(new OrganisationParentAdded(
                Id,
                Id,
                parentOrganisation.Id,
                parentOrganisation.State.Name,
                null,
                null));

            ApplyChange(new ParentAssignedToOrganisation(
                Id,
                parentOrganisation.Id,
                Id));
        }

        // TODO: discuss => Can we make name a Value Object, and put validation there as well?

        public void UpdateInfo(string name,
            Article article,
            string description,
            string shortName,
            List<Purpose> purposes,
            bool showOnVlaamseOverheidSites,
            Period validity,
            Period operationalValidity,
            IDateTimeProvider dateTimeProvider)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

            if (HasKboNumber)
            {
                KboV2Guards.ThrowIfChanged(State.Name, name);
                KboV2Guards.ThrowIfChanged(State.ShortName, shortName);
            }

            if(!string.Equals(name, State.Name))
                ApplyChange(new OrganisationNameUpdated(Id, name));

            if(!string.Equals(shortName, State.ShortName))
                ApplyChange(new OrganisationShortNameUpdated(Id, shortName));

            if(!string.Equals(article, State.Article))
                ApplyChange(new OrganisationArticleUpdated(Id, article));

            if(!string.Equals(description, State.Description))
                ApplyChange(new OrganisationDescriptionUpdated(Id, description));

            var purposes2 = purposes.Select(x => new Events.Purpose(x.Id, x.Name)).ToList();
            if(!purposes2.SequenceEqual(_purposes)) // todo: don't use events as type for List
            {
                ApplyChange(new OrganisationPurposesUpdated(Id, purposes2));
            }

            if(!showOnVlaamseOverheidSites.Equals(State.ShowOnVlaamseOverheidSites))
                ApplyChange(new OrganisationShowOnVlaamseOverheidSitesUpdated(Id, showOnVlaamseOverheidSites));

            if(!validity.Equals(State.Validity))
                ApplyChange(new OrganisationValidityUpdated(Id, validity.Start, validity.End));

            if(!operationalValidity.Equals(State.OperationalValidity))
                ApplyChange(new OrganisationOperationalValidityUpdated(Id, operationalValidity.Start, operationalValidity.End));

            var validityOverlapsWithToday = validity.OverlapsWith(dateTimeProvider.Today);
            if (State.IsActive && !validityOverlapsWithToday)
                ApplyChange(new OrganisationBecameInactive(Id));

            if (!State.IsActive && validityOverlapsWithToday)
                ApplyChange(new OrganisationBecameActive(Id));
        }

        public void UpdateInfoNotLimitedByVlimpers(string description,
            List<Purpose> purposes,
            bool showOnVlaamseOverheidSites)
        {
            if(!description.Equals(State.Description))
                ApplyChange(new OrganisationDescriptionUpdated(Id, description));

            var purposes2 = purposes.Select(x => new Events.Purpose(x.Id, x.Name)).ToList();
            if(!purposes2.SequenceEqual(_purposes)) // todo: don't use events as type for List
            {
                ApplyChange(new OrganisationPurposesUpdated(Id, purposes2));
            }

            if(!showOnVlaamseOverheidSites.Equals(State.ShowOnVlaamseOverheidSites))
                ApplyChange(new OrganisationShowOnVlaamseOverheidSitesUpdated(Id, showOnVlaamseOverheidSites));
        }

        public void UpdateVlimpersOrganisationInfo(
            Article article,
            string name,
            string shortName,
            Period validity,
            Period operationalValidity,
            IDateTimeProvider dateTimeProvider)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

            if (HasKboNumber)
            {
                KboV2Guards.ThrowIfChanged(State.Name, name);
                KboV2Guards.ThrowIfChanged(State.ShortName, shortName);
            }

            if(!string.Equals(name, State.Name))
                ApplyChange(new OrganisationNameUpdated(Id, name));


            if(!string.Equals(shortName, State.ShortName))
                ApplyChange(new OrganisationShortNameUpdated(Id, shortName));


            if(!string.Equals(article, State.Article))
                ApplyChange(new OrganisationArticleUpdated(Id, article));


            if(!validity.Equals(State.Validity))
                ApplyChange(new OrganisationValidityUpdated(Id, validity.Start, validity.End));

            if(!operationalValidity.Equals(State.OperationalValidity))
                ApplyChange(new OrganisationOperationalValidityUpdated(Id, operationalValidity.Start, operationalValidity.End));

            var validityOverlapsWithToday = validity.OverlapsWith(dateTimeProvider.Today);
            if (State.IsActive && !validityOverlapsWithToday)
                ApplyChange(new OrganisationBecameInactive(Id));

            if (!State.IsActive && validityOverlapsWithToday)
                ApplyChange(new OrganisationBecameActive(Id));
        }


        public void MarkTerminationFound(IMagdaTermination magdaTermination)
        {
            if (!HasKboNumber)
                throw new OrganisationNotCoupledWithKbo();

            var termination = KboTermination.FromMagda(magdaTermination);

            if (KboState.TerminationInKbo != null && termination.Equals(KboState.TerminationInKbo.Value))
                return;

            ApplyChange(new OrganisationTerminationFoundInKbo(
                Id,
                KboState.KboNumber!.ToDigitsOnly(),
                termination.Date,
                termination.Code,
                termination.Reason));
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
            if (kboOrganisationName == State.Name &&
                kboOrganisationShortName == State.ShortName)
                return;

            ApplyChange(new OrganisationInfoUpdatedFromKbo(
                Id,
                State.OvoNumber,
                kboOrganisationName,
                kboOrganisationShortName,
                State.Name,
                State.ShortName));
        }

        public void AddParent(
            Guid organisationOrganisationParentId,
            Organisation parentOrganisation,
            Period validity,
            IDateTimeProvider dateTimeProvider)
        {
            if (parentOrganisation.State.UnderVlimpersManagement !=
                State.UnderVlimpersManagement)
                throw new VlimpersAndNonVlimpersOrganisationCannotBeInParentalRelationship();

            if (State.OrganisationParents
                .Where(organisationParent => organisationParent.OrganisationOrganisationParentId != organisationOrganisationParentId)
                .Any(organisationParent => organisationParent.Validity.OverlapsWith(validity)))
                throw new OrganisationAlreadyCoupledToParentInThisPeriod();

            ApplyChange(new OrganisationParentAdded(
                Id,
                organisationOrganisationParentId,
                parentOrganisation.Id,
                parentOrganisation.State.Name,
                validity.Start,
                validity.End));

            CheckIfCurrentParentChanged(
                new OrganisationParent(
                    organisationOrganisationParentId,
                    parentOrganisation.Id,
                    parentOrganisation.State.Name,
                    validity),
                dateTimeProvider.Today);
        }

        public void UpdateParent(
            Guid organisationOrganisationParentId,
            Organisation parentOrganisation,
            Period validity,
            IDateTimeProvider dateTimeProvider)
        {
            if (parentOrganisation.State.UnderVlimpersManagement !=
                State.UnderVlimpersManagement)
                throw new VlimpersAndNonVlimpersOrganisationCannotBeInParentalRelationship();

            if (State.OrganisationParents
                .Where(organisationParent => organisationParent.OrganisationOrganisationParentId != organisationOrganisationParentId)
                .Any(organisationParent => organisationParent.Validity.OverlapsWith(validity)))
                throw new OrganisationAlreadyCoupledToParentInThisPeriod();

            var previousParentOrganisation = State.OrganisationParents.Single(parent => parent.OrganisationOrganisationParentId == organisationOrganisationParentId);

            ApplyChange(new OrganisationParentUpdated(
                Id,
                organisationOrganisationParentId,
                parentOrganisation.Id,
                parentOrganisation.State.Name,
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
                    parentOrganisation.State.Name,
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
            if (State.OrganisationFormalFrameworks
                .Where(organisationFormalFramework => organisationFormalFramework.OrganisationFormalFrameworkId != organisationFormalFrameworkId)
                .Where(organisationFormalFramework => organisationFormalFramework.FormalFrameworkId == formalFramework.Id)
                .Any(organisationParent => organisationParent.Validity.OverlapsWith(validity)))
                throw new OrganisationAlreadyCoupledToFormalFrameworkParentInThisPeriod();

            ApplyChange(new OrganisationFormalFrameworkAdded(
                Id,
                organisationFormalFrameworkId,
                formalFramework.Id,
                formalFramework.Name,
                parentOrganisation.Id,
                parentOrganisation.State.Name,
                validity.Start,
                validity.End));

            CheckIfCurrentFormalFrameworkParentChanged(
                new OrganisationFormalFramework(
                    organisationFormalFrameworkId,
                    formalFramework.Id,
                    formalFramework.Name,
                    parentOrganisation.Id,
                    parentOrganisation.State.Name,
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
            if (State.OrganisationFormalFrameworks
                .Where(x => x.OrganisationFormalFrameworkId != organisationFormalFrameworkId)
                .Where(x => x.FormalFrameworkId == formalFramework.Id)
                .Any(organisationParent => organisationParent.Validity.OverlapsWith(validity)))
                throw new OrganisationAlreadyCoupledToFormalFrameworkParentInThisPeriod();

            var previousParentOrganisation =
                State.OrganisationFormalFrameworks
                    .Single(x => x.OrganisationFormalFrameworkId == organisationFormalFrameworkId);

            ApplyChange(new OrganisationFormalFrameworkUpdated(
                Id,
                organisationFormalFrameworkId,
                formalFramework.Id,
                formalFramework.Name,
                parentOrganisation.Id,
                parentOrganisation.State.Name,
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
                    parentOrganisation.State.Name,
                    validity),
                dateTimeProvider.Today);
        }

        public void AddKey(
            Guid organisationKeyId,
            KeyType keyType,
            string value,
            Period validity,
            Func<Guid, bool> canUseKeyType)
        {
            if (!canUseKeyType(keyType.Id))
                throw new UserIsNotAuthorizedForKeyType();

            if (State.OrganisationKeys
                .Where(organisationKey => organisationKey.KeyTypeId == keyType.Id)
                .Where(organisationKey => organisationKey.OrganisationKeyId != organisationKeyId)
                .Any(organisationKey => organisationKey.Validity.OverlapsWith(validity)))
                throw new KeyAlreadyCoupledToInThisPeriod();

            ApplyChange(new OrganisationKeyAdded(
                Id,
                organisationKeyId,
                keyType.Id,
                keyType.Name,
                value,
                validity.Start,
                validity.End));
        }

        public void UpdateKey(Guid organisationKeyId,
            KeyType keyType,
            string value,
            Period validity,
            Func<Guid, bool> canUseKeyType)
        {
            var previousOrganisationKey =
                State.OrganisationKeys.Single(key => key.OrganisationKeyId == organisationKeyId);

            if (!canUseKeyType(previousOrganisationKey.KeyTypeId) ||
                !canUseKeyType(keyType.Id))
                throw new UserIsNotAuthorizedForKeyType();

            if (State.OrganisationKeys
                .Where(organisationKey => organisationKey.KeyTypeId == keyType.Id)
                .Where(organisationKey => organisationKey.OrganisationKeyId != organisationKeyId)
                .Any(organisationKey => organisationKey.Validity.OverlapsWith(validity)))
                throw new KeyAlreadyCoupledToInThisPeriod();


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

            var previousFunctionType = State.OrganisationFunctionTypes.Single(organisationFunctionType =>
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

            if (State.OrganisationRelations
                .Where(organisationRelation => organisationRelation.OrganisationRelationId != organisationRelationId)
                .Where(organisationRelation => organisationRelation.RelatedOrganisationId == relatedOrganisation.Id)
                .Any(organisationParent => organisationParent.Validity.OverlapsWith(period)))
                throw new OrganisationAlreadyLinkedToOrganisationInThisPeriod();

            if (Id == relatedOrganisation.Id)
                throw new OrganisationCannotBeLinkedToItself();

            ApplyChange(new OrganisationRelationAdded(
                Id,
                State.Name,
                organisationRelationId,
                relatedOrganisation.Id,
                relatedOrganisation.State.Name,
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

            if (State.OrganisationRelations
                .Where(organisationRelation => organisationRelation.OrganisationRelationId != organisationRelationId)
                .Where(organisationRelation => organisationRelation.RelatedOrganisationId == relatedOrganisation.Id)
                .Any(organisationParent => organisationParent.Validity.OverlapsWith(period)))
                throw new OrganisationAlreadyLinkedToOrganisationInThisPeriod();

            if (Id == relatedOrganisation.Id)
                throw new OrganisationCannotBeLinkedToItself();

            var previousOrganisationRelation = State.OrganisationRelations.Single(organisationRelation =>
                organisationRelation.OrganisationRelationId == organisationRelationId);

            var newOrganisationRelation =
                new OrganisationRelation(
                    organisationRelationId,
                    Id,
                    State.Name,
                    relation.Id,
                    relation.Name,
                    relation.InverseName,
                    relatedOrganisation.Id,
                    relatedOrganisation.State.Name,
                    period);

            ApplyChange(new OrganisationRelationUpdated(
                Id,
                State.Name,
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
                person?.FullName ?? "",
                functionType?.Id,
                functionType?.Name ?? "",
                location?.Id,
                location?.FormattedAddress ?? "",
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
            var previousCapacity = State.OrganisationCapacities.Single(organisationCapacity =>
                organisationCapacity.OrganisationCapacityId == organisationCapacityId);

            var newOrganisationCapacity =
                new OrganisationCapacity(
                    organisationCapacityId,
                    Id,
                    capacity.Id,
                    capacity.Name,
                    person?.Id,
                    person?.FullName ?? "",
                    functionType?.Id,
                    functionType?.Name ?? "",
                    location?.Id,
                    location?.FormattedAddress ?? "",
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

            if (State.OrganisationOrganisationClassifications
                .Where(x => x.OrganisationClassificationTypeId == organisationClassificationType.Id)
                .Where(x => x.OrganisationOrganisationClassificationId != organisationOrganisationClassificationId)
                .Any(x => x.Validity.OverlapsWith(validity)))
                throw new OrganisationClassificationTypeAlreadyCoupledToInThisPeriod();

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

            if (newLegalFormClassificationId == KboState.KboLegalFormOrganisationClassification?.OrganisationClassificationId)
                return;

            if (KboState.KboLegalFormOrganisationClassification != null)
                ApplyChange(
                    new KboLegalFormOrganisationOrganisationClassificationRemoved(
                        Id,
                        KboState.KboLegalFormOrganisationClassification.OrganisationOrganisationClassificationId,
                        KboState.KboLegalFormOrganisationClassification.OrganisationClassificationTypeId,
                        KboState.KboLegalFormOrganisationClassification.OrganisationClassificationTypeName,
                        KboState.KboLegalFormOrganisationClassification.OrganisationClassificationId,
                        KboState.KboLegalFormOrganisationClassification.OrganisationClassificationName,
                        KboState.KboLegalFormOrganisationClassification.Validity.Start,
                        KboState.KboLegalFormOrganisationClassification.Validity.End,
                        KboState.KboLegalFormOrganisationClassification.Validity.End));

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

            if (State.OrganisationOrganisationClassifications
                .Where(organisationOrganisationClassification => organisationOrganisationClassification.OrganisationClassificationTypeId == organisationClassificationType.Id)
                .Where(organisationOrganisationClassification => organisationOrganisationClassification.OrganisationOrganisationClassificationId != organisationOrganisationClassificationId)
                .Any(organisationOrganisationClassification => organisationOrganisationClassification.Validity.OverlapsWith(validity)))
                throw new OrganisationClassificationTypeAlreadyCoupledToInThisPeriod();

            var previousOrganisationOrganisationClassification =
                State.OrganisationOrganisationClassifications.Single(classification => classification.OrganisationOrganisationClassificationId == organisationOrganisationClassificationId);

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
                State.OrganisationContacts.Single(contact => contact.OrganisationContactId == organisationContactId);

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

        public void AddLabel(Guid organisationLabelId,
            LabelType labelType,
            string labelValue,
            Period validity,
            Func<Guid, bool> canUseLabelType)
        {
            if (!canUseLabelType(labelType.Id))
                throw new UserIsNotAuthorizedForKeyType();

            if (State.OrganisationLabels
                .Where(organisationLabel => organisationLabel.LabelTypeId == labelType.Id)
                .Where(organisationLabel => organisationLabel.OrganisationLabelId != organisationLabelId)
                .Any(organisationLabel => organisationLabel.Validity.OverlapsWith(validity)))
                throw new LabelAlreadyCoupledToInThisPeriod();

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
            if (KboState.KboFormalNameLabel.Value == kboFormalName.Value)
                return;

            ApplyChange(
                new KboFormalNameLabelRemoved(
                    Id,
                    KboState.KboFormalNameLabel.OrganisationLabelId,
                    KboState.KboFormalNameLabel.LabelTypeId,
                    KboState.KboFormalNameLabel.LabelTypeName,
                    KboState.KboFormalNameLabel.Value,
                    KboState.KboFormalNameLabel.Validity.Start,
                    kboFormalName.ValidFrom,
                    KboState.KboFormalNameLabel.Validity.End));

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

        public void UpdateLabel(Guid organisationLabelId,
            LabelType labelType,
            string labelValue,
            Period validity)
        {
            if (State.OrganisationLabels
                .Where(organisationLabel => organisationLabel.LabelTypeId == labelType.Id)
                .Where(organisationLabel => organisationLabel.OrganisationLabelId != organisationLabelId)
                .Any(organisationLabel => organisationLabel.Validity.OverlapsWith(validity)))
                throw new LabelAlreadyCoupledToInThisPeriod();

            var previousLabel = State.OrganisationLabels.Single(label => label.OrganisationLabelId == organisationLabelId);

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

            if (State.OrganisationBuildings.AlreadyHasTheSameOrganisationAndBuildingInTheSamePeriod(organisationBuilding))
                throw new BuildingAlreadyCoupledToInThisPeriod();

            if (organisationBuilding.IsMainBuilding && State.OrganisationBuildings.OrganisationAlreadyHasAMainBuildingInTheSamePeriod(organisationBuilding))
                throw new OrganisationAlreadyHasAMainBuildingInThisPeriod();

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

            if (State.OrganisationBuildings.AlreadyHasTheSameOrganisationAndBuildingInTheSamePeriod(organisationBuilding))
                throw new BuildingAlreadyCoupledToInThisPeriod();

            if (organisationBuilding.IsMainBuilding && State.OrganisationBuildings.OrganisationAlreadyHasAMainBuildingInTheSamePeriod(organisationBuilding))
                throw new OrganisationAlreadyHasAMainBuildingInThisPeriod();

            var previousOrganisationBuilding =
                State.OrganisationBuildings.Single(x => x.OrganisationBuildingId == organisationBuildingId);

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
            Source source,
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
                    validity,
                    source);

            if (State.OrganisationLocations.AlreadyHasTheSameOrganisationAndLocationInTheSamePeriod(organisationLocation))
                throw new LocationAlreadyCoupledToInThisPeriod();

            if (organisationLocation.IsMainLocation && State.OrganisationLocations.OrganisationAlreadyHasAMainLocationInTheSamePeriod(organisationLocation, KboState.KboRegisteredOffice))
                throw new OrganisationAlreadyHasAMainLocationInThisPeriod();

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
            if (newKboRegisteredOffice?.Location?.Id == KboState.KboRegisteredOffice?.LocationId)
                return;

            if (KboState.KboRegisteredOffice != null)
                ApplyChange(
                    new KboRegisteredOfficeOrganisationLocationRemoved(
                        Id,
                        KboState.KboRegisteredOffice.OrganisationLocationId,
                        KboState.KboRegisteredOffice.LocationId,
                        KboState.KboRegisteredOffice.FormattedAddress,
                        KboState.KboRegisteredOffice.IsMainLocation,
                        KboState.KboRegisteredOffice.LocationTypeId,
                        KboState.KboRegisteredOffice.LocationTypeName,
                        KboState.KboRegisteredOffice.Validity.Start,
                        KboState.KboRegisteredOffice.Validity.End,
                        KboState.KboRegisteredOffice.Validity.End));

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

        public void UpdateKboRegisteredOfficeLocationIsMainLocation(
            OrganisationLocation newKboRegisteredOffice)
        {
            if (KboState.KboRegisteredOffice is not { } kbkLocation) return;
            if (newKboRegisteredOffice.LocationId != kbkLocation.LocationId) return;
            if (newKboRegisteredOffice.IsMainLocation == kbkLocation.IsMainLocation) return;

            ApplyChange(
                new KboRegisteredOfficeLocationIsMainLocationChanged(
                    Id,
                    newKboRegisteredOffice.OrganisationLocationId,
                    newKboRegisteredOffice.IsMainLocation));
        }

        public void UpdateLocation(
            Guid organisationLocationId,
            Location location,
            bool isMainLocation,
            LocationType locationType,
            Period validity,
            Source source,
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
                    validity,
                    source);

            if (organisationLocation == null)
                throw new ArgumentNullException(nameof(organisationLocation));

            if (organisationLocation.IsMainLocation && State.OrganisationLocations.OrganisationAlreadyHasAMainLocationInTheSamePeriod(organisationLocation, KboState.KboRegisteredOffice))
                throw new OrganisationAlreadyHasAMainLocationInThisPeriod();

            if (source == Source.Kbo)
            {
                var previousLocation = KboState.KboRegisteredOffice;

                if (previousLocation?.OrganisationLocationId != organisationLocation.OrganisationLocationId)
                    throw new InvalidOperationException();

                UpdateKboRegisteredOfficeLocationIsMainLocation(organisationLocation);
            }
            else
            {
                if (State.OrganisationLocations.AlreadyHasTheSameOrganisationAndLocationInTheSamePeriod(organisationLocation))
                    throw new LocationAlreadyCoupledToInThisPeriod();

                var previousLocation =
                    State.OrganisationLocations.Single(x => x.OrganisationLocationId == organisationLocationId);

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
            }

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
            var previousOpeningHour = State.OrganisationOpeningHours.Single(label => label.OrganisationOpeningHourId == organisationOpeningHourId);

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

        public void AddRegulation(Guid organisationRegulationId,
            RegulationTheme? regulationTheme,
            RegulationSubTheme? regulationSubTheme,
            string name,
            string? url,
            WorkRulesUrl? workRulesUrl,
            DateTime? date,
            string? description,
            string? descriptionRendered,
            Period validity)
        {
            ApplyChange(new OrganisationRegulationAdded(
                Id,
                organisationRegulationId,
                regulationTheme?.Id,
                regulationTheme?.Name,
                regulationSubTheme?.Id,
                regulationSubTheme?.Name,
                name,
                url,
                workRulesUrl,
                date,
                description,
                descriptionRendered,
                validity.Start,
                validity.End));
        }

        public void UpdateRegulation(Guid organisationRegulationId,
            RegulationTheme? regulationTheme,
            RegulationSubTheme? regulationSubTheme,
            string name,
            string? link,
            WorkRulesUrl? workRulesUrl,
            DateTime? date,
            string? description,
            string? descriptionRendered,
            Period validity)
        {
            var previousRegulation =
                State.OrganisationRegulations.Single(regulation => regulation.OrganisationRegulationId == organisationRegulationId);

            ApplyChange(new OrganisationRegulationUpdated(
                Id,
                organisationRegulationId,
                regulationTheme?.Id,
                regulationTheme?.Name,
                regulationSubTheme?.Id,
                regulationSubTheme?.Name,
                name,
                link,
                workRulesUrl,
                date,
                description,
                descriptionRendered,
                validity.Start, validity.End,
                previousRegulation.RegulationThemeId,
                previousRegulation.RegulationThemeName,
                previousRegulation.RegulationSubThemeId,
                previousRegulation.RegulationSubThemeName,
                previousRegulation.Name,
                previousRegulation.Validity.Start,
                previousRegulation.Validity.End,
                previousRegulation.Link,
                previousRegulation.WorkRulesUrl,
                previousRegulation.Date,
                previousRegulation.Description,
                previousRegulation.DescriptionRendered));
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
                State.OrganisationBankAccounts.Single(bankAccount => bankAccount.OrganisationBankAccountId == organisationBankAccountId);

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
            foreach (var kboBankAccount in KboState.KboBankAccounts)
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
                if (!KboState.KboBankAccounts.Any(x =>
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

            var newMainOrganisationBuilding = State.OrganisationBuildings.TryFindMainOrganisationBuildingValidFor(today);

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

            var newMainOrganisationLocation = State.OrganisationLocations.TryFindMainOrganisationLocationValidFor(today);

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

            var newOrganisationParent = State.OrganisationParents.SingleOrDefault(parent => parent.Validity.OverlapsWith(today));
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
                State.OrganisationFormalFrameworkParentsPerFormalFramework.ContainsKey(formalFrameworkId)
                    ? State.OrganisationFormalFrameworkParentsPerFormalFramework[formalFrameworkId]
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

            if (!State.OrganisationFormalFrameworkParentsPerFormalFramework.ContainsKey(formalFrameworkId))
            {
                var organisationFormalFramework =
                    State.OrganisationFormalFrameworks.SingleOrDefault(framework =>
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
            foreach (var organisationCapacity in State.OrganisationCapacities)
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
                State.Name,
                State.OvoNumber,
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
                KboState.KboNumber!.ToDigitsOnly(),
                KboState.NameBeforeKboCoupling,
                KboState.ShortNameBeforeKboCoupling,
                State.Name,
                State.ShortName,
                State.OvoNumber,
                KboState.KboLegalFormOrganisationClassification?.OrganisationOrganisationClassificationId,
                KboState.KboFormalNameLabel?.OrganisationLabelId,
                KboState.KboRegisteredOffice?.OrganisationLocationId,
                KboState.KboBankAccounts.Select(account => account.OrganisationBankAccountId).ToList()));
        }

        public void TerminateOrganisationBasedOnKboTermination()
        {
            if (!HasKboNumber)
                throw new OrganisationNotCoupledWithKbo();

            if (!KboState.TerminationInKbo.HasValue)
                throw new KboOrganisationNotTerminated();

            ApplyChange(
                new OrganisationTerminationSyncedWithKbo(
                    Id,
                    KboState.KboNumber!.ToDigitsOnly(),
                    State.Name,
                    State.OvoNumber,
                    KboState.TerminationInKbo.Value.Date,
                    KboState.KboLegalFormOrganisationClassification?.OrganisationOrganisationClassificationId,
                    KboState.KboFormalNameLabel?.OrganisationLabelId,
                    KboState.KboRegisteredOffice?.Validity.End.IsInfinite ?? false ? KboState.KboRegisteredOffice.OrganisationLocationId : (Guid?) null,
                    KboState.KboBankAccounts
                        .Where(account => account.Validity.End.IsInfinite)
                        .Select(account => account.OrganisationBankAccountId).ToList()));
        }

        public void TerminateOrganisation(DateTime dateOfTermination,
            OrganisationTerminationCalculator.FieldsToTerminateConfig fieldsToTerminateConfig,
            IDateTimeProvider dateTimeProvider,
            bool forceKboTermination)
        {
            if (IsTerminated)
                throw new OrganisationAlreadyTerminated();

            if (KboState.TerminationInKbo.HasValue && forceKboTermination)
                throw new OrganisationAlreadyTerminatedInKbo();

            var organisationTermination = OrganisationTerminationCalculator.GetFieldsToTerminate(dateOfTermination,
                fieldsToTerminateConfig,
                State);

            var organisationTerminationKboSummary = forceKboTermination
                ? OrganisationTerminationCalculator.GetKboFieldsToForceTerminate(dateOfTermination, KboState)
                : new OrganisationTerminationKboSummary();

            ApplyChange(
                OrganisationTerminatedV2.Create(
                    Id,
                    State,
                    KboState,
                    organisationTermination,
                    forceKboTermination,
                    organisationTerminationKboSummary,
                    dateOfTermination));

            if (HasKboNumber && KboState.TerminationInKbo.HasValue)
                TerminateOrganisationBasedOnKboTermination();

            var validityOverlapsWithToday = State.Validity.OverlapsWith(dateTimeProvider.Today);
            if (State.IsActive && !validityOverlapsWithToday)
                ApplyChange(new OrganisationBecameInactive(Id));

            if (_mainOrganisationBuilding != null &&
                organisationTermination.Buildings.ContainsKey(_mainOrganisationBuilding.OrganisationBuildingId) &&
                organisationTermination.Buildings[_mainOrganisationBuilding.OrganisationBuildingId] < dateTimeProvider.Today)
            {
                ApplyChange(new MainBuildingClearedFromOrganisation(Id, _mainOrganisationBuilding.BuildingId));
            }

            if (_mainOrganisationLocation != null &&
                organisationTermination.Locations.ContainsKey(_mainOrganisationLocation.OrganisationLocationId) &&
                organisationTermination.Locations[_mainOrganisationLocation.OrganisationLocationId] < dateTimeProvider.Today)
            {
                ApplyChange(new MainLocationClearedFromOrganisation(Id, _mainOrganisationLocation.LocationId));
            }

            foreach (var (_, parent) in State.OrganisationFormalFrameworkParentsPerFormalFramework)
            {
                if (organisationTermination.FormalFrameworks.ContainsKey(parent.OrganisationFormalFrameworkId) &&
                    organisationTermination.FormalFrameworks[parent.OrganisationFormalFrameworkId] < dateTimeProvider.Today)
                {
                    ApplyChange(new FormalFrameworkClearedFromOrganisation(
                        parent.OrganisationFormalFrameworkId,
                        Id,
                        parent.FormalFrameworkId,
                        parent.ParentOrganisationId));
                }
            }
        }

        public void PlaceUnderVlimpersManagement()
        {
            ApplyChange(new OrganisationPlacedUnderVlimpersManagement(Id));
        }

        public void ReleaseFromVlimpersManagement()
        {
            ApplyChange(new OrganisationReleasedFromVlimpersManagement(Id));
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
                State.OrganisationFormalFrameworkParentsPerFormalFramework.ContainsKey(organisationFormalFramework.FormalFrameworkId) ?
                    State.OrganisationFormalFrameworkParentsPerFormalFramework[organisationFormalFramework.FormalFrameworkId] :
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
            State.Name = @event.Name;
            State.ShortName = @event.ShortName;
            State.Article = Article.Parse(@event.Article);
            State.OvoNumber = @event.OvoNumber;
            State.Description = @event.Description;
            _purposes = @event.Purposes;
            State.ShowOnVlaamseOverheidSites = @event.ShowOnVlaamseOverheidSites;
            State.Validity = new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo));
            State.OperationalValidity = new Period(new ValidFrom(@event.OperationalValidFrom), new ValidTo(@event.OperationalValidTo));
        }

        private void Apply(OrganisationCreatedFromKbo @event)
        {
            Id = @event.OrganisationId;
            State.Name = @event.Name;
            State.ShortName = @event.ShortName;
            State.Article = Article.Parse(@event.Article);
            State.OvoNumber = @event.OvoNumber;
            State.Description = @event.Description;
            _purposes = @event.Purposes;
            State.ShowOnVlaamseOverheidSites = @event.ShowOnVlaamseOverheidSites;
            State.Validity = new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo));
            State.OperationalValidity = new Period(new ValidFrom(@event.OperationalValidFrom), new ValidTo(@event.OperationalValidTo));
            KboState.KboNumber = new KboNumber(@event.KboNumber);
            CoupledToKboFromCreation = true;
        }

        private void Apply(OrganisationInfoUpdatedFromKbo @event)
        {
            State.Name = @event.Name;
            State.ShortName = @event.ShortName;
        }

        private void Apply(OrganisationBecameActive @event)
            => State.IsActive = true;

        private void Apply(OrganisationBecameInactive @event)
            => State.IsActive = false;

        private void Apply(OrganisationInfoUpdated @event)
        {
            State.Name = @event.Name;
            State.Description = @event.Description;
            State.Article = Article.Parse(@event.Article);
            State.ShortName = @event.ShortName;
            _purposes = @event.Purposes;
            State.ShowOnVlaamseOverheidSites = @event.ShowOnVlaamseOverheidSites;
            State.Validity = new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo));
            State.OperationalValidity = new Period(new ValidFrom(@event.OperationalValidFrom), new ValidTo(@event.OperationalValidTo));
        }

        private void Apply(OrganisationNameUpdated @event)
            => State.Name = @event.Name;

        private void Apply(OrganisationShortNameUpdated @event)
            => State.ShortName = @event.ShortName;

        private void Apply(OrganisationDescriptionUpdated @event)
            => State.Description = @event.Description;

        private void Apply(OrganisationArticleUpdated @event)
            => State.Article = Article.Parse(@event.Article);

        private void Apply(OrganisationShowOnVlaamseOverheidSitesUpdated @event)
            => State.ShowOnVlaamseOverheidSites = @event.ShowOnVlaamseOverheidSites;

        private void Apply(OrganisationPurposesUpdated @event)
            => _purposes = @event.Purposes;

        private void Apply(OrganisationValidityUpdated @event)
            => State.Validity = new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo));

        private void Apply(OrganisationOperationalValidityUpdated @event)
            => State.OperationalValidity = new Period(new ValidFrom(@event.OperationalValidFrom), new ValidTo(@event.OperationalValidTo));

        private void Apply(OrganisationKeyAdded @event)
            => State.OrganisationKeys.Add(new OrganisationKey(
                @event.OrganisationKeyId,
                @event.OrganisationId,
                @event.KeyTypeId,
                @event.KeyTypeName,
                @event.Value,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));

        private void Apply(OrganisationParentAdded @event)
            => State.OrganisationParents.Add(new OrganisationParent(
                @event.OrganisationOrganisationParentId,
                @event.ParentOrganisationId,
                @event.ParentOrganisationName,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));

        private void Apply(OrganisationFormalFrameworkAdded @event)
            => State.OrganisationFormalFrameworks.Add(new OrganisationFormalFramework(
                @event.OrganisationFormalFrameworkId,
                @event.FormalFrameworkId,
                @event.FormalFrameworkName,
                @event.ParentOrganisationId,
                @event.ParentOrganisationName,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));

        private void Apply(OrganisationContactAdded @event)
            => State.OrganisationContacts.Add(new OrganisationContact(
                @event.OrganisationContactId,
                @event.OrganisationId,
                @event.ContactTypeId,
                @event.ContactTypeName,
                @event.Value,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));

        private void Apply(OrganisationFunctionAdded @event)
            => State.OrganisationFunctionTypes.Add(new OrganisationFunction(
                @event.OrganisationFunctionId,
                @event.OrganisationId,
                @event.FunctionId,
                @event.FunctionName,
                @event.PersonId,
                @event.PersonFullName,
                @event.Contacts,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));

        private void Apply(OrganisationRelationAdded @event)
            => State.OrganisationRelations.Add(new OrganisationRelation(
                @event.OrganisationRelationId,
                @event.OrganisationId,
                @event.OrganisationName,
                @event.RelationId,
                @event.RelationName,
                @event.RelationInverseName,
                @event.RelatedOrganisationId,
                @event.RelatedOrganisationName,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));

        private void Apply(OrganisationCapacityAdded @event)
            => State.OrganisationCapacities.Add(new OrganisationCapacity(
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

        private void Apply(OrganisationOrganisationClassificationAdded @event)
            => State.OrganisationOrganisationClassifications.Add(new OrganisationOrganisationClassification(
                @event.OrganisationOrganisationClassificationId,
                @event.OrganisationId,
                @event.OrganisationClassificationTypeId,
                @event.OrganisationClassificationTypeName,
                @event.OrganisationClassificationId,
                @event.OrganisationClassificationName,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));

        private void Apply(KboLegalFormOrganisationOrganisationClassificationAdded @event)
            => KboState.KboLegalFormOrganisationClassification = new OrganisationOrganisationClassification(
                @event.OrganisationOrganisationClassificationId,
                @event.OrganisationId,
                @event.OrganisationClassificationTypeId,
                @event.OrganisationClassificationTypeName,
                @event.OrganisationClassificationId,
                @event.OrganisationClassificationName,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo)));

        private void Apply(KboLegalFormOrganisationOrganisationClassificationRemoved @event)
            => KboState.KboLegalFormOrganisationClassification = null;

        private void Apply(OrganisationLabelAdded @event)
            => State.OrganisationLabels.Add(new OrganisationLabel(
                @event.OrganisationLabelId,
                @event.OrganisationId,
                @event.LabelTypeId,
                @event.LabelTypeName,
                @event.Value,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));

        private void Apply(KboFormalNameLabelAdded @event)
            => KboState.KboFormalNameLabel = new OrganisationLabel(
                @event.OrganisationLabelId,
                @event.OrganisationId,
                @event.LabelTypeId,
                @event.LabelTypeName,
                @event.Value,
                new Period(
                    new ValidFrom(@event.ValidFrom),
                    new ValidTo(@event.ValidTo)));

        private void Apply(KboFormalNameLabelRemoved @event)
            => KboState.KboFormalNameLabel = null;

        private void Apply(OrganisationBuildingAdded @event)
            => State.OrganisationBuildings.Add(new OrganisationBuilding(
                @event.OrganisationBuildingId,
                @event.OrganisationId,
                @event.BuildingId,
                @event.BuildingName,
                @event.IsMainBuilding,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));

        private void Apply(OrganisationOpeningHourAdded @event)
            => State.OrganisationOpeningHours.Add(new OrganisationOpeningHour(
                @event.OrganisationOpeningHourId,
                @event.OrganisationId,
                @event.Opens,
                @event.Closes,
                @event.DayOfWeek,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));

        private void Apply(OrganisationBuildingUpdated @event)
        {
            var organisationBuilding = new OrganisationBuilding(
                @event.OrganisationBuildingId,
                @event.OrganisationId,
                @event.BuildingId,
                @event.BuildingName,
                @event.IsMainBuilding,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo)));

            var oldOrganisationBuilding = State.OrganisationBuildings.Single(building => building.OrganisationBuildingId == @event.OrganisationBuildingId);
            State.OrganisationBuildings.Remove(oldOrganisationBuilding);
            State.OrganisationBuildings.Add(organisationBuilding);
        }

        private void Apply(OrganisationLocationAdded @event)
            => State.OrganisationLocations.Add(
                new OrganisationLocation(
                    @event.OrganisationLocationId,
                    @event.OrganisationId,
                    @event.LocationId,
                    @event.LocationFormattedAddress,
                    @event.IsMainLocation,
                    @event.LocationTypeId,
                    @event.LocationTypeName,
                    new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo)),
                    Source.Wegwijs));

        private void Apply(KboRegisteredOfficeOrganisationLocationAdded @event)
            => KboState.KboRegisteredOffice = new OrganisationLocation(
                @event.OrganisationLocationId,
                @event.OrganisationId,
                @event.LocationId,
                @event.LocationFormattedAddress,
                KboState.KboRegisteredOffice?.IsMainLocation ?? @event.IsMainLocation,
                @event.LocationTypeId,
                @event.LocationTypeName,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo)),
                Source.Kbo);

        private void Apply(KboRegisteredOfficeOrganisationLocationRemoved @event)
            => KboState.KboRegisteredOffice = null;

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
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo)),
                Source.Wegwijs);

            var oldOrganisationLocation = State.OrganisationLocations.Single(location => location.OrganisationLocationId == @event.OrganisationLocationId);
            State.OrganisationLocations.Remove(oldOrganisationLocation);
            State.OrganisationLocations.Add(organisationLocation);
        }

        private void Apply(OrganisationKeyUpdated @event)
        {
            State.OrganisationKeys.Remove(State.OrganisationKeys.Single(ob => ob.OrganisationKeyId == @event.OrganisationKeyId));
            State.OrganisationKeys.Add(
                new OrganisationKey(
                    @event.OrganisationKeyId,
                    @event.OrganisationId,
                    @event.KeyTypeId,
                    @event.KeyTypeName,
                    @event.Value,
                    new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));
        }

        private void Apply(OrganisationRegulationAdded @event)
            => State.OrganisationRegulations.Add(new OrganisationRegulation(
                @event.OrganisationRegulationId,
                @event.OrganisationId,
                @event.RegulationThemeId,
                @event.RegulationThemeName,
                @event.RegulationSubThemeId,
                @event.RegulationSubThemeName,
                @event.Name,
                @event.Uri,
                @event.WorkRulesUrl,
                @event.Date,
                @event.Description,
                @event.DescriptionRendered,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));

        private void Apply(OrganisationRegulationUpdated @event)
        {
            State.OrganisationRegulations.Remove(State.OrganisationRegulations.Single(ob => ob.OrganisationRegulationId == @event.OrganisationRegulationId));
            State.OrganisationRegulations.Add(
                new OrganisationRegulation(
                    @event.OrganisationRegulationId,
                    @event.OrganisationId,
                    @event.RegulationThemeId,
                    @event.RegulationThemeName,
                    @event.RegulationSubThemeId,
                    @event.RegulationSubThemeName,
                    @event.Name,
                    @event.Url,
                    @event.WorkRulesUrl,
                    @event.Date,
                    @event.Description,
                    @event.DescriptionRendered,
                    new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));
        }

        private void Apply(OrganisationParentUpdated @event)
        {
            var newOrganisationParent = new OrganisationParent(
                @event.OrganisationOrganisationParentId,
                @event.ParentOrganisationId,
                @event.ParentOrganisationName,
                new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo)));

            State.OrganisationParents.Remove(newOrganisationParent);
            State.OrganisationParents.Add(newOrganisationParent);

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

            State.OrganisationFormalFrameworks.Remove(newOrganisationFormalFramework);
            State.OrganisationFormalFrameworks.Add(newOrganisationFormalFramework);

            if (State.OrganisationFormalFrameworkParentsPerFormalFramework.ContainsKey(newOrganisationFormalFramework.FormalFrameworkId) &&
                Equals(State.OrganisationFormalFrameworkParentsPerFormalFramework[newOrganisationFormalFramework.FormalFrameworkId], newOrganisationFormalFramework))
            {
                State.OrganisationFormalFrameworkParentsPerFormalFramework[newOrganisationFormalFramework.FormalFrameworkId] = newOrganisationFormalFramework; // update the values
            }
        }

        private void Apply(OrganisationContactUpdated @event)
        {
            State.OrganisationContacts.Remove(State.OrganisationContacts.Single(ob => ob.OrganisationContactId == @event.OrganisationContactId));
            State.OrganisationContacts.Add(
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
            State.OrganisationFunctionTypes.Remove(State.OrganisationFunctionTypes.Single(ob => ob.OrganisationFunctionId == @event.OrganisationFunctionId));
            State.OrganisationFunctionTypes.Add(
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
            State.OrganisationRelations.Remove(State.OrganisationRelations.Single(ob => ob.OrganisationRelationId == @event.OrganisationRelationId));
            State.OrganisationRelations.Add(
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
            State.OrganisationCapacities.Remove(State.OrganisationCapacities.Single(ob => ob.OrganisationCapacityId == @event.OrganisationCapacityId));
            State.OrganisationCapacities.Add(
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
            State.OrganisationOrganisationClassifications.Remove(State.OrganisationOrganisationClassifications.Single(ob => ob.OrganisationOrganisationClassificationId == @event.OrganisationOrganisationClassificationId));
            State.OrganisationOrganisationClassifications.Add(
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
            State.OrganisationLabels.Remove(State.OrganisationLabels.Single(ob => ob.OrganisationLabelId == @event.OrganisationLabelId));
            State.OrganisationLabels.Add(
                new OrganisationLabel(
                    @event.OrganisationLabelId,
                    @event.OrganisationId,
                    @event.LabelTypeId,
                    @event.LabelTypeName,
                    @event.Value,
                    new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));
        }

        private void Apply(OrganisationBankAccountAdded @event)
            => State.OrganisationBankAccounts.Add(
                new OrganisationBankAccount(
                    @event.OrganisationBankAccountId,
                    @event.OrganisationId,
                    @event.BankAccountNumber,
                    @event.IsIban,
                    @event.Bic,
                    @event.IsBic,
                    new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));

        private void Apply(KboOrganisationBankAccountAdded @event)
            => KboState.KboBankAccounts.Add(
                new OrganisationBankAccount(
                    @event.OrganisationBankAccountId,
                    @event.OrganisationId,
                    @event.BankAccountNumber,
                    @event.IsIban,
                    @event.Bic,
                    @event.IsBic,
                    new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));

        private void Apply(KboOrganisationBankAccountRemoved @event)
            => KboState.KboBankAccounts.Remove(KboState.KboBankAccounts.Single(account =>
                account.OrganisationBankAccountId == @event.OrganisationBankAccountId));

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
                State.OrganisationBankAccounts
                    .Single(bankAccount => bankAccount.OrganisationBankAccountId == @event.OrganisationBankAccountId);

            State.OrganisationBankAccounts.Remove(oldOrganisationBankAccount);
            State.OrganisationBankAccounts.Add(organisationBankAccount);
        }

        private void Apply(OrganisationOpeningHourUpdated @event)
        {
            State.OrganisationOpeningHours.Remove(State.OrganisationOpeningHours.Single(ob => ob.OrganisationOpeningHourId == @event.OrganisationOpeningHourId));
            State.OrganisationOpeningHours.Add(
                new OrganisationOpeningHour(
                    @event.OrganisationOpeningHourId,
                    @event.OrganisationId,
                    @event.Opens,
                    @event.Closes,
                    @event.DayOfWeek,
                    new Period(new ValidFrom(@event.ValidFrom), new ValidTo(@event.ValidTo))));
        }

        private void Apply(ParentAssignedToOrganisation @event)
            => _currentOrganisationParent = State.OrganisationParents.Single(ob => ob.OrganisationOrganisationParentId == @event.OrganisationOrganisationParentId);

        private void Apply(ParentClearedFromOrganisation @event)
            => _currentOrganisationParent = null;

        private void Apply(FormalFrameworkAssignedToOrganisation @event)
            => State.OrganisationFormalFrameworkParentsPerFormalFramework[@event.FormalFrameworkId] =
                State.OrganisationFormalFrameworks.Single(off => off.OrganisationFormalFrameworkId == @event.OrganisationFormalFrameworkId);

        private void Apply(FormalFrameworkClearedFromOrganisation @event)
            => State.OrganisationFormalFrameworkParentsPerFormalFramework.Remove(@event.FormalFrameworkId);

        private void Apply(MainBuildingAssignedToOrganisation @event)
            => _mainOrganisationBuilding = State.OrganisationBuildings.Single(ob => ob.OrganisationBuildingId == @event.OrganisationBuildingId);

        private void Apply(MainBuildingClearedFromOrganisation @event)
            => _mainOrganisationBuilding = null;

        private void Apply(KboRegisteredOfficeLocationIsMainLocationChanged @event)
            => KboState.KboRegisteredOffice!.IsMainLocation = @event.IsMainLocation;

        private void Apply(MainLocationAssignedToOrganisation @event)
            => _mainOrganisationLocation = KboState.KboRegisteredOffice?.IsMainLocation == true
                ? KboState.KboRegisteredOffice
                : State.OrganisationLocations.Single(ob => ob.OrganisationLocationId == @event.OrganisationLocationId);

        private void Apply(MainLocationClearedFromOrganisation @event)
            => _mainOrganisationLocation = null;

        private void Apply(OrganisationCapacityBecameActive @event)
            => State.OrganisationCapacities
                .Single(ob => ob.OrganisationCapacityId == @event.OrganisationCapacityId)
                .IsActive = true;

        private void Apply(OrganisationCapacityBecameInactive @event)
            => State.OrganisationCapacities
                .Single(ob => ob.OrganisationCapacityId == @event.OrganisationCapacityId)
                .IsActive = false;

        private void Apply(OrganisationCoupledWithKbo @event)
        {
            KboState.KboNumber = new KboNumber(@event.KboNumber);
            KboState.NameBeforeKboCoupling = State.Name;
            KboState.ShortNameBeforeKboCoupling = State.ShortName;
            CoupledToKboFromCreation = false;
        }

        private void Apply(OrganisationCouplingWithKboCancelled @event)
        {
            State.ShortName = KboState.ShortNameBeforeKboCoupling!;
            State.Name = KboState.NameBeforeKboCoupling!;

            KboState.Clear();
        }

        private void Apply(OrganisationTerminationFoundInKbo @event)
            => KboState.TerminationInKbo = new KboTermination(@event.TerminationDate, @event.TerminationCode, @event.TerminationReason);

        private void Apply(OrganisationTerminationSyncedWithKbo @event)
            => KboState.Clear();

        private void Apply(OrganisationTerminated @event)
        {
            _dateOfTermination = @event.DateOfTermination;
            State.Validity = new Period(State.Validity.Start, new ValidTo(@event.DateOfTermination));
            State.OperationalValidity = new Period(State.OperationalValidity.Start, new ValidTo(@event.DateOfTermination));

            foreach (var (key, value) in @event.FieldsToTerminate.Buildings)
            {
                var building = State.OrganisationBuildings
                    .Single(organisationBuilding => organisationBuilding.OrganisationBuildingId == key);

                State.OrganisationBuildings.Remove(building);
                State.OrganisationBuildings.Add(building.WithValidTo(new ValidTo(value)));
            }

            foreach (var (key, value) in @event.FieldsToTerminate.Locations)
            {
                var location = State.OrganisationLocations
                    .Single(organisationLocation => organisationLocation.OrganisationLocationId == key);

                State.OrganisationLocations.Remove(location);
                State.OrganisationLocations.Add(location.WithValidTo(new ValidTo(value)));
            }

            foreach (var (key, value) in @event.FieldsToTerminate.Capacities)
            {
                var capacity = State.OrganisationCapacities
                    .Single(organisationCapacity => organisationCapacity.OrganisationCapacityId == key);

                State.OrganisationCapacities.Remove(capacity);
                State.OrganisationCapacities.Add(capacity.WithValidTo(new ValidTo(value)));
            }

            foreach (var (key, value) in @event.FieldsToTerminate.Classifications)
            {
                var classification = State.OrganisationOrganisationClassifications
                    .Single(organisationClassification => organisationClassification.OrganisationOrganisationClassificationId == key);

                State.OrganisationOrganisationClassifications.Remove(classification);
                State.OrganisationOrganisationClassifications.Add(classification.WithValidTo(new ValidTo(value)));
            }

            foreach (var (key, value) in @event.FieldsToTerminate.Contacts)
            {
                var contact = State.OrganisationContacts
                    .Single(organisationContact => organisationContact.OrganisationContactId == key);

                State.OrganisationContacts.Remove(contact);
                State.OrganisationContacts.Add(contact.WithValidTo(new ValidTo(value)));
            }

            foreach (var (key, value) in @event.FieldsToTerminate.Functions)
            {
                var function = State.OrganisationFunctionTypes
                    .Single(organisationFunction => organisationFunction.OrganisationFunctionId == key);

                State.OrganisationFunctionTypes.Remove(function);
                State.OrganisationFunctionTypes.Add(function.WithValidTo(new ValidTo(value)));
            }

            foreach (var (key, value) in @event.FieldsToTerminate.Labels)
            {
                var label = State.OrganisationLabels
                    .Single(organisationLabel => organisationLabel.OrganisationLabelId == key);

                State.OrganisationLabels.Remove(label);
                State.OrganisationLabels.Add(label.WithValidTo(new ValidTo(value)));
            }

            foreach (var (key, value) in @event.FieldsToTerminate.Parents)
            {
                var parent = State.OrganisationParents
                    .Single(organisationParent => organisationParent.OrganisationOrganisationParentId == key);

                State.OrganisationParents.Remove(parent);
                State.OrganisationParents.Add(parent.WithValidTo(new ValidTo(value)));
            }

            foreach (var (key, value) in @event.FieldsToTerminate.Relations)
            {
                var relation = State.OrganisationRelations
                    .Single(organisationRelation => organisationRelation.OrganisationRelationId == key);

                State.OrganisationRelations.Remove(relation);
                State.OrganisationRelations.Add(relation.WithValidTo(new ValidTo(value)));
            }

            foreach (var (key, value) in @event.FieldsToTerminate.BankAccounts)
            {
                var bankAccount = State.OrganisationBankAccounts
                    .Single(organisationBankAccount => organisationBankAccount.OrganisationBankAccountId == key);

                State.OrganisationBankAccounts.Remove(bankAccount);
                State.OrganisationBankAccounts.Add(bankAccount.WithValidTo(new ValidTo(value)));
            }

            foreach (var (key, value) in @event.FieldsToTerminate.FormalFrameworks)
            {
                var formalFramework = State.OrganisationFormalFrameworks
                    .Single(organisationFormalFramework => organisationFormalFramework.OrganisationFormalFrameworkId == key);

                State.OrganisationFormalFrameworks.Remove(formalFramework);
                State.OrganisationFormalFrameworks.Add(formalFramework.WithValidTo(new ValidTo(value)));
            }

            foreach (var (key, value) in @event.FieldsToTerminate.OpeningHours)
            {
                var openingHour = State.OrganisationOpeningHours
                    .Single(organisationOpeningHour => organisationOpeningHour.OrganisationOpeningHourId == key);

                State.OrganisationOpeningHours.Remove(openingHour);
                State.OrganisationOpeningHours.Add(openingHour.WithValidTo(new ValidTo(value)));
            }

            // We don't need to clear KBO state here if !ForcedKboTermination.
            // If the organisation is terminated according to KBO,
            // SyncKboTermination() will take care of clearing the KBO state by
            // publishing a OrganisationTerminationSyncedWithKbo event.

            if (@event.ForcedKboTermination)
                KboState.Clear();
        }

        private void Apply(OrganisationPlacedUnderVlimpersManagement @event)
        {
            State.UnderVlimpersManagement = true;
        }

        private void Apply(OrganisationReleasedFromVlimpersManagement @event)
        {
            State.UnderVlimpersManagement = false;
        }

        private void Apply(OrganisationTerminatedV2 @event)
        {
            _dateOfTermination = @event.DateOfTermination;
            State.Validity = new Period(State.Validity.Start, new ValidTo(@event.DateOfTermination));
            State.OperationalValidity = new Period(State.OperationalValidity.Start, new ValidTo(@event.DateOfTermination));

            foreach (var (key, value) in @event.FieldsToTerminate.Buildings)
            {
                var building = State.OrganisationBuildings
                    .Single(organisationBuilding => organisationBuilding.OrganisationBuildingId == key);

                State.OrganisationBuildings.Remove(building);
                State.OrganisationBuildings.Add(building.WithValidTo(new ValidTo(value)));
            }

            foreach (var (key, value) in @event.FieldsToTerminate.Locations)
            {
                var location = State.OrganisationLocations
                    .Single(organisationLocation => organisationLocation.OrganisationLocationId == key);

                State.OrganisationLocations.Remove(location);
                State.OrganisationLocations.Add(location.WithValidTo(new ValidTo(value)));
            }

            foreach (var (key, value) in @event.FieldsToTerminate.Capacities)
            {
                var capacity = State.OrganisationCapacities
                    .Single(organisationCapacity => organisationCapacity.OrganisationCapacityId == key);

                State.OrganisationCapacities.Remove(capacity);
                State.OrganisationCapacities.Add(capacity.WithValidTo(new ValidTo(value)));
            }

            foreach (var (key, value) in @event.FieldsToTerminate.Classifications)
            {
                var classification = State.OrganisationOrganisationClassifications
                    .Single(organisationClassification => organisationClassification.OrganisationOrganisationClassificationId == key);

                State.OrganisationOrganisationClassifications.Remove(classification);
                State.OrganisationOrganisationClassifications.Add(classification.WithValidTo(new ValidTo(value)));
            }

            foreach (var (key, value) in @event.FieldsToTerminate.Contacts)
            {
                var contact = State.OrganisationContacts
                    .Single(organisationContact => organisationContact.OrganisationContactId == key);

                State.OrganisationContacts.Remove(contact);
                State.OrganisationContacts.Add(contact.WithValidTo(new ValidTo(value)));
            }

            foreach (var (key, value) in @event.FieldsToTerminate.Functions)
            {
                var function = State.OrganisationFunctionTypes
                    .Single(organisationFunction => organisationFunction.OrganisationFunctionId == key);

                State.OrganisationFunctionTypes.Remove(function);
                State.OrganisationFunctionTypes.Add(function.WithValidTo(new ValidTo(value)));
            }

            foreach (var (key, value) in @event.FieldsToTerminate.Labels)
            {
                var label = State.OrganisationLabels
                    .Single(organisationLabel => organisationLabel.OrganisationLabelId == key);

                State.OrganisationLabels.Remove(label);
                State.OrganisationLabels.Add(label.WithValidTo(new ValidTo(value)));
            }

            foreach (var (key, value) in @event.FieldsToTerminate.Relations)
            {
                var relation = State.OrganisationRelations
                    .Single(organisationRelation => organisationRelation.OrganisationRelationId == key);

                State.OrganisationRelations.Remove(relation);
                State.OrganisationRelations.Add(relation.WithValidTo(new ValidTo(value)));
            }

            foreach (var (key, value) in @event.FieldsToTerminate.BankAccounts)
            {
                var bankAccount = State.OrganisationBankAccounts
                    .Single(organisationBankAccount => organisationBankAccount.OrganisationBankAccountId == key);

                State.OrganisationBankAccounts.Remove(bankAccount);
                State.OrganisationBankAccounts.Add(bankAccount.WithValidTo(new ValidTo(value)));
            }

            foreach (var (key, value) in @event.FieldsToTerminate.FormalFrameworks)
            {
                var formalFramework = State.OrganisationFormalFrameworks
                    .Single(organisationFormalFramework => organisationFormalFramework.OrganisationFormalFrameworkId == key);

                State.OrganisationFormalFrameworks.Remove(formalFramework);
                State.OrganisationFormalFrameworks.Add(formalFramework.WithValidTo(new ValidTo(value)));
            }

            foreach (var (key, value) in @event.FieldsToTerminate.OpeningHours)
            {
                var openingHour = State.OrganisationOpeningHours
                    .Single(organisationOpeningHour => organisationOpeningHour.OrganisationOpeningHourId == key);

                State.OrganisationOpeningHours.Remove(openingHour);
                State.OrganisationOpeningHours.Add(openingHour.WithValidTo(new ValidTo(value)));
            }

            // We don't need to clear KBO state here if !ForcedKboTermination.
            // If the organisation is terminated according to KBO,
            // SyncKboTermination() will take care of clearing the KBO state by
            // publishing a OrganisationTerminationSyncedWithKbo event.

            if (@event.ForcedKboTermination)
                KboState.Clear();
        }

        public IEnumerable<OrganisationParent> ParentsInPeriod(Period validity)
        {
            return State.OrganisationParents.Where(parent => parent.Validity.OverlapsWith(validity));
        }

        public IEnumerable<OrganisationFormalFramework> ParentsInPeriod(FormalFramework formalFramework, Period validity)
        {
            return State.OrganisationFormalFrameworks
                .Where(parent => parent.Validity.OverlapsWith(validity))
                .Where(parent => parent.FormalFrameworkId == formalFramework.Id);
        }

        public void ThrowIfTerminated(IUser user)
        {
            if (IsTerminated &&
                !user.IsInRole(Role.OrganisationRegistryBeheerder) &&
                !user.IsInRole(Role.AutomatedTask) &&
                !(user.IsInRole(Role.VlimpersBeheerder) && State.UnderVlimpersManagement))
                throw new OrganisationAlreadyTerminated();
        }

        public void ThrowIfUnauthorizedForVlimpers(IUser user)
        {
            if (State.UnderVlimpersManagement && !user.IsAuthorizedForVlimpersOrganisations)
                throw new UserIsNotAuthorizedForVlimpersOrganisations();
        }

        public void ThrowIfUnauthorizedForNonVlimpers(IUser user)
        {
            if (!State.UnderVlimpersManagement && user.IsAuthorizedForVlimpersOrganisations)
                throw new UserIsNotAuthorizedForOrganisation();
        }
    }
}
