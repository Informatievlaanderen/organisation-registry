using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class CreateActivePeopleAssignedToBodyMandatesList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ActivePeopleAssignedToBodyMandatesList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    DelegationAssignmentId = table.Column<Guid>(nullable: false),
                    BodyId = table.Column<Guid>(nullable: false),
                    BodyMandateId = table.Column<Guid>(nullable: false),
                    PersonFullName = table.Column<string>(nullable: false),
                    PersonId = table.Column<Guid>(nullable: false),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActivePeopleAssignedToBodyMandatesList", x => x.DelegationAssignmentId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActivePeopleAssignedToBodyMandatesList_ValidTo",
                schema: "OrganisationRegistry",
                table: "ActivePeopleAssignedToBodyMandatesList",
                column: "ValidTo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ActivePeopleAssignedToBodyMandatesList",
                schema: "OrganisationRegistry");
        }
    }
}
