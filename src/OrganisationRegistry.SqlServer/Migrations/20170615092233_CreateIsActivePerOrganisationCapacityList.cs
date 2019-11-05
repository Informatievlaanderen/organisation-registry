using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class CreateIsActivePerOrganisationCapacityList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IsActivePerOrganisationCapacityList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationCapacityId = table.Column<Guid>(nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IsActivePerOrganisationCapacityList", x => x.OrganisationCapacityId)
                        .Annotation("SqlServer:Clustered", false);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IsActivePerOrganisationCapacityList",
                schema: "OrganisationRegistry");
        }
    }
}
