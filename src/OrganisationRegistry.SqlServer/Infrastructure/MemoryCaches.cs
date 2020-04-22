namespace OrganisationRegistry.SqlServer.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using OrganisationRegistry.Body.Events;
    using OrganisationRegistry.Building.Events;
    using OrganisationRegistry.ContactType.Events;
    using OrganisationRegistry.Infrastructure.AppSpecific;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.LabelType.Events;
    using OrganisationRegistry.Location.Events;
    using OrganisationRegistry.Organisation.Events;

    public enum MemoryCacheType
    {
        OvoNumbers,
        OrganisationNames,
        OrganisationShortNames,
        OrganisationParents,
        OrganisationValidFroms,
        OrganisationValidTos,

        BodyNames,

        LabelTypeNames,
        BuildingNames,
        LocationNames,
        ContactTypeNames,

        BodySeatNames,
        BodySeatNumbers,
        IsSeatPaid
    }

    // Scoped as SingleInstance()
    public class MemoryCaches : IMemoryCaches
    {
        private readonly OrganisationRegistryContext _context;

        private Dictionary<Guid, string> _ovoNumbers;
        private Dictionary<Guid, string> _organisationNames;
        private Dictionary<Guid, string> _organisationShortNames;
        private Dictionary<Guid, Guid?> _organisationParents;
        private Dictionary<Guid, DateTime?> _organisationValidFroms;
        private Dictionary<Guid, DateTime?> _organisationValidTos;

        private Dictionary<Guid, string> _bodyNames;
        private Dictionary<Guid, string> _bodySeatNames;
        private Dictionary<Guid, string> _bodySeatNumbers;

        private Dictionary<Guid, string> _labelTypeNames;
        private Dictionary<Guid, string> _buildingNames;
        private Dictionary<Guid, string> _locationNames;
        private Dictionary<Guid, string> _contactTypeNames;

        private Dictionary<Guid, bool> _isSeatPaid;

        public IReadOnlyDictionary<Guid, string> OvoNumbers =>
            ToReadOnlyDictionary(GetCache<string>(MemoryCacheType.OvoNumbers));

        public IReadOnlyDictionary<Guid, string> OrganisationNames =>
            ToReadOnlyDictionary(GetCache<string>(MemoryCacheType.OrganisationNames));

        public IReadOnlyDictionary<Guid, string> OrganisationShortNames =>
            ToReadOnlyDictionary(GetCache<string>(MemoryCacheType.OrganisationShortNames));

        public IReadOnlyDictionary<Guid, Guid?> OrganisationParents =>
            ToReadOnlyDictionary(GetCache<Guid?>(MemoryCacheType.OrganisationParents));

        public IReadOnlyDictionary<Guid, DateTime?> OrganisationValidFroms =>
            ToReadOnlyDictionary(GetCache<DateTime?>(MemoryCacheType.OrganisationValidFroms));

        public IReadOnlyDictionary<Guid, DateTime?> OrganisationValidTos =>
            ToReadOnlyDictionary(GetCache<DateTime?>(MemoryCacheType.OrganisationValidTos));

        public IReadOnlyDictionary<Guid, string> BodyNames =>
            ToReadOnlyDictionary(GetCache<string>(MemoryCacheType.BodyNames));

        public IReadOnlyDictionary<Guid, string> BodySeatNames =>
            ToReadOnlyDictionary(GetCache<string>(MemoryCacheType.BodySeatNames));

        public IReadOnlyDictionary<Guid, string> BodySeatNumbers =>
            ToReadOnlyDictionary(GetCache<string>(MemoryCacheType.BodySeatNumbers));

        public IReadOnlyDictionary<Guid, string> LabelTypeNames =>
            ToReadOnlyDictionary(GetCache<string>(MemoryCacheType.LabelTypeNames));

        public IReadOnlyDictionary<Guid, string> BuildingNames =>
            ToReadOnlyDictionary(GetCache<string>(MemoryCacheType.BuildingNames));

        public IReadOnlyDictionary<Guid, string> LocationNames =>
            ToReadOnlyDictionary(GetCache<string>(MemoryCacheType.LocationNames));

        public IReadOnlyDictionary<Guid, string> ContactTypeNames =>
            ToReadOnlyDictionary(GetCache<string>(MemoryCacheType.ContactTypeNames));

        public IReadOnlyDictionary<Guid, bool> IsSeatPaid =>
            ToReadOnlyDictionary(GetCache<bool>(MemoryCacheType.IsSeatPaid));

        public MemoryCaches(OrganisationRegistryContext context)
        {
            _context = context;

            foreach (MemoryCacheType memoryCacheType in Enum.GetValues(typeof(MemoryCacheType)))
                ResetCache(memoryCacheType);
        }

        internal Dictionary<Guid, T> GetCache<T>(MemoryCacheType cacheType)
        {
            switch (cacheType)
            {
                case MemoryCacheType.OvoNumbers:
                    return _ovoNumbers as Dictionary<Guid, T>;

                case MemoryCacheType.OrganisationNames:
                    return _organisationNames as Dictionary<Guid, T>;

                case MemoryCacheType.OrganisationShortNames:
                    return _organisationShortNames as Dictionary<Guid, T>;

                case MemoryCacheType.OrganisationParents:
                    return _organisationParents as Dictionary<Guid, T>;

                case MemoryCacheType.OrganisationValidFroms:
                    return _organisationValidFroms as Dictionary<Guid, T>;

                case MemoryCacheType.OrganisationValidTos:
                    return _organisationValidTos as Dictionary<Guid, T>;

                case MemoryCacheType.BodyNames:
                    return _bodyNames as Dictionary<Guid, T>;

                case MemoryCacheType.BodySeatNames:
                    return _bodySeatNames as Dictionary<Guid, T>;

                case MemoryCacheType.BodySeatNumbers:
                    return _bodySeatNumbers as Dictionary<Guid, T>;

                case MemoryCacheType.LabelTypeNames:
                    return _labelTypeNames as Dictionary<Guid, T>;

                case MemoryCacheType.BuildingNames:
                    return _buildingNames as Dictionary<Guid, T>;

                case MemoryCacheType.LocationNames:
                    return _locationNames as Dictionary<Guid, T>;

                case MemoryCacheType.ContactTypeNames:
                    return _contactTypeNames as Dictionary<Guid, T>;

                case MemoryCacheType.IsSeatPaid:
                    return _isSeatPaid as Dictionary<Guid, T>;

                default:
                    throw new ArgumentOutOfRangeException(nameof(cacheType), cacheType, null);
            }
        }

        internal void ResetCache(MemoryCacheType cacheType)
        {
            switch (cacheType)
            {
                case MemoryCacheType.OvoNumbers:
                    _ovoNumbers = _context.OrganisationDetail.BuildMemoryCache(item => item.Id, item => item.OvoNumber);
                    break;

                case MemoryCacheType.OrganisationNames:
                    _organisationNames = _context.OrganisationDetail.BuildMemoryCache(item => item.Id, item => item.Name);
                    break;

                case MemoryCacheType.OrganisationShortNames:
                    _organisationShortNames = _context.OrganisationDetail.BuildMemoryCache(item => item.Id, item => item.ShortName);
                    break;

                case MemoryCacheType.OrganisationParents:
                    _organisationParents = _context.OrganisationDetail.BuildMemoryCache(item => item.Id, item => item.ParentOrganisationId);
                    break;

                case MemoryCacheType.OrganisationValidFroms:
                    _organisationValidFroms = _context.OrganisationDetail.BuildMemoryCache(item => item.Id, item => item.ValidFrom);
                    break;

                case MemoryCacheType.OrganisationValidTos:
                    _organisationValidTos = _context.OrganisationDetail.BuildMemoryCache(item => item.Id, item => item.ValidTo);
                    break;

                case MemoryCacheType.BodyNames:
                    _bodyNames = _context.BodyDetail.BuildMemoryCache(item => item.Id, item => item.Name);
                    break;

                case MemoryCacheType.BodySeatNames:
                    _bodySeatNames = _context.BodySeatList.BuildMemoryCache(item => item.BodySeatId, item => item.Name);
                    break;

                case MemoryCacheType.BodySeatNumbers:
                    _bodySeatNumbers = _context.BodySeatList.BuildMemoryCache(item => item.BodySeatId, item => item.BodySeatNumber);
                    break;

                case MemoryCacheType.LabelTypeNames:
                    _labelTypeNames = _context.LabelTypeList.BuildMemoryCache(item => item.Id, item => item.Name);
                    break;

                case MemoryCacheType.BuildingNames:
                    _buildingNames = _context.BuildingList.BuildMemoryCache(item => item.Id, item => item.Name);
                    break;

                case MemoryCacheType.LocationNames:
                    _locationNames = _context.LocationList.BuildMemoryCache(item => item.Id, item => item.FormattedAddress);
                    break;

                case MemoryCacheType.ContactTypeNames:
                    _contactTypeNames = _context.ContactTypeList.BuildMemoryCache(item => item.Id, item => item.Name);
                    break;

                case MemoryCacheType.IsSeatPaid:
                    _isSeatPaid = _context.BodySeatList.BuildMemoryCache(item => item.BodySeatId, item => item.PaidSeat);
                    break;

                default:
                    throw new ArgumentOutOfRangeException(nameof(cacheType), cacheType, null);
            }
        }

        private static ReadOnlyDictionary<Guid, T> ToReadOnlyDictionary<T>(IDictionary<Guid, T> dictionary)
        {
            return new ReadOnlyDictionary<Guid, T>(dictionary);
        }
    }

    public interface IMemoryCachesMaintainer :
        IEventHandler<LabelTypeCreated>,
        IEventHandler<LabelTypeUpdated>,
        IEventHandler<BuildingCreated>,
        IEventHandler<BuildingUpdated>,
        IEventHandler<LocationCreated>,
        IEventHandler<LocationUpdated>,
        IEventHandler<ContactTypeCreated>,
        IEventHandler<ContactTypeUpdated>,
        IEventHandler<OrganisationCreated>,
        IEventHandler<OrganisationCreatedFromKbo>,
        IEventHandler<OrganisationInfoUpdated>,
        IEventHandler<OrganisationInfoUpdatedFromKbo>,
        IEventHandler<ParentAssignedToOrganisation>,
        IEventHandler<OrganisationParentUpdated>,
        IEventHandler<ParentClearedFromOrganisation>,
        IEventHandler<BodyRegistered>,
        IEventHandler<BodyInfoChanged>,
        IEventHandler<BodySeatAdded>,
        IEventHandler<BodySeatUpdated>,
        IEventHandler<ResetMemoryCache>
    {
    }

    public class MemoryCachesMaintainer : IMemoryCachesMaintainer
    {
        private readonly MemoryCaches _memoryCaches;

        public MemoryCachesMaintainer(MemoryCaches memoryCaches)
        {
            _memoryCaches = memoryCaches;
        }

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<BodyRegistered> message)
        {
            _memoryCaches.GetCache<string>(MemoryCacheType.BodyNames)
                .UpdateMemoryCache(message.Body.BodyId, message.Body.Name);
        }

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<BodyInfoChanged> message)
        {
            _memoryCaches.GetCache<string>(MemoryCacheType.BodyNames)
                .UpdateMemoryCache(message.Body.BodyId, message.Body.Name);
        }

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<BodySeatAdded> message)
        {
            _memoryCaches.GetCache<string>(MemoryCacheType.BodySeatNames)
                .UpdateMemoryCache(message.Body.BodyId, message.Body.Name);

            _memoryCaches.GetCache<string>(MemoryCacheType.BodySeatNumbers)
                .UpdateMemoryCache(message.Body.BodyId, message.Body.BodySeatNumber);

            _memoryCaches.GetCache<bool>(MemoryCacheType.IsSeatPaid)
                .UpdateMemoryCache(message.Body.BodySeatId, message.Body.PaidSeat);
        }

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<BodySeatUpdated> message)
        {
            _memoryCaches.GetCache<string>(MemoryCacheType.BodySeatNames)
                .UpdateMemoryCache(message.Body.BodyId, message.Body.Name);

            _memoryCaches.GetCache<bool>(MemoryCacheType.IsSeatPaid)
                .UpdateMemoryCache(message.Body.BodySeatId, message.Body.PaidSeat);
        }

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<LabelTypeCreated> message)
        {
            _memoryCaches.GetCache<string>(MemoryCacheType.LabelTypeNames)
                .UpdateMemoryCache(message.Body.LabelTypeId, message.Body.Name);
        }

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<LabelTypeUpdated> message)
        {
            _memoryCaches.GetCache<string>(MemoryCacheType.LabelTypeNames)
                .UpdateMemoryCache(message.Body.LabelTypeId, message.Body.Name);
        }

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<BuildingCreated> message)
        {
            _memoryCaches.GetCache<string>(MemoryCacheType.BuildingNames)
                .UpdateMemoryCache(message.Body.BuildingId, message.Body.Name);
        }

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<BuildingUpdated> message)
        {
            _memoryCaches.GetCache<string>(MemoryCacheType.BuildingNames)
                .UpdateMemoryCache(message.Body.BuildingId, message.Body.Name);
        }

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<LocationCreated> message)
        {
            _memoryCaches.GetCache<string>(MemoryCacheType.LocationNames)
                .UpdateMemoryCache(message.Body.LocationId, message.Body.FormattedAddress);
        }

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<LocationUpdated> message)
        {
            _memoryCaches.GetCache<string>(MemoryCacheType.LocationNames)
                .UpdateMemoryCache(message.Body.LocationId, message.Body.FormattedAddress);
        }

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<ContactTypeCreated> message)
        {
            _memoryCaches.GetCache<string>(MemoryCacheType.ContactTypeNames)
                .UpdateMemoryCache(message.Body.ContactTypeId, message.Body.Name);
        }

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<ContactTypeUpdated> message)
        {
            _memoryCaches.GetCache<string>(MemoryCacheType.ContactTypeNames)
                .UpdateMemoryCache(message.Body.ContactTypeId, message.Body.Name);
        }

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<OrganisationCreated> message)
        {
            _memoryCaches.GetCache<string>(MemoryCacheType.OvoNumbers)
                .UpdateMemoryCache(message.Body.OrganisationId, message.Body.OvoNumber);

            _memoryCaches.GetCache<string>(MemoryCacheType.OrganisationNames)
                .UpdateMemoryCache(message.Body.OrganisationId, message.Body.Name);

            _memoryCaches.GetCache<string>(MemoryCacheType.OrganisationShortNames)
                .UpdateMemoryCache(message.Body.OrganisationId, message.Body.ShortName);

            _memoryCaches.GetCache<DateTime?>(MemoryCacheType.OrganisationValidFroms)
                .UpdateMemoryCache(message.Body.OrganisationId, message.Body.ValidFrom);

            _memoryCaches.GetCache<DateTime?>(MemoryCacheType.OrganisationValidTos)
                .UpdateMemoryCache(message.Body.OrganisationId, message.Body.ValidTo);
        }

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<OrganisationCreatedFromKbo> message)
        {
            _memoryCaches.GetCache<string>(MemoryCacheType.OvoNumbers)
                .UpdateMemoryCache(message.Body.OrganisationId, message.Body.OvoNumber);

            _memoryCaches.GetCache<string>(MemoryCacheType.OrganisationNames)
                .UpdateMemoryCache(message.Body.OrganisationId, message.Body.Name);

            _memoryCaches.GetCache<string>(MemoryCacheType.OrganisationShortNames)
                .UpdateMemoryCache(message.Body.OrganisationId, message.Body.ShortName);

            _memoryCaches.GetCache<DateTime?>(MemoryCacheType.OrganisationValidFroms)
                .UpdateMemoryCache(message.Body.OrganisationId, message.Body.ValidFrom);

            _memoryCaches.GetCache<DateTime?>(MemoryCacheType.OrganisationValidTos)
                .UpdateMemoryCache(message.Body.OrganisationId, message.Body.ValidTo);
        }

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<OrganisationInfoUpdated> message)
        {
            _memoryCaches.GetCache<string>(MemoryCacheType.OvoNumbers)
                .UpdateMemoryCache(message.Body.OrganisationId, message.Body.OvoNumber);

            _memoryCaches.GetCache<string>(MemoryCacheType.OrganisationNames)
                .UpdateMemoryCache(message.Body.OrganisationId, message.Body.Name);

            _memoryCaches.GetCache<string>(MemoryCacheType.OrganisationShortNames)
                .UpdateMemoryCache(message.Body.OrganisationId, message.Body.ShortName);

            _memoryCaches.GetCache<DateTime?>(MemoryCacheType.OrganisationValidFroms)
                .UpdateMemoryCache(message.Body.OrganisationId, message.Body.ValidFrom);

            _memoryCaches.GetCache<DateTime?>(MemoryCacheType.OrganisationValidTos)
                .UpdateMemoryCache(message.Body.OrganisationId, message.Body.ValidTo);
        }

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        {
            _memoryCaches.GetCache<string>(MemoryCacheType.OrganisationNames)
                .UpdateMemoryCache(message.Body.OrganisationId, message.Body.Name);

            _memoryCaches.GetCache<string>(MemoryCacheType.OrganisationShortNames)
                .UpdateMemoryCache(message.Body.OrganisationId, message.Body.ShortName);
        }

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<ParentAssignedToOrganisation> message)
        {
            _memoryCaches.GetCache<Guid?>(MemoryCacheType.OrganisationParents)
                .UpdateMemoryCache(message.Body.OrganisationId, message.Body.ParentOrganisationId);
        }

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<OrganisationParentUpdated> message)
        {
            _memoryCaches.GetCache<Guid?>(MemoryCacheType.OrganisationParents)
                .UpdateMemoryCache(message.Body.OrganisationId, message.Body.ParentOrganisationId);
        }

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<ParentClearedFromOrganisation> message)
        {
            _memoryCaches.GetCache<Guid?>(MemoryCacheType.OrganisationParents)
                .UpdateMemoryCache(message.Body.OrganisationId, null);
        }

        public async Task Handle(DbConnection _, DbTransaction __, IEnvelope<ResetMemoryCache> message)
        {
            CheckResetCache(message.Body.Events, new[] { typeof(LabelTypeCreated), typeof(LabelTypeUpdated) }, new[] { MemoryCacheType.LabelTypeNames });

            CheckResetCache(message.Body.Events, new[] { typeof(BuildingCreated), typeof(BuildingUpdated) }, new[] { MemoryCacheType.BuildingNames });

            CheckResetCache(message.Body.Events, new[] { typeof(LocationCreated), typeof(LocationUpdated) }, new[] { MemoryCacheType.LocationNames });

            CheckResetCache(message.Body.Events, new[] { typeof(ContactTypeCreated), typeof(ContactTypeUpdated) }, new[] { MemoryCacheType.ContactTypeNames });

            CheckResetCache(message.Body.Events, new[] { typeof(BodyRegistered), typeof(BodyInfoChanged) }, new[] { MemoryCacheType.BodyNames });

            CheckResetCache(message.Body.Events, new[] { typeof(BodySeatAdded), typeof(BodySeatUpdated) }, new[]
            {
                MemoryCacheType.BodySeatNames,
                MemoryCacheType.BodySeatNumbers,
                MemoryCacheType.IsSeatPaid
            });

            CheckResetCache(
                message.Body.Events,
                new[]
                {
                    typeof(OrganisationCreated),
                    typeof(OrganisationCreatedFromKbo),
                    typeof(OrganisationInfoUpdated),
                    typeof(OrganisationInfoUpdatedFromKbo)
                },
                new[]
                {
                    MemoryCacheType.OvoNumbers,
                    MemoryCacheType.OrganisationNames,
                    MemoryCacheType.OrganisationShortNames,
                    MemoryCacheType.OrganisationValidFroms,
                    MemoryCacheType.OrganisationValidTos
                });

            CheckResetCache(
                message.Body.Events,
                new[]
                {
                    typeof(ParentAssignedToOrganisation),
                    typeof(OrganisationParentUpdated),
                    typeof(ParentClearedFromOrganisation)
                },
                new[] { MemoryCacheType.OrganisationParents });
        }

        private void CheckResetCache(IEnumerable<IEvent> events, Type[] eventTypes, IEnumerable<MemoryCacheType> memoryCacheTypes)
        {
            if (events.Any(x => eventTypes.Contains(x.GetType())))
                foreach (var memoryCacheType in memoryCacheTypes)
                    _memoryCaches.ResetCache(memoryCacheType);
        }
    }
}
