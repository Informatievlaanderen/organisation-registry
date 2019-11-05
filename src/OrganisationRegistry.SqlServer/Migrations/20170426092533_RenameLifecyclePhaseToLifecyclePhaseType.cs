using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class RenameLifecyclePhaseToLifecyclePhaseType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LifecyclePhaseList",
                schema: "OrganisationRegistry");

            migrationBuilder.CreateTable(
                name: "LifecyclePhaseTypeList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDefaultPhase = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 500, nullable: false),
                    RepresentsActivePhase = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LifecyclePhaseTypeList", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LifecyclePhaseTypeList_Name",
                schema: "OrganisationRegistry",
                table: "LifecyclePhaseTypeList",
                column: "Name",
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_LifecyclePhaseTypeList_RepresentsActivePhase_IsDefaultPhase",
                schema: "OrganisationRegistry",
                table: "LifecyclePhaseTypeList",
                columns: new[] { "RepresentsActivePhase", "IsDefaultPhase" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LifecyclePhaseTypeList",
                schema: "OrganisationRegistry");

            migrationBuilder.CreateTable(
                name: "LifecyclePhaseList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsDefaultPhase = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(maxLength: 500, nullable: false),
                    RepresentsActivePhase = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LifecyclePhaseList", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_LifecyclePhaseList_Name",
                schema: "OrganisationRegistry",
                table: "LifecyclePhaseList",
                column: "Name",
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_LifecyclePhaseList_RepresentsActivePhase_IsDefaultPhase",
                schema: "OrganisationRegistry",
                table: "LifecyclePhaseList",
                columns: new[] { "RepresentsActivePhase", "IsDefaultPhase" });
        }
    }
}
