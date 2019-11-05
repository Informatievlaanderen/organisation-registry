using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using OrganisationRegistry.SqlServer.Infrastructure;

namespace OrganisationRegistry.SqlServer.Migrations
{
    [DbContext(typeof(OrganisationRegistryContext))]
    [Migration("20161026123235_AddOrganisationChildrenProjections")]
    partial class AddOrganisationChildrenProjections
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

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Key.KeyListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 500);

                    b.HasKey("Id");

                    b.ToTable("KeyList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Location.LocationList", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AddressLine")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<string>("City")
                        .HasAnnotation("MaxLength", 100);

                    b.Property<string>("ZipCode")
                        .HasAnnotation("MaxLength", 20);

                    b.HasKey("Id");

                    b.ToTable("LocationList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationKeyListItem", b =>
                {
                    b.Property<Guid>("OrganisationId");

                    b.Property<Guid>("KeyId");

                    b.Property<string>("KeyName")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<string>("KeyValue")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<DateTime?>("ValidFrom");

                    b.Property<DateTime?>("ValidTo");

                    b.HasKey("OrganisationId", "KeyId");

                    b.ToTable("OrganisationKeyList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<Guid?>("FormalFrameworkId");

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<Guid?>("OrganisationClassificationId");

                    b.Property<Guid?>("OrganisationClassificationTypeId");

                    b.Property<string>("OvoNumber");

                    b.Property<string>("ParentOrganisation");

                    b.Property<Guid?>("ParentOrganisationId");

                    b.HasKey("Id");

                    b.ToTable("OrganisationList","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.OrganisationChild.OrganisationChildListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<Guid?>("ParentOrganisationId");

                    b.HasKey("Id");

                    b.ToTable("OrganisationChildList","OrganisationRegistry");
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

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Person.PersonList", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateOfBirth");

                    b.Property<string>("Firstname")
                        .HasAnnotation("MaxLength", 100);

                    b.Property<int>("Gender");

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 100);

                    b.HasKey("Id");

                    b.ToTable("PersonList","OrganisationRegistry");
                });
        }
    }
}
