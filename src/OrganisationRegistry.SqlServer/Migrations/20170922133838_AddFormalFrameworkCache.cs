using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddFormalFrameworkCache : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BodySeatGenderRatio_FormalFrameworkPerBodyList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    BodyId = table.Column<Guid>(nullable: false),
                    FormalFrameworkId = table.Column<Guid>(nullable: false),
                    FormalFrameworkName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodySeatGenderRatio_FormalFrameworkPerBodyList", x => x.BodyId)
                        .Annotation("SqlServer:Clustered", false);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BodySeatGenderRatio_FormalFrameworkPerBodyList",
                schema: "OrganisationRegistry");
        }
    }
}
