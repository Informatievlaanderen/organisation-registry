using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class UseBodyAsTopEntityForBodySeatGenderRatioProjection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrganisationId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioPostsPerTypeList");

            migrationBuilder.DropColumn(
                name: "OrganisationIsActive",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioPostsPerTypeList");

            migrationBuilder.DropColumn(
                name: "OrganisationName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioPostsPerTypeList");

            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioBodyList",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OrganisationIsActive",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioBodyList",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OrganisationName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioBodyList",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrganisationId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioBodyList");

            migrationBuilder.DropColumn(
                name: "OrganisationIsActive",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioBodyList");

            migrationBuilder.DropColumn(
                name: "OrganisationName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioBodyList");

            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioPostsPerTypeList",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "OrganisationIsActive",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioPostsPerTypeList",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "OrganisationName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioPostsPerTypeList",
                maxLength: 500,
                nullable: true);
        }
    }
}
