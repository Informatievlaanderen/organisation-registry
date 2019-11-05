using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddValidityToBodySeatOrganisationClassification : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BodySeatGenderRatioOrganisationClassificationList",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioOrganisationClassificationList");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioOrganisationClassificationList",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "ClassificationValidFrom",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioOrganisationClassificationList",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ClassificationValidTo",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioOrganisationClassificationList",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BodySeatGenderRatioOrganisationClassificationList",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioOrganisationClassificationList",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BodySeatGenderRatioOrganisationClassificationList",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioOrganisationClassificationList");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioOrganisationClassificationList");

            migrationBuilder.DropColumn(
                name: "ClassificationValidFrom",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioOrganisationClassificationList");

            migrationBuilder.DropColumn(
                name: "ClassificationValidTo",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioOrganisationClassificationList");

            migrationBuilder.AddPrimaryKey(
                name: "PK_BodySeatGenderRatioOrganisationClassificationList",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioOrganisationClassificationList",
                column: "OrganisationClassificationId")
                .Annotation("SqlServer:Clustered", false);
        }
    }
}
