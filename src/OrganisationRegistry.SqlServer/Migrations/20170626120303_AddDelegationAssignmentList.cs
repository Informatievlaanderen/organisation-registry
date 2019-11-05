using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddDelegationAssignmentList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DelegationAssignmentList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BodyMandateId = table.Column<Guid>(nullable: false),
                    PersonId = table.Column<Guid>(nullable: false),
                    PersonName = table.Column<string>(maxLength: 200, nullable: true),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DelegationAssignmentList", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DelegationAssignmentList_BodyMandateId",
                schema: "OrganisationRegistry",
                table: "DelegationAssignmentList",
                column: "BodyMandateId");

            migrationBuilder.CreateIndex(
                name: "IX_DelegationAssignmentList_PersonName",
                schema: "OrganisationRegistry",
                table: "DelegationAssignmentList",
                column: "PersonName");

            migrationBuilder.CreateIndex(
                name: "IX_DelegationAssignmentList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "DelegationAssignmentList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_DelegationAssignmentList_ValidTo",
                schema: "OrganisationRegistry",
                table: "DelegationAssignmentList",
                column: "ValidTo");

            migrationBuilder.CreateIndex(
                name: "IX_DelegationAssignmentList_BodyMandateId_PersonName",
                schema: "OrganisationRegistry",
                table: "DelegationAssignmentList",
                columns: new[] { "BodyMandateId", "PersonName" })
                .Annotation("SqlServer:Clustered", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DelegationAssignmentList",
                schema: "OrganisationRegistry");
        }
    }
}
