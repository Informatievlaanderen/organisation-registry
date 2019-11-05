using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class CreateFuturePeopleAssignedToBodyMandatesList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FuturePeopleAssignedToBodyMandatesList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    DelegationAssignmentId = table.Column<Guid>(nullable: false),
                    BodyId = table.Column<Guid>(nullable: false),
                    BodyMandateId = table.Column<Guid>(nullable: false),
                    PersonFullName = table.Column<string>(nullable: false),
                    PersonId = table.Column<Guid>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FuturePeopleAssignedToBodyMandatesList", x => x.DelegationAssignmentId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FuturePeopleAssignedToBodyMandatesList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "FuturePeopleAssignedToBodyMandatesList",
                column: "ValidFrom");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FuturePeopleAssignedToBodyMandatesList",
                schema: "OrganisationRegistry");
        }
    }
}
