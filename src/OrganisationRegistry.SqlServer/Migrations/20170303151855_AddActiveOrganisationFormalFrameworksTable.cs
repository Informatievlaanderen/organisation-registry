using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddActiveOrganisationFormalFrameworksTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VlaanderenBeNotificationsQueue",
                schema: "OrganisationRegistry");

            migrationBuilder.CreateTable(
                name: "ActiveOrganisationFormalFrameworkList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationFormalFrameworkId = table.Column<Guid>(nullable: false),
                    FormalFrameworkId = table.Column<Guid>(nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActiveOrganisationFormalFrameworkList", x => x.OrganisationFormalFrameworkId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActiveOrganisationFormalFrameworkList_ValidTo",
                schema: "OrganisationRegistry",
                table: "ActiveOrganisationFormalFrameworkList",
                column: "ValidTo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActiveOrganisationFormalFrameworkList",
                schema: "OrganisationRegistry");

            migrationBuilder.CreateTable(
                name: "VlaanderenBeNotificationsQueue",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Body = table.Column<string>(nullable: true),
                    EventNumber = table.Column<int>(nullable: false),
                    Subject = table.Column<string>(nullable: true),
                    To = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VlaanderenBeNotificationsQueue", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });
        }
    }
}
