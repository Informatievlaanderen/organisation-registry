namespace OrganisationRegistry.SqlServer.Infrastructure
{
    using System.Reflection;
    using Body;
    using Body.ScheduledActions.Organisation;
    using Body.ScheduledActions.PeopleAssignedToBodyMandates;
    using BodyClassification;
    using BodyClassificationType;
    using Building;
    using Capacity;
    using Configuration;
    using ContactType;
    using DelegationAssignments;
    using Delegations;
    using ElasticSearchProjections;
    using Event;
    using FormalFramework;
    using FormalFrameworkCategory;
    using FunctionType;
    using KboSyncQueue;
    using KeyType;
    using LabelType;
    using LifecyclePhaseType;
    using Location;
    using LocationType;
    using Log;
    using Magda;
    using MandateRoleType;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Design;
    using Organisation;
    using Organisation.ScheduledActions.FormalFramework;
    using Organisation.ScheduledActions.Parent;
    using OrganisationClassification;
    using OrganisationClassificationType;
    using OrganisationRelationType;
    using Person;
    using ProjectionState;
    using Purpose;
    using Reporting;
    using SeatType;
    using Security;

    public class OrganisationRegistryContext : DbContext
    {
        public DbSet<EventListItem> Events { get; set; }
        public DbSet<ConfigurationListItem> Configuration { get; set; }

        public DbSet<Log> Logs { get; set; }

        public DbSet<MagdaCallReference> MagdaCallReferences { get; set; }

        public DbSet<BodyDetail> BodyDetail { get; set; }
        public DbSet<BodyContactListItem> BodyContactList { get; set; }
        public DbSet<BodyFormalFrameworkListItem> BodyFormalFrameworkList { get; set; }
        public DbSet<BodyLifecyclePhaseListItem> BodyLifecyclePhaseList { get; set; }
        public DbSet<BodyLifecyclePhaseValidity> BodyLifecyclePhaseValidities { get; set; }
        public DbSet<BodyListItem> BodyList { get; set; }
        public DbSet<BodyMandateListItem> BodyMandateList { get; set; }
        public DbSet<BodySeatCacheItemForBodyMandateList> BodySeatCacheForBodyMandateList { get; set; }
        public DbSet<BodyOrganisationListItem> BodyOrganisationList { get; set; }
        public DbSet<BodySeatListItem> BodySeatList { get; set; }
        public DbSet<BodyBodyClassificationListItem> BodyBodyClassificationList { get; set; }

        public DbSet<BodyClassificationListItem> BodyClassificationList { get; set; }
        public DbSet<BodyClassificationTypeListItem> BodyClassificationTypeList { get; set; }

        public DbSet<DelegationListItem> DelegationList { get; set; }
        public DbSet<DelegationAssignmentListItem> DelegationAssignmentList { get; set; }

        public DbSet<ActiveBodyOrganisationListItem> ActiveBodyOrganisationList { get; set; }
        public DbSet<FutureActiveBodyOrganisationListItem> FutureActiveBodyOrganisationList { get; set; }

        public DbSet<ActivePeopleAssignedToBodyMandateListItem> ActivePeopleAssignedToBodyMandatesList { get; set; }
        public DbSet<FuturePeopleAssignedToBodyMandatesListItem> FuturePeopleAssignedToBodyMandatesList { get; set; }

        public DbSet<BuildingListItem> BuildingList { get; set; }
        public DbSet<CapacityListItem> CapacityList { get; set; }
        public DbSet<ContactTypeListItem> ContactTypeList { get; set; }
        public DbSet<FormalFrameworkListItem> FormalFrameworkList { get; set; }
        public DbSet<FormalFrameworkCategoryListItem> FormalFrameworkCategoryList { get; set; }
        public DbSet<FunctionTypeListItem> FunctionTypeList { get; set; }
        public DbSet<KeyTypeListItem> KeyTypeList { get; set; }
        public DbSet<LabelTypeListItem> LabelTypeList { get; set; }
        public DbSet<LifecyclePhaseTypeListItem> LifecyclePhaseTypeList { get; set; }
        public DbSet<LocationListItem> LocationList { get; set; }
        public DbSet<MandateRoleTypeListItem> MandateRoleTypeList { get; set; }
        public DbSet<SeatTypeListItem> SeatTypeList { get; set; }
        public DbSet<LocationTypeListItem> LocationTypeList { get; set; }

        public DbSet<OrganisationDetailItem> OrganisationDetail { get; set; }
        public DbSet<OrganisationListItem> OrganisationList { get; set; }
        public DbSet<OrganisationClassificationValidity> OrganisationClassificationValidities { get; set; }
        public DbSet<OrganisationBodyListItem> OrganisationBodyList { get; set; }
        public DbSet<OrganisationBuildingListItem> OrganisationBuildingList { get; set; }
        public DbSet<OrganisationCapacityListItem> OrganisationCapacityList { get; set; }
        public DbSet<OrganisationContactListItem> OrganisationContactList { get; set; }
        public DbSet<OrganisationFunctionListItem> OrganisationFunctionList { get; set; }
        public DbSet<OrganisationRelationListItem> OrganisationRelationList { get; set; }
        public DbSet<OrganisationLabelListItem> OrganisationLabelList { get; set; }
        public DbSet<OrganisationChildListItem> OrganisationChildrenList { get; set; }
        public DbSet<OrganisationKeyListItem> OrganisationKeyList { get; set; }
        public DbSet<OrganisationLocationListItem> OrganisationLocationList { get; set; }
        public DbSet<OrganisationOrganisationClassificationListItem> OrganisationOrganisationClassificationList { get; set; }
        public DbSet<OrganisationParentListItem> OrganisationParentList { get; set; }
        public DbSet<OrganisationFormalFrameworkListItem> OrganisationFormalFrameworkList { get; set; }
        public DbSet<OrganisationBankAccountListItem> OrganisationBankAccountList { get; set; }
        public DbSet<OrganisationOpeningHourListItem> OrganisationOpeningHourList { get; set; }

        public DbSet<ActiveOrganisationFormalFrameworkListItem> ActiveOrganisationFormalFrameworkList { get; set; }
        public DbSet<FutureActiveOrganisationFormalFrameworkListItem> FutureActiveOrganisationFormalFrameworkList { get; set; }

        public DbSet<ActiveOrganisationParentListItem> ActiveOrganisationParentList { get; set; }
        public DbSet<FutureActiveOrganisationParentListItem> FutureActiveOrganisationParentList { get; set; }

        public DbSet<OrganisationClassificationListItem> OrganisationClassificationList { get; set; }
        public DbSet<OrganisationClassificationTypeListItem> OrganisationClassificationTypeList { get; set; }
        public DbSet<OrganisationRelationTypeListItem> OrganisationRelationTypeList { get; set; }

        public DbSet<OrganisationTerminationListItem> OrganisationTerminationList { get; set; }

        public DbSet<PersonListItem> PersonList { get; set; }
        public DbSet<PersonCapacityListItem> PersonCapacityList { get; set; }
        public DbSet<PersonFunctionListItem> PersonFunctionList { get; set; }
        public DbSet<PersonMandateListItem> PersonMandateList { get; set; }

        public DbSet<PurposeListItem> PurposeList { get; set; }
        public DbSet<OrganisationTreeItem> OrganisationTreeList { get; set; }

        public DbSet<ProjectionStateItem> ProjectionStates { get; set; }

        // ElasticSearch
        public DbSet<ShowOnVlaamseOverheidSitesPerOrganisation> ShowOnVlaamseOverheidSitesPerOrganisationList { get; set; }
        public DbSet<IsActivePerOrganisationCapacity> IsActivePerOrganisationCapacityList { get; set; }
        public DbSet<Delegations.OrganisationPerBody> OrganisationPerBodyList { get; set; }
        public DbSet<ElasticSearchProjections.OrganisationPerBody> OrganisationPerBodyListForES { get; set; }
        public DbSet<OrganisationToRebuild> OrganisationsToRebuild { get; set; }

        public DbSet<OrganisationCacheItem> OrganisationCache { get; set; }
        public DbSet<BodySeatCacheItem> BodySeatCache { get; set; }
        public DbSet<BodyCacheItem> BodyCache { get; set; }


        // Reporting
        public DbSet<BodySeatGenderRatioOrganisationPerBodyListItem> BodySeatGenderRatioOrganisationPerBodyList { get; set; }
        public DbSet<BodySeatGenderRatioPersonListItem> BodySeatGenderRatioPersonList { get; set; }
        public DbSet<BodySeatGenderRatioOrganisationListItem> BodySeatGenderRatioOrganisationList { get; set; }

        public DbSet<BodySeatGenderRatioBodyItem> BodySeatGenderRatioBodyList { get; set; }
        public DbSet<BodySeatGenderRatioPostsPerTypeItem> BodySeatGenderRatioPostsPerTypeList { get; set; }
        public DbSet<BodySeatGenderRatioBodyMandateItem> BodySeatGenderRatioBodyMandateList { get; set; }
        public DbSet<BodySeatGenderRatioOrganisationClassificationItem> BodySeatGenderRatioOrganisationClassificationList { get; set; }

        //Kbo Sync Queue
        public DbSet<KboSyncQueueItem> KboSyncQueue { get; set; }

        // This needs to be DbContextOptions<T> for Autofac!
        public OrganisationRegistryContext(DbContextOptions<OrganisationRegistryContext> options)
            : base(options)
        { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.ConfigureWarnings(
            //    x => x.Throw(RelationalEventId.QueryClientEvaluationWarning));

            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(
                    @"Server=localhost,21433;Database=OrganisationRegistry;User ID=sa;Password=E@syP@ssw0rd;",
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory", "OrganisationRegistry"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AddEntityConfigurationsFromAssembly(GetType().GetTypeInfo().Assembly);
        }
    }

    public class TemporaryDbContextFactory : IDesignTimeDbContextFactory<OrganisationRegistryContext>
    {
        //public OrganisationRegistryContext Create(DbContextFactoryOptions options)
        //{
        //    var builder = new DbContextOptionsBuilder<OrganisationRegistryContext>();

        //    builder.UseSqlServer(
        //        @"Server=.;Database=wegwijstemp;Trusted_Connection=True;",
        //        x => x.MigrationsHistoryTable("__EFMigrationsHistory", "OrganisationRegistry"));

        //    return new OrganisationRegistryContext(builder.Options);
        //}

        public OrganisationRegistryContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<OrganisationRegistryContext>();

            builder.UseSqlServer(
                @"Server=localhost,21433;Database=OrganisationRegistry;User ID=sa;Password=E@syP@ssw0rd;",
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", "OrganisationRegistry"));

            return new OrganisationRegistryContext(builder.Options);
        }
    }
}
