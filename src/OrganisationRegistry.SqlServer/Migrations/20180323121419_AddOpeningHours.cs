using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddOpeningHours : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganisationOpeningHourList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationOpeningHourId = table.Column<Guid>(nullable: false),
                    Closes = table.Column<TimeSpan>(nullable: false),
                    DayOfWeek = table.Column<int>(nullable: true),
                    Opens = table.Column<TimeSpan>(nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationOpeningHourList", x => x.OrganisationOpeningHourId)
                        .Annotation("SqlServer:Clustered", false);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganisationOpeningHourList",
                schema: "OrganisationRegistry");
        }
    }
}
