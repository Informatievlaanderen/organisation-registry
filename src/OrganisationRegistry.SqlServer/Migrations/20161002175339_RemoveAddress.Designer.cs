using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    using Infrastructure;

    [DbContext(typeof(OrganisationRegistryContext))]
    [Migration("20161002175339_RemoveAddress")]
    partial class RemoveAddress
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.1")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

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
