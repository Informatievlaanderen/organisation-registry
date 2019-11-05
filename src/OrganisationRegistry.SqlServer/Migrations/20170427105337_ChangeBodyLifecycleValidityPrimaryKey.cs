using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class ChangeBodyLifecycleValidityPrimaryKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BodyLifecyclePhaseValidity",
                schema: "OrganisationRegistry",
                table: "BodyLifecyclePhaseValidity");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BodyLifecyclePhaseValidity",
                schema: "OrganisationRegistry",
                table: "BodyLifecyclePhaseValidity",
                column: "BodyLifecyclePhaseId")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_BodyLifecyclePhaseValidity_BodyId",
                schema: "OrganisationRegistry",
                table: "BodyLifecyclePhaseValidity",
                column: "BodyId")
                .Annotation("SqlServer:Clustered", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BodyLifecyclePhaseValidity",
                schema: "OrganisationRegistry",
                table: "BodyLifecyclePhaseValidity");

            migrationBuilder.DropIndex(
                name: "IX_BodyLifecyclePhaseValidity_BodyId",
                schema: "OrganisationRegistry",
                table: "BodyLifecyclePhaseValidity");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BodyLifecyclePhaseValidity",
                schema: "OrganisationRegistry",
                table: "BodyLifecyclePhaseValidity",
                column: "BodyId")
                .Annotation("SqlServer:Clustered", true);
        }
    }
}
