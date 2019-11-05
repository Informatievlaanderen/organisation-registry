using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class DummyEventsMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.CreateTable(
            //    name: "Events",
            //    schema: "OrganisationRegistry",
            //    columns: table => new
            //    {
            //        Id = table.Column<Guid>(nullable: false),
            //        Version = table.Column<int>(nullable: false),
            //        Data = table.Column<string>(nullable: false),
            //        FirstName = table.Column<string>(maxLength: 200, nullable: true),
            //        Ip = table.Column<string>(maxLength: 100, nullable: true),
            //        LastName = table.Column<string>(maxLength: 200, nullable: true),
            //        Name = table.Column<string>(maxLength: 200, nullable: false),
            //        Number = table.Column<int>(nullable: false)
            //            .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
            //        Timestamp = table.Column<DateTimeOffset>(nullable: false),
            //        UserId = table.Column<string>(maxLength: 100, nullable: true)
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Events", x => new { x.Id, x.Version })
            //            .Annotation("SqlServer:Clustered", true);
            //    });

            //migrationBuilder.CreateIndex(
            //    name: "IX_Events_Name",
            //    schema: "OrganisationRegistry",
            //    table: "Events",
            //    column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropTable(
            //    name: "Events",
            //    schema: "OrganisationRegistry");
        }
    }
}
