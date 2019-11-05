using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using OrganisationRegistry.SqlServer.Infrastructure;

namespace OrganisationRegistry.SqlServer.Migrations
{
    [DbContext(typeof(OrganisationRegistryContext))]
    [Migration("20170112093519_AddOrganisationListIndices")]
    partial class AddOrganisationListIndices
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Building.BuildingListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 500);

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
                        .HasAnnotation("MaxLength", 500);

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("CapacityList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.ContactType.ContactTypeListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 500);

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("ContactTypeList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.FormalFramework.FormalFrameworkListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 50);

                    b.Property<Guid>("FormalFrameworkCategoryId");

                    b.Property<string>("FormalFrameworkCategoryName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 500);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 500);

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
                        .HasAnnotation("MaxLength", 500);

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("FormalFrameworkCategoryList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Function.FunctionListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 500);

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
                        .HasAnnotation("MaxLength", 500);

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
                        .HasAnnotation("MaxLength", 500);

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("LabelTypeList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Location.LocationListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("City")
                        .IsRequired();

                    b.Property<string>("Country")
                        .IsRequired();

                    b.Property<string>("CrabLocationId");

                    b.Property<string>("FormattedAddress");

                    b.Property<string>("Street")
                        .IsRequired();

                    b.Property<string>("ZipCode")
                        .IsRequired();

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("City");

                    b.HasIndex("Country");

                    b.HasIndex("FormattedAddress")
                        .HasAnnotation("SqlServer:Clustered", true);

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
                        .HasAnnotation("MaxLength", 50);

                    b.Property<DateTime>("Logged");

                    b.Property<string>("Logger")
                        .HasAnnotation("MaxLength", 250);

                    b.Property<string>("Message");

                    b.HasKey("Id");

                    b.ToTable("Logs","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationBuildingListItem", b =>
                {
                    b.Property<Guid>("OrganisationBuildingId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("BuildingId");

                    b.Property<string>("BuildingName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 500);

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
                        .HasAnnotation("MaxLength", 500);

                    b.Property<Guid?>("FunctionId");

                    b.Property<string>("FunctionName")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<Guid>("OrganisationId");

                    b.Property<Guid?>("PersonId");

                    b.Property<string>("PersonName")
                        .HasAnnotation("MaxLength", 500);

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
                        .HasAnnotation("MaxLength", 500);

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

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationContactListItem", b =>
                {
                    b.Property<Guid>("OrganisationContactId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ContactTypeId");

                    b.Property<string>("ContactTypeName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 500);

                    b.Property<string>("ContactValue")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 500);

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
                        .HasAnnotation("MaxLength", 500);

                    b.Property<Guid?>("OrganisationClassificationId");

                    b.Property<Guid?>("OrganisationClassificationTypeId");

                    b.Property<string>("OvoNumber")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 10);

                    b.Property<string>("ParentOrganisation");

                    b.Property<Guid?>("ParentOrganisationId");

                    b.Property<string>("PurposeIds");

                    b.Property<string>("PurposeNames");

                    b.Property<string>("ShortName");

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
                        .HasAnnotation("MaxLength", 500);

                    b.Property<Guid>("OrganisationId");

                    b.Property<Guid>("ParentOrganisationId");

                    b.Property<string>("ParentOrganisationName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 500);

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

                    b.ToTable("OrganisationFormalFrameworkValidity","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationFunctionListItem", b =>
                {
                    b.Property<Guid>("OrganisationFunctionId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("FunctionId");

                    b.Property<string>("FunctionName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 500);

                    b.Property<Guid>("OrganisationId");

                    b.Property<Guid>("PersonId");

                    b.Property<string>("PersonName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 500);

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
                        .HasAnnotation("MaxLength", 500);

                    b.Property<string>("KeyValue")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 500);

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
                        .HasAnnotation("MaxLength", 500);

                    b.Property<string>("LabelValue")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 500);

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
                        .HasAnnotation("MaxLength", 500);

                    b.Property<Guid?>("OrganisationClassificationId");

                    b.Property<Guid?>("OrganisationClassificationTypeId");

                    b.Property<Guid>("OrganisationId");

                    b.Property<string>("OvoNumber")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 10);

                    b.Property<string>("ParentOrganisation");

                    b.Property<Guid?>("ParentOrganisationId");

                    b.Property<string>("ShortName");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("FormalFrameworkId");

                    b.HasIndex("Name")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.HasIndex("OrganisationClassificationId");

                    b.HasIndex("OrganisationClassificationTypeId");

                    b.HasIndex("OvoNumber");

                    b.HasIndex("ParentOrganisation");

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
                        .HasAnnotation("MaxLength", 500);

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
                        .HasAnnotation("MaxLength", 500);

                    b.Property<Guid>("OrganisationClassificationTypeId");

                    b.Property<string>("OrganisationClassificationTypeName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 500);

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
                        .HasAnnotation("MaxLength", 500);

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

            modelBuilder.Entity("OrganisationRegistry.SqlServer.OrganisationClassification.OrganisationClassificationListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 500);

                    b.Property<int>("Order");

                    b.Property<Guid>("OrganisationClassificationTypeId");

                    b.Property<string>("OrganisationClassificationTypeName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 500);

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
                        .HasAnnotation("MaxLength", 500);

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
                        .HasAnnotation("MaxLength", 500);

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
                        .HasAnnotation("MaxLength", 500);

                    b.Property<Guid?>("FunctionId");

                    b.Property<string>("FunctionName")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<Guid>("OrganisationId");

                    b.Property<string>("OrganisationName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 2000);

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
                        .HasAnnotation("MaxLength", 500);

                    b.Property<Guid>("OrganisationId");

                    b.Property<string>("OrganisationName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 2000);

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
                        .HasAnnotation("MaxLength", 200);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 200);

                    b.Property<int?>("Sex");

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("FirstName");

                    b.HasIndex("Name")
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("PersonList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Purpose.PurposeListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 500);

                    b.HasKey("Id")
                        .HasAnnotation("SqlServer:Clustered", false);

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasAnnotation("SqlServer:Clustered", true);

                    b.ToTable("PurposeList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Security.OrganisationTreeItem", b =>
                {
                    b.Property<string>("OvoNumber")
                        .HasAnnotation("MaxLength", 10);

                    b.Property<string>("OrganisationTree");

                    b.HasKey("OvoNumber");

                    b.ToTable("OrganisationTreeList","OrganisationRegistry");
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
