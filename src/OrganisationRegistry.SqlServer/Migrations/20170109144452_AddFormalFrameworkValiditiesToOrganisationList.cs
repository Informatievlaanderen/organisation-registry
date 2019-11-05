using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddFormalFrameworkValiditiesToOrganisationList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FormalFrameworkValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.DropColumn(
                name: "FormalFrameworkValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.CreateTable(
                name: "OrganisationFormalFrameworkValidity",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    OrganisationFormalFrameworkId = table.Column<Guid>(nullable: false),
                    OrganisationListItemId = table.Column<int>(nullable: true),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationFormalFrameworkValidity", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_OrganisationFormalFrameworkValidity_OrganisationList_OrganisationListItemId",
                        column: x => x.OrganisationListItemId,
                        principalSchema: "OrganisationRegistry",
                        principalTable: "OrganisationList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationFormalFrameworkValidity_OrganisationListItemId",
                schema: "OrganisationRegistry",
                table: "OrganisationFormalFrameworkValidity",
                column: "OrganisationListItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganisationFormalFrameworkValidity",
                schema: "OrganisationRegistry");

            migrationBuilder.AddColumn<DateTime>(
                name: "FormalFrameworkValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FormalFrameworkValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                nullable: true);
        }
    }
}
