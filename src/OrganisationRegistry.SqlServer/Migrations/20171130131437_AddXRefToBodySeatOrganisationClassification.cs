using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddXRefToBodySeatOrganisationClassification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioOrganisationClassificationList",
                newName: "OrganisationOrganisationClassificationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OrganisationOrganisationClassificationId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioOrganisationClassificationList",
                newName: "Id");
        }
    }
}
