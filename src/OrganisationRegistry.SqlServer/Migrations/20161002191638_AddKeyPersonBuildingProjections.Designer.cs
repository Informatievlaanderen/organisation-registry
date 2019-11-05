using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    using Infrastructure;

    [DbContext(typeof(OrganisationRegistryContext))]
    [Migration("20161002191638_AddKeyPersonBuildingProjections")]
    partial class AddKeyPersonBuildingProjections
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

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Key.KeyList", b =>
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
