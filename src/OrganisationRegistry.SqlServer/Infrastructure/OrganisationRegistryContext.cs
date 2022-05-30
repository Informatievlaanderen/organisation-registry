namespace OrganisationRegistry.SqlServer.Infrastructure;

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
using Import.Organisations;
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
using OrganisationRegistry.Infrastructure;
using OrganisationRelationType;
using Person;
using ProjectionState;
using Purpose;
using RegulationSubTheme;
using RegulationTheme;
using Reporting;
using SeatType;
using Security;

public class OrganisationRegistryContext : DbContext
{
    public DbSet<EventListItem> Events { get; set; } = null!;
    public DbSet<ConfigurationListItem> Configuration { get; set; } = null!;

    public DbSet<Log> Logs { get; set; } = null!;

    public DbSet<MagdaCallReference> MagdaCallReferences { get; set; } = null!;

    public DbSet<BodyDetail> BodyDetail { get; set; } = null!;
    public DbSet<BodyContactListItem> BodyContactList { get; set; } = null!;
    public DbSet<BodyFormalFrameworkListItem> BodyFormalFrameworkList { get; set; } = null!;
    public DbSet<BodyLifecyclePhaseListItem> BodyLifecyclePhaseList { get; set; } = null!;
    public DbSet<BodyLifecyclePhaseValidity> BodyLifecyclePhaseValidities { get; set; } = null!;
    public DbSet<BodyListItem> BodyList { get; set; } = null!;
    public DbSet<BodyMandateListItem> BodyMandateList { get; set; } = null!;
    public DbSet<BodySeatCacheItemForBodyMandateList> BodySeatCacheForBodyMandateList { get; set; } = null!;
    public DbSet<BodyOrganisationListItem> BodyOrganisationList { get; set; } = null!;
    public DbSet<BodySeatListItem> BodySeatList { get; set; } = null!;
    public DbSet<BodyBodyClassificationListItem> BodyBodyClassificationList { get; set; } = null!;

    public DbSet<BodyClassificationListItem> BodyClassificationList { get; set; } = null!;
    public DbSet<BodyClassificationTypeListItem> BodyClassificationTypeList { get; set; } = null!;

    public DbSet<DelegationListItem> DelegationList { get; set; } = null!;
    public DbSet<DelegationAssignmentListItem> DelegationAssignmentList { get; set; } = null!;
    public DbSet<Delegations.OrganisationPerBody> OrganisationPerBodyList { get; set; } = null!;

    public DbSet<ActiveBodyOrganisationListItem> ActiveBodyOrganisationList { get; set; } = null!;
    public DbSet<FutureActiveBodyOrganisationListItem> FutureActiveBodyOrganisationList { get; set; } = null!;

    public DbSet<ActivePeopleAssignedToBodyMandateListItem> ActivePeopleAssignedToBodyMandatesList { get; set; } = null!;
    public DbSet<FuturePeopleAssignedToBodyMandatesListItem> FuturePeopleAssignedToBodyMandatesList { get; set; } = null!;

    public DbSet<BuildingListItem> BuildingList { get; set; } = null!;
    public DbSet<CapacityListItem> CapacityList { get; set; } = null!;
    public DbSet<ContactTypeListItem> ContactTypeList { get; set; } = null!;
    public DbSet<FormalFrameworkListItem> FormalFrameworkList { get; set; } = null!;
    public DbSet<FormalFrameworkCategoryListItem> FormalFrameworkCategoryList { get; set; } = null!;
    public DbSet<FunctionTypeListItem> FunctionTypeList { get; set; } = null!;
    public DbSet<KeyTypeListItem> KeyTypeList { get; set; } = null!;
    public DbSet<LabelTypeListItem> LabelTypeList { get; set; } = null!;
    public DbSet<LifecyclePhaseTypeListItem> LifecyclePhaseTypeList { get; set; } = null!;
    public DbSet<LocationListItem> LocationList { get; set; } = null!;
    public DbSet<MandateRoleTypeListItem> MandateRoleTypeList { get; set; } = null!;
    public DbSet<SeatTypeListItem> SeatTypeList { get; set; } = null!;
    public DbSet<LocationTypeListItem> LocationTypeList { get; set; } = null!;
    public DbSet<RegulationThemeListItem> RegulationThemeList { get; set; } = null!;
    public DbSet<RegulationSubThemeListItem> RegulationSubThemeList { get; set; } = null!;

    public DbSet<OrganisationDetailItem> OrganisationDetail { get; set; } = null!;
    public DbSet<OrganisationListItem> OrganisationList { get; set; } = null!;
    public DbSet<OrganisationClassificationValidity> OrganisationClassificationValidities { get; set; } = null!;
    public DbSet<OrganisationBodyListItem> OrganisationBodyList { get; set; } = null!;
    public DbSet<OrganisationBuildingListItem> OrganisationBuildingList { get; set; } = null!;
    public DbSet<OrganisationCapacityListItem> OrganisationCapacityList { get; set; } = null!;
    public DbSet<OrganisationContactListItem> OrganisationContactList { get; set; } = null!;
    public DbSet<OrganisationFunctionListItem> OrganisationFunctionList { get; set; } = null!;
    public DbSet<OrganisationRelationListItem> OrganisationRelationList { get; set; } = null!;
    public DbSet<OrganisationLabelListItem> OrganisationLabelList { get; set; } = null!;
    public DbSet<OrganisationChildListItem> OrganisationChildrenList { get; set; } = null!;
    public DbSet<OrganisationKeyListItem> OrganisationKeyList { get; set; } = null!;
    public DbSet<OrganisationLocationListItem> OrganisationLocationList { get; set; } = null!;
    public DbSet<OrganisationOrganisationClassificationListItem> OrganisationOrganisationClassificationList { get; set; } = null!;
    public DbSet<OrganisationParentListItem> OrganisationParentList { get; set; } = null!;
    public DbSet<OrganisationRegulationListItem> OrganisationRegulationList { get; set; } = null!;

    public DbSet<OrganisationFormalFrameworkListItem> OrganisationFormalFrameworkList { get; set; } = null!;
    public DbSet<OrganisationBankAccountListItem> OrganisationBankAccountList { get; set; } = null!;
    public DbSet<OrganisationOpeningHourListItem> OrganisationOpeningHourList { get; set; } = null!;

    public DbSet<ActiveOrganisationFormalFrameworkListItem> ActiveOrganisationFormalFrameworkList { get; set; } = null!;
    public DbSet<FutureActiveOrganisationFormalFrameworkListItem> FutureActiveOrganisationFormalFrameworkList { get; set; } = null!;

    public DbSet<ActiveOrganisationParentListItem> ActiveOrganisationParentList { get; set; } = null!;
    public DbSet<FutureActiveOrganisationParentListItem> FutureActiveOrganisationParentList { get; set; } = null!;

    public DbSet<OrganisationClassificationListItem> OrganisationClassificationList { get; set; } = null!;
    public DbSet<OrganisationClassificationTypeListItem> OrganisationClassificationTypeList { get; set; } = null!;
    public DbSet<OrganisationRelationTypeListItem> OrganisationRelationTypeList { get; set; } = null!;

    public DbSet<OrganisationTerminationListItem> OrganisationTerminationList { get; set; } = null!;

    public DbSet<PersonListItem> PersonList { get; set; } = null!;
    public DbSet<PersonCapacityListItem> PersonCapacityList { get; set; } = null!;
    public DbSet<PersonFunctionListItem> PersonFunctionList { get; set; } = null!;
    public DbSet<PersonMandateListItem> PersonMandateList { get; set; } = null!;

    public DbSet<PurposeListItem> PurposeList { get; set; } = null!;
    public DbSet<OrganisationTreeItem> OrganisationTreeList { get; set; } = null!;

    public DbSet<ProjectionStateItem> ProjectionStates { get; set; } = null!;

    // ElasticSearch
    public DbSet<ShowOnVlaamseOverheidSitesPerOrganisation> ShowOnVlaamseOverheidSitesPerOrganisationList { get; set; } = null!;
    public DbSet<IsActivePerOrganisationCapacity> IsActivePerOrganisationCapacityList { get; set; } = null!;
    public DbSet<ElasticSearchProjections.OrganisationPerBody> OrganisationPerBodyListForES { get; set; } = null!;
    public DbSet<OrganisationToRebuild> OrganisationsToRebuild { get; set; } = null!;

    public DbSet<OrganisationCacheItem> OrganisationCache { get; set; } = null!;
    public DbSet<ContactTypeCacheItem> ContactTypeCache { get; set; } = null!;
    public DbSet<BodySeatCacheItem> BodySeatCache { get; set; } = null!;
    public DbSet<BodyCacheItem> BodyCache { get; set; } = null!;


    // Reporting
    public DbSet<BodySeatGenderRatioOrganisationPerBodyListItem> BodySeatGenderRatioOrganisationPerBodyList { get; set; } = null!;
    public DbSet<BodySeatGenderRatioPersonListItem> BodySeatGenderRatioPersonList { get; set; } = null!;
    public DbSet<BodySeatGenderRatioOrganisationListItem> BodySeatGenderRatioOrganisationList { get; set; } = null!;

    public DbSet<BodySeatGenderRatioBodyItem> BodySeatGenderRatioBodyList { get; set; } = null!;
    public DbSet<BodySeatGenderRatioPostsPerTypeItem> BodySeatGenderRatioPostsPerTypeList { get; set; } = null!;
    public DbSet<BodySeatGenderRatioBodyMandateItem> BodySeatGenderRatioBodyMandateList { get; set; } = null!;
    public DbSet<BodySeatGenderRatioOrganisationClassificationItem> BodySeatGenderRatioOrganisationClassificationList { get; set; } = null!;

    //Kbo Sync Queue
    public DbSet<KboSyncQueueItem> KboSyncQueue { get; set; } = null!;

    // Import

    public DbSet<ImportOrganisationsStatusListItem> ImportOrganisationsStatusList { get; set; } = null!;

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
                @"Server=localhost,21433;Database=OrganisationRegistry;User ID=sa;Password=E@syP@ssw0rd;TrustServerCertificate=True;",
                x => x.MigrationsHistoryTable("__EFMigrationsHistory", WellknownSchemas.BackofficeSchema));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.AddEntityConfigurationsFromAssembly(GetType().GetTypeInfo().Assembly);
    }
}

public class TemporaryDbContextFactory : IDesignTimeDbContextFactory<OrganisationRegistryContext>
{
    public OrganisationRegistryContext CreateDbContext(string[] args)
    {
        var builder = new DbContextOptionsBuilder<OrganisationRegistryContext>();

        builder.UseSqlServer(
            @"Server=localhost,21433;Database=OrganisationRegistry;User ID=sa;Password=E@syP@ssw0rd;TrustServerCertificate=True;",
            x => x.MigrationsHistoryTable("__EFMigrationsHistory", WellknownSchemas.BackofficeSchema));

        return new OrganisationRegistryContext(builder.Options);
    }
}