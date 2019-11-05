using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddBodyContacts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PersonMandateList_BodyMandateId_DelegationAssignmentId",
                schema: "OrganisationRegistry",
                table: "PersonMandateList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationList_OrganisationId_FormalFrameworkId",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.CreateTable(
                name: "BodyContactList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    BodyContactId = table.Column<Guid>(nullable: false),
                    BodyId = table.Column<Guid>(nullable: false),
                    ContactTypeId = table.Column<Guid>(nullable: false),
                    ContactTypeName = table.Column<string>(maxLength: 500, nullable: false),
                    ContactValue = table.Column<string>(maxLength: 500, nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyContactList", x => x.BodyContactId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PersonMandateList_BodyMandateId_DelegationAssignmentId",
                schema: "OrganisationRegistry",
                table: "PersonMandateList",
                columns: new[] { "BodyMandateId", "DelegationAssignmentId" },
                unique: true,
                filter: "[DelegationAssignmentId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationList_OrganisationId_FormalFrameworkId",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                columns: new[] { "OrganisationId", "FormalFrameworkId" },
                unique: true,
                filter: "[FormalFrameworkId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BodyContactList_ContactTypeName",
                schema: "OrganisationRegistry",
                table: "BodyContactList",
                column: "ContactTypeName")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_BodyContactList_ContactValue",
                schema: "OrganisationRegistry",
                table: "BodyContactList",
                column: "ContactValue");

            migrationBuilder.CreateIndex(
                name: "IX_BodyContactList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "BodyContactList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_BodyContactList_ValidTo",
                schema: "OrganisationRegistry",
                table: "BodyContactList",
                column: "ValidTo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BodyContactList",
                schema: "OrganisationRegistry");

            migrationBuilder.DropIndex(
                name: "IX_PersonMandateList_BodyMandateId_DelegationAssignmentId",
                schema: "OrganisationRegistry",
                table: "PersonMandateList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationList_OrganisationId_FormalFrameworkId",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.CreateIndex(
                name: "IX_PersonMandateList_BodyMandateId_DelegationAssignmentId",
                schema: "OrganisationRegistry",
                table: "PersonMandateList",
                columns: new[] { "BodyMandateId", "DelegationAssignmentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationList_OrganisationId_FormalFrameworkId",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                columns: new[] { "OrganisationId", "FormalFrameworkId" },
                unique: true);
        }
    }
}
