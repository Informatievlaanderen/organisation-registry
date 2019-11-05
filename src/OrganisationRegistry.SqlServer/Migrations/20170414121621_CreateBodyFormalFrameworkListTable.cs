using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class CreateBodyFormalFrameworkListTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BodyFormalFrameworkList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    BodyFormalFrameworkId = table.Column<Guid>(nullable: false),
                    BodyId = table.Column<Guid>(nullable: false),
                    FormalFrameworkId = table.Column<Guid>(nullable: false),
                    FormalFrameworkName = table.Column<string>(maxLength: 500, nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyFormalFrameworkList", x => x.BodyFormalFrameworkId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BodyFormalFrameworkList_FormalFrameworkName",
                schema: "OrganisationRegistry",
                table: "BodyFormalFrameworkList",
                column: "FormalFrameworkName")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_BodyFormalFrameworkList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "BodyFormalFrameworkList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_BodyFormalFrameworkList_ValidTo",
                schema: "OrganisationRegistry",
                table: "BodyFormalFrameworkList",
                column: "ValidTo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BodyFormalFrameworkList",
                schema: "OrganisationRegistry");
        }
    }
}
