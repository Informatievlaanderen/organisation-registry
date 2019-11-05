using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using OrganisationRegistry.SqlServer.Infrastructure;

namespace OrganisationRegistry.SqlServer.Migrations
{
    [DbContext(typeof(OrganisationRegistryContext))]
    [Migration("20161220200041_AddPurposes")]
    partial class AddPurposes
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
                        .HasAnnotation("MaxLength", 500);

                    b.Property<string>("VimId")
                        .HasAnnotation("MaxLength", 100);

                    b.HasKey("Id");

                    b.ToTable("BuildingList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Capacity.CapacityListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 500);

                    b.HasKey("Id");

                    b.ToTable("CapacityList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.ContactType.ContactTypeListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 500);

                    b.HasKey("Id");

                    b.ToTable("ContactTypeList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.FormalFramework.FormalFrameworkListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .HasAnnotation("MaxLength", 50);

                    b.Property<Guid>("FormalFrameworkCategoryId");

                    b.Property<string>("FormalFrameworkCategoryName")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 500);

                    b.HasKey("Id");

                    b.ToTable("FormalFrameworkList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.FormalFrameworkCategory.FormalFrameworkCategoryListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 500);

                    b.HasKey("Id");

                    b.ToTable("FormalFrameworkCategoryList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Function.FunctionListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 500);

                    b.HasKey("Id");

                    b.ToTable("FunctionList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.KeyType.KeyTypeListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 500);

                    b.HasKey("Id");

                    b.ToTable("KeyTypeList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.LabelType.LabelTypeListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 500);

                    b.HasKey("Id");

                    b.ToTable("LabelTypeList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Location.LocationListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("City");

                    b.Property<string>("Country");

                    b.Property<string>("CrabLocationId");

                    b.Property<string>("FormattedAddress");

                    b.Property<string>("Street");

                    b.Property<string>("ZipCode");

                    b.HasKey("Id");

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
                        .HasAnnotation("MaxLength", 500);

                    b.Property<bool>("IsMainBuilding");

                    b.Property<Guid>("OrganisationId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("OrganisationBuildingId");

                    b.ToTable("OrganisationBuildingList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationCapacityListItem", b =>
                {
                    b.Property<Guid>("OrganisationCapacityId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CapacityId");

                    b.Property<string>("CapacityName")
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

                    b.HasKey("OrganisationCapacityId");

                    b.ToTable("OrganisationCapacityList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationChildListItem", b =>
                {
                    b.Property<Guid>("OrganisationOrganisationParentId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("Id");

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<DateTime?>("OrganisationValidFrom");

                    b.Property<DateTime?>("OrganisationValidTo");

                    b.Property<string>("OvoNumber")
                        .IsRequired();

                    b.Property<Guid>("ParentOrganisationId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("OrganisationOrganisationParentId");

                    b.ToTable("OrganisationChildList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationContactListItem", b =>
                {
                    b.Property<Guid>("OrganisationContactId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("ContactTypeId");

                    b.Property<string>("ContactTypeName")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<string>("ContactValue")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<Guid>("OrganisationId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("OrganisationContactId");

                    b.ToTable("OrganisationContactList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationFunctionListItem", b =>
                {
                    b.Property<Guid>("OrganisationFunctionId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("FunctionId");

                    b.Property<string>("FunctionName")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<Guid>("OrganisationId");

                    b.Property<Guid>("PersonId");

                    b.Property<string>("PersonName")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("OrganisationFunctionId");

                    b.ToTable("OrganisationFunctionList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationKeyListItem", b =>
                {
                    b.Property<Guid>("OrganisationKeyId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("KeyTypeId");

                    b.Property<string>("KeyTypeName")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<string>("KeyValue")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<Guid>("OrganisationId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("OrganisationKeyId");

                    b.ToTable("OrganisationKeyList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationLabelListItem", b =>
                {
                    b.Property<Guid>("OrganisationLabelId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("LabelTypeId");

                    b.Property<string>("LabelTypeName")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<string>("LabelValue")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<Guid>("OrganisationId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("OrganisationLabelId");

                    b.ToTable("OrganisationLabelList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationListItem", b =>
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
                        .HasAnnotation("MaxLength", 500);

                    b.Property<Guid?>("OrganisationClassificationId");

                    b.Property<Guid?>("OrganisationClassificationTypeId");

                    b.Property<string>("OvoNumber")
                        .HasAnnotation("MaxLength", 10);

                    b.Property<string>("ParentOrganisation");

                    b.Property<Guid?>("ParentOrganisationId");

                    b.Property<string>("Purposes");

                    b.Property<string>("ShortName");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("Id");

                    b.ToTable("OrganisationList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationLocationListItem", b =>
                {
                    b.Property<Guid>("OrganisationLocationId")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("IsMainLocation");

                    b.Property<Guid>("LocationId");

                    b.Property<string>("LocationName")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<Guid>("OrganisationId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("OrganisationLocationId");

                    b.ToTable("OrganisationLocationList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationOrganisationClassificationListItem", b =>
                {
                    b.Property<Guid>("OrganisationOrganisationClassificationId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("OrganisationClassificationId");

                    b.Property<string>("OrganisationClassificationName")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<Guid>("OrganisationClassificationTypeId");

                    b.Property<string>("OrganisationClassificationTypeName")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<Guid>("OrganisationId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("OrganisationOrganisationClassificationId");

                    b.ToTable("OrganisationOrganisationClassificationList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationParentListItem", b =>
                {
                    b.Property<Guid>("OrganisationOrganisationParentId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("OrganisationId");

                    b.Property<Guid>("ParentOrganisationId");

                    b.Property<string>("ParentOrganisationName")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("OrganisationOrganisationParentId");

                    b.ToTable("OrganisationParentList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.OrganisationClassification.OrganisationClassificationListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Active");

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<int>("Order");

                    b.Property<Guid>("OrganisationClassificationTypeId");

                    b.Property<string>("OrganisationClassificationTypeName")
                        .HasAnnotation("MaxLength", 500);

                    b.HasKey("Id");

                    b.ToTable("OrganisationClassificationList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.OrganisationClassificationType.OrganisationClassificationTypeListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 500);

                    b.HasKey("Id");

                    b.ToTable("OrganisationClassificationTypeList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.OrganisationRelationType.OrganisationRelationTypeListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 500);

                    b.HasKey("Id");

                    b.ToTable("OrganisationRelationTypeList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Person.PersonCapacityListItem", b =>
                {
                    b.Property<Guid>("OrganisationCapacityId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("CapacityId");

                    b.Property<string>("CapacityName")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<Guid?>("FunctionId");

                    b.Property<string>("FunctionName")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<Guid>("OrganisationId");

                    b.Property<string>("OrganisationName")
                        .HasAnnotation("MaxLength", 2000);

                    b.Property<Guid?>("PersonId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("OrganisationCapacityId");

                    b.ToTable("PersonCapacityList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Person.PersonFunctionListItem", b =>
                {
                    b.Property<Guid>("OrganisationFunctionId")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid>("FunctionId");

                    b.Property<string>("FunctionName")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<Guid>("OrganisationId");

                    b.Property<string>("OrganisationName")
                        .HasAnnotation("MaxLength", 2000);

                    b.Property<Guid>("PersonId");

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("OrganisationFunctionId");

                    b.ToTable("PersonFunctionList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Person.PersonListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime?>("DateOfBirth");

                    b.Property<string>("FirstName")
                        .HasAnnotation("MaxLength", 200);

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 200);

                    b.Property<int?>("Sex");

                    b.HasKey("Id");

                    b.ToTable("PersonList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Purpose.PurposeListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 500);

                    b.HasKey("Id");

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
        }
    }
}
