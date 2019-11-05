using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddVlaanderenBeNotificationsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VlaanderenBeNotifications",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Event = table.Column<string>(nullable: true),
                    EventNumber = table.Column<int>(nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    OrganisationName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VlaanderenBeNotifications", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VlaanderenBeNotifications",
                schema: "OrganisationRegistry");
        }
    }
}
