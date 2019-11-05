using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    using Infrastructure;

    [DbContext(typeof(OrganisationRegistryContext))]
    [Migration("20161002174109_AddLocationList")]
    partial class AddLocationList
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Entities.Address", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AddressLine")
                        .HasAnnotation("MaxLength", 500);

                    b.Property<string>("CountryCode")
                        .HasAnnotation("MaxLength", 3);

                    b.Property<int?>("CrabId");

                    b.Property<string>("Gemeente")
                        .HasAnnotation("MaxLength", 100);

                    b.Property<string>("PostCode")
                        .HasAnnotation("MaxLength", 20);

                    b.HasKey("Id");

                    b.ToTable("Address","OrganisationRegistry");
                });

            modelBuilder.Entity("OrganisationRegistry.SqlServer.Entities.LocationList", b =>
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
        }
    }
}
