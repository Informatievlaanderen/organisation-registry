using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddLifecyclePhaseFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDefaultPhase",
                schema: "OrganisationRegistry",
                table: "LifecyclePhaseList",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "RepresentsActivePhase",
                schema: "OrganisationRegistry",
                table: "LifecyclePhaseList",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateIndex(
                name: "IX_LifecyclePhaseList_RepresentsActivePhase_IsDefaultPhase",
                schema: "OrganisationRegistry",
                table: "LifecyclePhaseList",
                columns: new[] { "RepresentsActivePhase", "IsDefaultPhase" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_LifecyclePhaseList_RepresentsActivePhase_IsDefaultPhase",
                schema: "OrganisationRegistry",
                table: "LifecyclePhaseList");

            migrationBuilder.DropColumn(
                name: "IsDefaultPhase",
                schema: "OrganisationRegistry",
                table: "LifecyclePhaseList");

            migrationBuilder.DropColumn(
                name: "RepresentsActivePhase",
                schema: "OrganisationRegistry",
                table: "LifecyclePhaseList");
        }
    }
}
