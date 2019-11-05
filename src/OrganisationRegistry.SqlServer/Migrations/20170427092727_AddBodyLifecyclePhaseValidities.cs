using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddBodyLifecyclePhaseValidities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BodyLifecyclePhaseValidity",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    BodyId = table.Column<Guid>(nullable: false),
                    BodyLifecyclePhaseId = table.Column<Guid>(nullable: false),
                    BodyListItemId = table.Column<Guid>(nullable: true),
                    RepresentsActivePhase = table.Column<bool>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyLifecyclePhaseValidity", x => x.BodyId)
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_BodyLifecyclePhaseValidity_BodyList_BodyListItemId",
                        column: x => x.BodyListItemId,
                        principalSchema: "OrganisationRegistry",
                        principalTable: "BodyList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BodyLifecyclePhaseValidity_BodyListItemId",
                schema: "OrganisationRegistry",
                table: "BodyLifecyclePhaseValidity",
                column: "BodyListItemId");

            migrationBuilder.CreateIndex(
                name: "IX_BodyLifecyclePhaseValidity_RepresentsActivePhase",
                schema: "OrganisationRegistry",
                table: "BodyLifecyclePhaseValidity",
                column: "RepresentsActivePhase");

            migrationBuilder.CreateIndex(
                name: "IX_BodyLifecyclePhaseValidity_ValidFrom",
                schema: "OrganisationRegistry",
                table: "BodyLifecyclePhaseValidity",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_BodyLifecyclePhaseValidity_ValidTo",
                schema: "OrganisationRegistry",
                table: "BodyLifecyclePhaseValidity",
                column: "ValidTo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BodyLifecyclePhaseValidity",
                schema: "OrganisationRegistry");
        }
    }
}
