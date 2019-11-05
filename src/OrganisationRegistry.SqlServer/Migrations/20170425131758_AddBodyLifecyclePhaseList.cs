using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddBodyLifecyclePhaseList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BodyLifecyclePhaseList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    BodyLifecyclePhaseId = table.Column<Guid>(nullable: false),
                    BodyId = table.Column<Guid>(nullable: false),
                    LifecyclePhaseId = table.Column<Guid>(nullable: false),
                    LifecyclePhaseName = table.Column<string>(maxLength: 500, nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyLifecyclePhaseList", x => x.BodyLifecyclePhaseId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BodyLifecyclePhaseList_LifecyclePhaseName",
                schema: "OrganisationRegistry",
                table: "BodyLifecyclePhaseList",
                column: "LifecyclePhaseName")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_BodyLifecyclePhaseList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "BodyLifecyclePhaseList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_BodyLifecyclePhaseList_ValidTo",
                schema: "OrganisationRegistry",
                table: "BodyLifecyclePhaseList",
                column: "ValidTo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BodyLifecyclePhaseList",
                schema: "OrganisationRegistry");
        }
    }
}
