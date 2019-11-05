using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddBodySeatGenderRatio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BodySeatGenderRatio",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BodyId = table.Column<Guid>(nullable: false),
                    BodyName = table.Column<string>(maxLength: 500, nullable: false),
                    BodyNumber = table.Column<string>(maxLength: 10, nullable: true),
                    BodySeatId = table.Column<Guid>(nullable: false),
                    BodySeatNumber = table.Column<string>(maxLength: 10, nullable: true),
                    BodySeatTypeId = table.Column<Guid>(nullable: false),
                    BodySeatTypeName = table.Column<string>(maxLength: 500, nullable: false),
                    BodyShortName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodySeatGenderRatio", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BodySeatGenderRatio",
                schema: "OrganisationRegistry");
        }
    }
}
