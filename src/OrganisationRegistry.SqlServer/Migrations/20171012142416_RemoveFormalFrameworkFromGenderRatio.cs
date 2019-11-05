using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class RemoveFormalFrameworkFromGenderRatio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BodySeatGenderRatio_OrganisationPerBodyList",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio_OrganisationPerBodyList");

            migrationBuilder.DropColumn(
                name: "FormalFrameworkActive",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioList");

            migrationBuilder.DropColumn(
                name: "FormalFrameworkId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioList");

            migrationBuilder.DropColumn(
                name: "FormalFrameworkName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioList");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio_OrganisationPerBodyList",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BodySeatGenderRatio_OrganisationPerBodyList",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio_OrganisationPerBodyList",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BodySeatGenderRatio_OrganisationPerBodyList",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio_OrganisationPerBodyList");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio_OrganisationPerBodyList");

            migrationBuilder.AddColumn<bool>(
                name: "FormalFrameworkActive",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioList",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "FormalFrameworkId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioList",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FormalFrameworkName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BodySeatGenderRatio_OrganisationPerBodyList",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio_OrganisationPerBodyList",
                column: "BodyId")
                .Annotation("SqlServer:Clustered", false);
        }
    }
}
