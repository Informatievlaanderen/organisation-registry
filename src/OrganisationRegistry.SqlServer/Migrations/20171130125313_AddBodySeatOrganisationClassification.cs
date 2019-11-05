using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddBodySeatOrganisationClassification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BodySeatGenderRatioOrganisationClassificationList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationClassificationId = table.Column<Guid>(nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodySeatGenderRatioOrganisationClassificationList", x => x.OrganisationClassificationId)
                        .Annotation("SqlServer:Clustered", false);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BodySeatGenderRatioOrganisationClassificationList",
                schema: "OrganisationRegistry");
        }
    }
}
