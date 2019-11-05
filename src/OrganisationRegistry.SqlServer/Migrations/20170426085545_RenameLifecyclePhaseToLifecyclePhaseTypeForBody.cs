using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class RenameLifecyclePhaseToLifecyclePhaseTypeForBody : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LifecyclePhaseName",
                schema: "OrganisationRegistry",
                table: "BodyLifecyclePhaseList",
                newName: "LifecyclePhaseTypeName");

            migrationBuilder.RenameColumn(
                name: "LifecyclePhaseId",
                schema: "OrganisationRegistry",
                table: "BodyLifecyclePhaseList",
                newName: "LifecyclePhaseTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_BodyLifecyclePhaseList_LifecyclePhaseName",
                schema: "OrganisationRegistry",
                table: "BodyLifecyclePhaseList",
                newName: "IX_BodyLifecyclePhaseList_LifecyclePhaseTypeName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LifecyclePhaseTypeName",
                schema: "OrganisationRegistry",
                table: "BodyLifecyclePhaseList",
                newName: "LifecyclePhaseName");

            migrationBuilder.RenameColumn(
                name: "LifecyclePhaseTypeId",
                schema: "OrganisationRegistry",
                table: "BodyLifecyclePhaseList",
                newName: "LifecyclePhaseId");

            migrationBuilder.RenameIndex(
                name: "IX_BodyLifecyclePhaseList_LifecyclePhaseTypeName",
                schema: "OrganisationRegistry",
                table: "BodyLifecyclePhaseList",
                newName: "IX_BodyLifecyclePhaseList_LifecyclePhaseName");
        }
    }
}
