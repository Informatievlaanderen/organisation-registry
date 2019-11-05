using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddOrganisationClassificationValidities : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganisationList_OrganisationClassificationId",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationList_OrganisationClassificationTypeId",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.DropColumn(
                name: "OrganisationClassificationId",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.DropColumn(
                name: "OrganisationClassificationTypeId",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.CreateTable(
                name: "OrganisationClassificationValidity",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrganisationClassificationId = table.Column<Guid>(nullable: false),
                    OrganisationClassificationTypeId = table.Column<Guid>(nullable: false),
                    OrganisationListItemId = table.Column<int>(nullable: true),
                    OrganisationOrganisationClassificationId = table.Column<Guid>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationClassificationValidity", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_OrganisationClassificationValidity_OrganisationList_OrganisationListItemId",
                        column: x => x.OrganisationListItemId,
                        principalSchema: "OrganisationRegistry",
                        principalTable: "OrganisationList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationClassificationValidity_OrganisationListItemId",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationValidity",
                column: "OrganisationListItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganisationClassificationValidity",
                schema: "OrganisationRegistry");

            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationClassificationId",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationClassificationTypeId",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationList_OrganisationClassificationId",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                column: "OrganisationClassificationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationList_OrganisationClassificationTypeId",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                column: "OrganisationClassificationTypeId");
        }
    }
}
