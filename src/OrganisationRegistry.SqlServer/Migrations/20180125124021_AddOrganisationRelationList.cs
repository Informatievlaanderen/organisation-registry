using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddOrganisationRelationList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganisationRelationList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationRelationId = table.Column<Guid>(nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    RelationId = table.Column<Guid>(nullable: false),
                    RelationName = table.Column<string>(maxLength: 500, nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationRelationList", x => x.OrganisationRelationId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationRelationList_RelationName",
                schema: "OrganisationRegistry",
                table: "OrganisationRelationList",
                column: "RelationName");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationRelationList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationRelationList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationRelationList_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationRelationList",
                column: "ValidTo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganisationRelationList",
                schema: "OrganisationRegistry");
        }
    }
}
