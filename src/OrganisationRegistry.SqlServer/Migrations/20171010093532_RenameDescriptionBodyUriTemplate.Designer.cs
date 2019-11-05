using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using OrganisationRegistry.SqlServer.Infrastructure;
using OrganisationRegistry.Body;

namespace OrganisationRegistry.SqlServer.Migrations
{
    [DbContext(typeof(OrganisationRegistryContext))]
    [Migration("20171010093532_RenameDescriptionBodyUriTemplate")]
    partial class RenameDescriptionBodyUriTemplate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.1.2")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Body.BodyDetail", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BodyNumber")
                        .HasMaxLength(10);

                    b.Property<string>("Description");

                    b.Property<DateTime?>("FormalValidFrom");

                    b.Property<DateTime?>("FormalValidTo");

                    b.Property<bool>("IsLifecycleValid");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<string>("Organisation")
                        .HasMaxLength(500);

                    b.Property<Guid?>("OrganisationId");

                    b.Property<string>("ShortName");

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("FormalValidFrom");

                    b.HasIndex("FormalValidTo");

                    b.HasIndex("Name")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("Organisation");

                    b.HasIndex("ShortName");

                    b.ToTable("BodyDetail","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Body.BodyFormalFrameworkListItem", b =>
                {
                    b.Property<Guid>("BodyFormalFrameworkId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BodyId");

                    b.Property<Guid>("FormalFrameworkId");

                    b.Property<string>("FormalFrameworkName")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("BodyFormalFrameworkId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("FormalFrameworkName")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("ValidFrom");

                    b.HasIndex("ValidTo");

                    b.ToTable("BodyFormalFrameworkList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Body.BodyLifecyclePhaseListItem", b =>
                {
                    b.Property<Guid>("BodyLifecyclePhaseId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BodyId");

                    b.Property<bool>("HasAdjacentGaps");

                    b.Property<Guid>("LifecyclePhaseTypeId");

                    b.Property<string>("LifecyclePhaseTypeName")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("BodyLifecyclePhaseId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("LifecyclePhaseTypeName")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("ValidFrom");

                    b.HasIndex("ValidTo");

                    b.ToTable("BodyLifecyclePhaseList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Body.BodyLifecyclePhaseValidity", b =>
                {
                    b.Property<Guid>("BodyLifecyclePhaseId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BodyId");

                    b.Property<Guid?>("BodyListItemId");

                    b.Property<bool>("RepresentsActivePhase");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("BodyLifecyclePhaseId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("BodyId")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("BodyListItemId");

                    b.HasIndex("RepresentsActivePhase");

                    b.HasIndex("ValidFrom");

                    b.HasIndex("ValidTo");

                    b.ToTable("BodyLifecyclePhaseValidity","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Body.BodyListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("BodyNumber")
                        .HasMaxLength(10);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<string>("Organisation")
                        .HasMaxLength(500);

                    b.Property<Guid?>("OrganisationId");

                    b.Property<string>("ShortName");

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("BodyNumber");

                    b.HasIndex("Name")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("Organisation");

                    b.HasIndex("ShortName");

                    b.ToTable("BodyList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Body.BodyMandateListItem", b =>
                {
                    b.Property<Guid>("BodyMandateId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("AssignedToId");

                    b.Property<string>("AssignedToName")
                        .HasMaxLength(401);

                    b.Property<Guid>("BodyId");

                    b.Property<int>("BodyMandateType");

                    b.Property<Guid>("BodySeatId");

                    b.Property<string>("BodySeatName")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<string>("BodySeatNumber")
                        .HasMaxLength(10);

                    b.Property<string>("ContactsJson");

                    b.Property<Guid?>("DelegatedId");

                    b.Property<string>("DelegatedName")
                        .HasMaxLength(500);

                    b.Property<Guid>("DelegatorId");

                    b.Property<string>("DelegatorName")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("BodyMandateId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("BodySeatName");

                    b.HasIndex("DelegatorName")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("ValidFrom");

                    b.HasIndex("ValidTo");

                    b.ToTable("BodyMandateList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Body.BodyOrganisationListItem", b =>
                {
                    b.Property<Guid>("BodyOrganisationId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BodyId");

                    b.Property<Guid>("OrganisationId");

                    b.Property<string>("OrganisationName")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("BodyOrganisationId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("OrganisationName")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("ValidFrom");

                    b.HasIndex("ValidTo");

                    b.ToTable("BodyOrganisationList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Body.BodySeatListItem", b =>
                {
                    b.Property<Guid>("BodySeatId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BodyId");

                    b.Property<string>("BodySeatNumber")
                        .HasMaxLength(10);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<bool>("PaidSeat");

                    b.Property<Guid>("SeatTypeId");

                    b.Property<string>("SeatTypeName")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("BodySeatId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Name")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("ValidFrom");

                    b.HasIndex("ValidTo");

                    b.ToTable("BodySeatList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Body.ScheduledActions.Organisation.ActiveBodyOrganisationListItem", b =>
                {
                    b.Property<Guid>("BodyOrganisationId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BodyId");

                    b.Property<Guid>("OrganisationId");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("BodyOrganisationId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("ValidTo");

                    b.ToTable("ActiveBodyOrganisationList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Body.ScheduledActions.Organisation.FutureActiveBodyOrganisationListItem", b =>
                {
                    b.Property<Guid>("BodyOrganisationId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BodyId");

                    b.Property<Guid>("OrganisationId");

                    b.Property<DateTime?>("ValidFrom");

                    b.HasKey("BodyOrganisationId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("ValidFrom");

                    b.ToTable("FutureActiveBodyOrganisationList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Body.ScheduledActions.PeopleAssignedToBodyMandates.ActivePeopleAssignedToBodyMandateListItem", b =>
                {
                    b.Property<Guid>("DelegationAssignmentId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BodyId");

                    b.Property<Guid>("BodyMandateId");

                    b.Property<Guid>("BodySeatId");

                    b.Property<string>("PersonFullName")
                        .IsRequired();

                    b.Property<Guid>("PersonId");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("DelegationAssignmentId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("ValidTo");

                    b.ToTable("ActivePeopleAssignedToBodyMandatesList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Body.ScheduledActions.PeopleAssignedToBodyMandates.FuturePeopleAssignedToBodyMandatesListItem", b =>
                {
                    b.Property<Guid>("DelegationAssignmentId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BodyId");

                    b.Property<Guid>("BodyMandateId");

                    b.Property<Guid>("BodySeatId");

                    b.Property<string>("PersonFullName")
                        .IsRequired();

                    b.Property<Guid>("PersonId");

                    b.Property<DateTime?>("ValidFrom");

                    b.HasKey("DelegationAssignmentId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("ValidFrom");

                    b.ToTable("FuturePeopleAssignedToBodyMandatesList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Building.BuildingListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<int?>("VimId");

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("VimId");

                    b.ToTable("BuildingList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Capacity.CapacityListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("CapacityList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Configuration.ConfigurationListItem", b =>
                {
                    b.Property<string>("Key")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(450);

                    b.Property<string>("Description");

                    b.Property<string>("Value");

                    b.HasKey("Key")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("Configuration","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.ContactType.ContactTypeListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("ContactTypeList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.DelegationAssignments.DelegationAssignmentListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BodyId");

                    b.Property<Guid>("BodyMandateId");

                    b.Property<Guid>("BodySeatId");

                    b.Property<string>("ContactsJson");

                    b.Property<Guid>("PersonId");

                    b.Property<string>("PersonName")
                        .HasMaxLength(200);

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("BodyMandateId");

                    b.HasIndex("PersonName");

                    b.HasIndex("ValidFrom");

                    b.HasIndex("ValidTo");

                    b.HasIndex("BodyMandateId", "PersonName")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("DelegationAssignmentList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Delegations.DelegationListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BodyId");

                    b.Property<string>("BodyName")
                        .HasMaxLength(500);

                    b.Property<Guid?>("BodyOrganisationId");

                    b.Property<string>("BodyOrganisationName")
                        .HasMaxLength(500);

                    b.Property<Guid>("BodySeatId");

                    b.Property<string>("BodySeatName")
                        .HasMaxLength(500);

                    b.Property<string>("BodySeatNumber")
                        .HasMaxLength(10);

                    b.Property<Guid?>("FunctionTypeId");

                    b.Property<string>("FunctionTypeName")
                        .HasMaxLength(500);

                    b.Property<bool>("IsDelegated");

                    b.Property<int>("NumberOfDelegationAssignments");

                    b.Property<Guid>("OrganisationId");

                    b.Property<string>("OrganisationName")
                        .HasMaxLength(500);

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("BodyName");

                    b.HasIndex("BodySeatName");

                    b.HasIndex("BodySeatNumber");

                    b.HasIndex("IsDelegated");

                    b.HasIndex("OrganisationName")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("ValidFrom");

                    b.HasIndex("ValidTo");

                    b.ToTable("DelegationList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Delegations.OrganisationPerBody", b =>
                {
                    b.Property<Guid>("BodyId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BodyOrganisationId");

                    b.Property<Guid>("OrganisationId");

                    b.Property<string>("OrganisationName")
                        .IsRequired();

                    b.HasKey("BodyId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.ToTable("OrganisationPerBodyList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.ElasticSearchProjections.IsActivePerOrganisationCapacity", b =>
                {
                    b.Property<Guid>("OrganisationCapacityId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsActive");

                    b.HasKey("OrganisationCapacityId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.ToTable("IsActivePerOrganisationCapacityList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.ElasticSearchProjections.OrganisationPerBody", b =>
                {
                    b.Property<Guid>("BodyId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("OrganisationId");

                    b.Property<string>("OrganisationName")
                        .IsRequired();

                    b.HasKey("BodyId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.ToTable("OrganisationPerBodyListForES","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.ElasticSearchProjections.ShowOnVlaamseOverheidSitesPerOrganisation", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("ShowOnVlaamseOverheidSites");

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.ToTable("ShowOnVlaamseOverheidSitesPerOrganisationList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Event.EventListItem", b =>
                {
                    b.Property<Guid>("Id");

                    b.Property<int>("Version");

                    b.Property<string>("Data")
                        .IsRequired();

                    b.Property<string>("FirstName")
                        .HasMaxLength(200);

                    b.Property<string>("Ip")
                        .HasMaxLength(100);

                    b.Property<string>("LastName")
                        .HasMaxLength(200);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<int>("Number")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Timestamp");

                    b.Property<string>("UserId")
                        .HasMaxLength(100);

                    b.HasKey("Id", "Version")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("Name");

                    b.HasIndex("Number");

                    b.ToTable("Events","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.FormalFramework.FormalFrameworkListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.Property<Guid>("FormalFrameworkCategoryId");

                    b.Property<string>("FormalFrameworkCategoryName")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Code");

                    b.HasIndex("FormalFrameworkCategoryName");

                    b.HasIndex("Name")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("Name", "FormalFrameworkCategoryId")
                        .IsUnique();

                    b.ToTable("FormalFrameworkList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.FormalFrameworkCategory.FormalFrameworkCategoryListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("FormalFrameworkCategoryList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.FunctionType.FunctionTypeListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("FunctionList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.KeyType.KeyTypeListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("KeyTypeList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.LabelType.LabelTypeListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("LabelTypeList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.LifecyclePhaseType.LifecyclePhaseTypeListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsDefaultPhase");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<bool>("RepresentsActivePhase");

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("RepresentsActivePhase", "IsDefaultPhase");

                    b.ToTable("LifecyclePhaseTypeList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Location.LocationListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("City")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasMaxLength(100);

                    b.Property<string>("CrabLocationId");

                    b.Property<string>("FormattedAddress")
                        .HasMaxLength(460);

                    b.Property<bool>("HasCrabLocation");

                    b.Property<string>("Street")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("ZipCode")
                        .IsRequired()
                        .HasMaxLength(50);

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("City");

                    b.HasIndex("Country");

                    b.HasIndex("FormattedAddress")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("HasCrabLocation");

                    b.HasIndex("Street");

                    b.HasIndex("ZipCode");

                    b.ToTable("LocationList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Log.Log", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Application");

                    b.Property<string>("Callsite");

                    b.Property<string>("Exception");

                    b.Property<string>("Level")
                        .HasMaxLength(50);

                    b.Property<DateTime>("Logged");

                    b.Property<string>("Logger")
                        .HasMaxLength(250);

                    b.Property<string>("Message");

                    b.HasKey("Id");

                    b.ToTable("Logs","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.MandateRoleType.MandateRoleTypeListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("MandateRoleTypeList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationBodyListItem", b =>
                {
                    b.Property<Guid>("OrganisationBodyId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BodyId");

                    b.Property<string>("BodyName")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<Guid>("OrganisationId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("OrganisationBodyId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("BodyName")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("ValidFrom");

                    b.HasIndex("ValidTo");

                    b.ToTable("OrganisationBodyList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationBuildingListItem", b =>
                {
                    b.Property<Guid>("OrganisationBuildingId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BuildingId");

                    b.Property<string>("BuildingName")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<bool>("IsMainBuilding");

                    b.Property<Guid>("OrganisationId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("OrganisationBuildingId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("BuildingName")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("IsMainBuilding");

                    b.HasIndex("ValidFrom");

                    b.HasIndex("ValidTo");

                    b.ToTable("OrganisationBuildingList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationCapacityListItem", b =>
                {
                    b.Property<Guid>("OrganisationCapacityId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CapacityId");

                    b.Property<string>("CapacityName")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<string>("ContactsJson");

                    b.Property<Guid?>("FunctionId");

                    b.Property<string>("FunctionName")
                        .HasMaxLength(500);

                    b.Property<Guid>("OrganisationId");

                    b.Property<Guid?>("PersonId");

                    b.Property<string>("PersonName")
                        .HasMaxLength(401);

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("OrganisationCapacityId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("CapacityName")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("FunctionName");

                    b.HasIndex("PersonName");

                    b.HasIndex("ValidFrom");

                    b.HasIndex("ValidTo");

                    b.ToTable("OrganisationCapacityList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationChildListItem", b =>
                {
                    b.Property<Guid>("OrganisationOrganisationParentId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("Id");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<DateTime?>("OrganisationValidFrom");

                    b.Property<DateTime?>("OrganisationValidTo");

                    b.Property<string>("OvoNumber")
                        .IsRequired();

                    b.Property<Guid>("ParentOrganisationId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("OrganisationOrganisationParentId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Name")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("OrganisationValidFrom");

                    b.HasIndex("OrganisationValidTo");

                    b.HasIndex("OvoNumber");

                    b.HasIndex("ValidFrom");

                    b.HasIndex("ValidTo");

                    b.ToTable("OrganisationChildList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationClassificationValidity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid>("OrganisationClassificationId");

                    b.Property<Guid>("OrganisationClassificationTypeId");

                    b.Property<int?>("OrganisationListItemId");

                    b.Property<Guid>("OrganisationOrganisationClassificationId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("OrganisationClassificationTypeId");

                    b.HasIndex("OrganisationListItemId");

                    b.HasIndex("ValidFrom");

                    b.HasIndex("ValidTo");

                    b.ToTable("OrganisationClassificationValidity","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationContactListItem", b =>
                {
                    b.Property<Guid>("OrganisationContactId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ContactTypeId");

                    b.Property<string>("ContactTypeName")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<string>("ContactValue")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<Guid>("OrganisationId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("OrganisationContactId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("ContactTypeName")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("ContactValue");

                    b.HasIndex("ValidFrom");

                    b.HasIndex("ValidTo");

                    b.ToTable("OrganisationContactList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationDetailItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Description");

                    b.Property<Guid?>("FormalFrameworkId");

                    b.Property<Guid?>("MainBuildingId");

                    b.Property<string>("MainBuildingName");

                    b.Property<Guid?>("MainLocationId");

                    b.Property<string>("MainLocationName");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<Guid?>("OrganisationClassificationId");

                    b.Property<Guid?>("OrganisationClassificationTypeId");

                    b.Property<string>("OvoNumber")
                        .IsRequired()
                        .HasMaxLength(10);

                    b.Property<string>("ParentOrganisation");

                    b.Property<Guid?>("ParentOrganisationId");

                    b.Property<Guid?>("ParentOrganisationOrganisationParentId");

                    b.Property<string>("PurposeIds");

                    b.Property<string>("PurposeNames");

                    b.Property<string>("ShortName");

                    b.Property<bool>("ShowOnVlaamseOverheidSites");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Name")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("OvoNumber")
                        .IsUnique();

                    b.HasIndex("ParentOrganisation");

                    b.ToTable("OrganisationDetail","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationFormalFrameworkListItem", b =>
                {
                    b.Property<Guid>("OrganisationFormalFrameworkId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("FormalFrameworkId");

                    b.Property<string>("FormalFrameworkName")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<Guid>("OrganisationId");

                    b.Property<Guid>("ParentOrganisationId");

                    b.Property<string>("ParentOrganisationName")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("OrganisationFormalFrameworkId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("FormalFrameworkName");

                    b.HasIndex("ParentOrganisationName")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("ValidFrom");

                    b.HasIndex("ValidTo");

                    b.ToTable("OrganisationFormalFrameworkList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationFormalFrameworkValidity", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid>("OrganisationFormalFrameworkId");

                    b.Property<int?>("OrganisationListItemId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("OrganisationListItemId");

                    b.HasIndex("ValidFrom");

                    b.HasIndex("ValidTo");

                    b.ToTable("OrganisationFormalFrameworkValidity","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationFunctionListItem", b =>
                {
                    b.Property<Guid>("OrganisationFunctionId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ContactsJson");

                    b.Property<Guid>("FunctionId");

                    b.Property<string>("FunctionName")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<Guid>("OrganisationId");

                    b.Property<Guid>("PersonId");

                    b.Property<string>("PersonName")
                        .IsRequired()
                        .HasMaxLength(401);

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("OrganisationFunctionId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("FunctionName");

                    b.HasIndex("PersonName")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("ValidFrom");

                    b.HasIndex("ValidTo");

                    b.ToTable("OrganisationFunctionList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationKeyListItem", b =>
                {
                    b.Property<Guid>("OrganisationKeyId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("KeyTypeId");

                    b.Property<string>("KeyTypeName")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<string>("KeyValue")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<Guid>("OrganisationId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("OrganisationKeyId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("KeyTypeName")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("KeyValue");

                    b.HasIndex("ValidFrom");

                    b.HasIndex("ValidTo");

                    b.ToTable("OrganisationKeyList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationLabelListItem", b =>
                {
                    b.Property<Guid>("OrganisationLabelId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("LabelTypeId");

                    b.Property<string>("LabelTypeName")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<string>("LabelValue")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<Guid>("OrganisationId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("OrganisationLabelId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("LabelTypeName")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("LabelValue");

                    b.HasIndex("ValidFrom");

                    b.HasIndex("ValidTo");

                    b.ToTable("OrganisationLabelList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationListItem", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<Guid?>("FormalFrameworkId");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<Guid>("OrganisationId");

                    b.Property<string>("OvoNumber")
                        .IsRequired()
                        .HasMaxLength(10);

                    b.Property<string>("ParentOrganisation")
                        .HasMaxLength(500);

                    b.Property<Guid?>("ParentOrganisationId");

                    b.Property<Guid?>("ParentOrganisationsRelationshipId");

                    b.Property<string>("ShortName");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("FormalFrameworkId");

                    b.HasIndex("Name")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("OvoNumber");

                    b.HasIndex("ParentOrganisation");

                    b.HasIndex("ShortName");

                    b.HasIndex("ValidFrom");

                    b.HasIndex("ValidTo");

                    b.HasIndex("OrganisationId", "FormalFrameworkId")
                        .IsUnique();

                    b.ToTable("OrganisationList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationLocationListItem", b =>
                {
                    b.Property<Guid>("OrganisationLocationId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsMainLocation");

                    b.Property<Guid>("LocationId");

                    b.Property<string>("LocationName")
                        .IsRequired()
                        .HasMaxLength(460);

                    b.Property<Guid>("OrganisationId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("OrganisationLocationId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("IsMainLocation");

                    b.HasIndex("LocationName")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("ValidFrom");

                    b.HasIndex("ValidTo");

                    b.ToTable("OrganisationLocationList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationOrganisationClassificationListItem", b =>
                {
                    b.Property<Guid>("OrganisationOrganisationClassificationId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("OrganisationClassificationId");

                    b.Property<string>("OrganisationClassificationName")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<Guid>("OrganisationClassificationTypeId");

                    b.Property<string>("OrganisationClassificationTypeName")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<Guid>("OrganisationId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("OrganisationOrganisationClassificationId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("OrganisationClassificationName");

                    b.HasIndex("OrganisationClassificationTypeName")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("ValidFrom");

                    b.HasIndex("ValidTo");

                    b.ToTable("OrganisationOrganisationClassificationList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationParentListItem", b =>
                {
                    b.Property<Guid>("OrganisationOrganisationParentId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("OrganisationId");

                    b.Property<Guid>("ParentOrganisationId");

                    b.Property<string>("ParentOrganisationName")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("OrganisationOrganisationParentId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("ParentOrganisationName")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("ValidFrom");

                    b.HasIndex("ValidTo");

                    b.ToTable("OrganisationParentList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.ScheduledActions.Building.ActiveOrganisationBuildingListItem", b =>
                {
                    b.Property<Guid>("OrganisationBuildingId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BuildingId");

                    b.Property<Guid>("OrganisationId");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("OrganisationBuildingId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("ValidTo");

                    b.ToTable("ActiveOrganisationBuildingList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.ScheduledActions.Building.FutureActiveOrganisationBuildingListItem", b =>
                {
                    b.Property<Guid>("OrganisationBuildingId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BuildingId");

                    b.Property<Guid>("OrganisationId");

                    b.Property<DateTime?>("ValidFrom");

                    b.HasKey("OrganisationBuildingId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("ValidFrom");

                    b.ToTable("FutureActiveOrganisationBuildingList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.ScheduledActions.FormalFramework.ActiveOrganisationFormalFrameworkListItem", b =>
                {
                    b.Property<Guid>("OrganisationFormalFrameworkId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("FormalFrameworkId");

                    b.Property<Guid>("OrganisationId");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("OrganisationFormalFrameworkId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("ValidTo");

                    b.ToTable("ActiveOrganisationFormalFrameworkList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.ScheduledActions.FormalFramework.FutureActiveOrganisationFormalFrameworkListItem", b =>
                {
                    b.Property<Guid>("OrganisationFormalFrameworkId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("FormalFrameworkId");

                    b.Property<Guid>("OrganisationId");

                    b.Property<DateTime?>("ValidFrom");

                    b.HasKey("OrganisationFormalFrameworkId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("ValidFrom");

                    b.ToTable("FutureActiveOrganisationFormalFrameworkList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.ScheduledActions.Location.ActiveOrganisationLocationListItem", b =>
                {
                    b.Property<Guid>("OrganisationLocationId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("LocationId");

                    b.Property<Guid>("OrganisationId");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("OrganisationLocationId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("ValidTo");

                    b.ToTable("ActiveOrganisationLocationList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.ScheduledActions.Location.FutureActiveOrganisationLocationListItem", b =>
                {
                    b.Property<Guid>("OrganisationLocationId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("LocationId");

                    b.Property<Guid>("OrganisationId");

                    b.Property<DateTime?>("ValidFrom");

                    b.HasKey("OrganisationLocationId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("ValidFrom");

                    b.ToTable("FutureActiveOrganisationLocationList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.ScheduledActions.Parent.ActiveOrganisationParentListItem", b =>
                {
                    b.Property<Guid>("OrganisationOrganisationParentId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("OrganisationId");

                    b.Property<Guid>("ParentOrganisationId");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("OrganisationOrganisationParentId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("ValidTo");

                    b.ToTable("ActiveOrganisationParentList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.ScheduledActions.Parent.FutureActiveOrganisationParentListItem", b =>
                {
                    b.Property<Guid>("OrganisationOrganisationParentId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("OrganisationId");

                    b.Property<Guid>("ParentOrganisationId");

                    b.Property<DateTime?>("ValidFrom");

                    b.HasKey("OrganisationOrganisationParentId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("ValidFrom");

                    b.ToTable("FutureActiveOrganisationParentList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.OrganisationClassification.OrganisationClassificationListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<int>("Order");

                    b.Property<Guid>("OrganisationClassificationTypeId");

                    b.Property<string>("OrganisationClassificationTypeName")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Active");

                    b.HasIndex("Name")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("Order");

                    b.HasIndex("OrganisationClassificationTypeName");

                    b.HasIndex("Name", "OrganisationClassificationTypeId")
                        .IsUnique();

                    b.ToTable("OrganisationClassificationList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.OrganisationClassificationType.OrganisationClassificationTypeListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("OrganisationClassificationTypeList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.OrganisationRelationType.OrganisationRelationTypeListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("OrganisationRelationTypeList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Person.PersonCapacityListItem", b =>
                {
                    b.Property<Guid>("OrganisationCapacityId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CapacityId");

                    b.Property<string>("CapacityName")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<Guid?>("FunctionId");

                    b.Property<string>("FunctionName")
                        .HasMaxLength(500);

                    b.Property<Guid>("OrganisationId");

                    b.Property<string>("OrganisationName")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<Guid?>("PersonId")
                        .IsRequired();

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("OrganisationCapacityId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("CapacityName");

                    b.HasIndex("FunctionName");

                    b.HasIndex("OrganisationName")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("ValidFrom");

                    b.HasIndex("ValidTo");

                    b.ToTable("PersonCapacityList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Person.PersonFunctionListItem", b =>
                {
                    b.Property<Guid>("OrganisationFunctionId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("FunctionId");

                    b.Property<string>("FunctionName")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<Guid>("OrganisationId");

                    b.Property<string>("OrganisationName")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<Guid>("PersonId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("OrganisationFunctionId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("FunctionName");

                    b.HasIndex("OrganisationName")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("ValidFrom");

                    b.HasIndex("ValidTo");

                    b.ToTable("PersonFunctionList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Person.PersonListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("DateOfBirth");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(401);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(200);

                    b.Property<int?>("Sex");

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("FirstName");

                    b.HasIndex("FullName");

                    b.HasIndex("Name")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("PersonList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Person.PersonMandateListItem", b =>
                {
                    b.Property<Guid>("PersonMandateId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BodyId");

                    b.Property<Guid>("BodyMandateId");

                    b.Property<string>("BodyName")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<Guid?>("BodyOrganisationId");

                    b.Property<string>("BodyOrganisationName")
                        .HasMaxLength(500);

                    b.Property<Guid>("BodySeatId");

                    b.Property<string>("BodySeatName")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.Property<string>("BodySeatNumber")
                        .HasMaxLength(10);

                    b.Property<Guid?>("DelegationAssignmentId");

                    b.Property<Guid?>("FunctionTypeId");

                    b.Property<string>("FunctionTypeName")
                        .HasMaxLength(500);

                    b.Property<Guid?>("OrganisationId");

                    b.Property<string>("OrganisationName")
                        .HasMaxLength(500);

                    b.Property<bool>("PaidSeat");

                    b.Property<Guid>("PersonId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("PersonMandateId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("BodyName")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("ValidFrom");

                    b.HasIndex("ValidTo");

                    b.HasIndex("BodyMandateId", "DelegationAssignmentId")
                        .IsUnique();

                    b.ToTable("PersonMandateList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.ProjectionState.ProjectionStateItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("EventNumber");

                    b.Property<string>("Name")
                        .IsRequired();

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("ProjectionStateList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Purpose.PurposeListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("PurposeList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Reporting.BodySeatGenderRatioFormalFrameworkPerBodyListItem", b =>
                {
                    b.Property<Guid>("BodyId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("FormalFrameworkActive");

                    b.Property<Guid>("FormalFrameworkId");

                    b.Property<string>("FormalFrameworkName")
                        .IsRequired();

                    b.HasKey("BodyId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.ToTable("BodySeatGenderRatio_FormalFrameworkPerBodyList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Reporting.BodySeatGenderRatioItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("BodyActive");

                    b.Property<Guid>("BodyId");

                    b.Property<Guid?>("BodyMandateId");

                    b.Property<string>("BodyName")
                        .HasMaxLength(500);

                    b.Property<bool>("BodySeatActive");

                    b.Property<Guid>("BodySeatId");

                    b.Property<string>("BodySeatNumber")
                        .HasMaxLength(10);

                    b.Property<Guid>("BodySeatTypeId");

                    b.Property<string>("BodySeatTypeName")
                        .HasMaxLength(500);

                    b.Property<bool>("FormalFrameworkActive");

                    b.Property<Guid?>("FormalFrameworkId");

                    b.Property<string>("FormalFrameworkName")
                        .HasMaxLength(500);

                    b.Property<bool>("IsAssigned")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(false);

                    b.Property<bool>("IsFunctionAssigned")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(false);

                    b.Property<bool>("IsOrganisationAssigned")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(false);

                    b.Property<bool>("IsPersonAssigned")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValue(false);

                    b.Property<bool>("OrganisationActive");

                    b.Property<Guid?>("OrganisationId");

                    b.Property<string>("OrganisationName")
                        .HasMaxLength(500);

                    b.Property<string>("PersonFullName")
                        .HasMaxLength(401);

                    b.Property<Guid?>("PersonId");

                    b.Property<int?>("PersonSex");

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.ToTable("BodySeatGenderRatioList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Reporting.BodySeatGenderRatioOrganisationListItem", b =>
                {
                    b.Property<Guid>("OrganisationId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("OrganisationActive");

                    b.Property<string>("OrganisationName")
                        .IsRequired();

                    b.HasKey("OrganisationId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.ToTable("BodySeatGenderRatio_OrganisationList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Reporting.BodySeatGenderRatioOrganisationPerBodyListItem", b =>
                {
                    b.Property<Guid>("BodyId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("OrganisationId");

                    b.Property<string>("OrganisationName")
                        .IsRequired();

                    b.HasKey("BodyId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.ToTable("BodySeatGenderRatio_OrganisationPerBodyList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Reporting.BodySeatGenderRatioPersonListItem", b =>
                {
                    b.Property<Guid>("PersonId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("PersonFullName")
                        .HasMaxLength(401);

                    b.Property<int?>("PersonSex");

                    b.HasKey("PersonId")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.ToTable("BodySeatGenderRatio_PersonList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.SeatType.SeatTypeListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(500);

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("SeatTypeList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Security.OrganisationTreeItem", b =>
                {
                    b.Property<string>("OvoNumber")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(10);

                    b.Property<string>("OrganisationTree");

                    b.HasKey("OvoNumber");

                    b.ToTable("OrganisationTreeList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Body.BodyLifecyclePhaseValidity", b =>
                {
                    b.HasOne("OrganisationRegistry.SqlServer.Body.BodyListItem")
                        .WithMany("BodyLifecyclePhaseValidities")
                        .HasForeignKey("BodyListItemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationClassificationValidity", b =>
                {
                    b.HasOne("OrganisationRegistry.SqlServer.Organisation.OrganisationListItem")
                        .WithMany("OrganisationClassificationValidities")
                        .HasForeignKey("OrganisationListItemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationFormalFrameworkValidity", b =>
                {
                    b.HasOne("OrganisationRegistry.SqlServer.Organisation.OrganisationListItem")
                        .WithMany("FormalFrameworkValidities")
                        .HasForeignKey("OrganisationListItemId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
        }
    }
}
