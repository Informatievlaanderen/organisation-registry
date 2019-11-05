using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using OrganisationRegistry.SqlServer.Infrastructure;

namespace OrganisationRegistry.SqlServer.Migrations
{
    [DbContext(typeof(OrganisationRegistryContext))]
    [Migration("20161024193242_RenameMotherOrganisationToParentOrganisation")]
    partial class RenameMotherOrganisationToParentOrganisation
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

            modelBuilder.Entity("OrganisationRegistry.SqlServer.CapacityListType.CapacityListItem", b =>
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

            modelBuilder.Entity("OrganisationRegistry.SqlServer.FunctionListType.FunctionListItem", b =>
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

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Organisation.OrganisationListItem", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<string>("OvoNumber");

                    b.Property<string>("ParentOrganisation");

                    b.Property<Guid>("ParentOrganisationId");

                    b.HasKey("Id");

                    b.ToTable("OrganisationList","OrganisationRegistry");
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

                    b.Property<string>("FirstName")
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
