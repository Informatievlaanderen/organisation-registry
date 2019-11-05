using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class BodySeatGenderRatioPersonCache : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BodySeatGenderRatio_PersonList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    PersonId = table.Column<Guid>(nullable: false),
                    PersonFullName = table.Column<string>(maxLength: 401, nullable: true),
                    PersonSex = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodySeatGenderRatio_PersonList", x => x.PersonId)
                        .Annotation("SqlServer:Clustered", false);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BodySeatGenderRatio_PersonList",
                schema: "OrganisationRegistry");
        }
    }
}
