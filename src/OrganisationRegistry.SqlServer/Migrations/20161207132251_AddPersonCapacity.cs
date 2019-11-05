using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddPersonCapacity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PersonCapacityList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationCapacityId = table.Column<Guid>(nullable: false),
                    CapacityId = table.Column<Guid>(nullable: false),
                    CapacityName = table.Column<string>(maxLength: 500, nullable: true),
                    FunctionId = table.Column<Guid>(nullable: true),
                    FunctionName = table.Column<string>(maxLength: 500, nullable: true),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    OrganisationName = table.Column<string>(maxLength: 2000, nullable: true),
                    PersonId = table.Column<Guid>(nullable: true),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonCapacityList", x => x.OrganisationCapacityId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PersonCapacityList",
                schema: "OrganisationRegistry");
        }
    }
}
