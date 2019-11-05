using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddCapacityPersonList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CapacityPersonList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CapacityId = table.Column<Guid>(nullable: true),
                    CapacityName = table.Column<string>(maxLength: 500, nullable: true),
                    FunctionId = table.Column<Guid>(nullable: true),
                    FunctionName = table.Column<string>(maxLength: 500, nullable: true),
                    LocationFormattedAddress = table.Column<string>(maxLength: 460, nullable: true),
                    LocationId = table.Column<Guid>(nullable: true),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    OrganisationName = table.Column<string>(maxLength: 500, nullable: false),
                    OrganisationShortName = table.Column<string>(nullable: true),
                    PersonId = table.Column<Guid>(nullable: true),
                    PersonName = table.Column<string>(maxLength: 401, nullable: false),
                    PurposeId = table.Column<Guid>(nullable: true),
                    PurposeName = table.Column<string>(maxLength: 500, nullable: true),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CapacityPersonList", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CapacityPersonList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "CapacityPersonList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_CapacityPersonList_ValidTo",
                schema: "OrganisationRegistry",
                table: "CapacityPersonList",
                column: "ValidTo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CapacityPersonList",
                schema: "OrganisationRegistry");
        }
    }
}
