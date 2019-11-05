using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class ModifyOrganisationPerBodyCache : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BodySeatGenderRatio_OrganisationPerBodyList",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio_OrganisationPerBodyList");

            migrationBuilder.DropColumn(
                name: "Id",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio_OrganisationPerBodyList");

            migrationBuilder.AddColumn<Guid>(
                name: "BodyOrganisationId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio_OrganisationPerBodyList",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "OrganisationActive",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio_OrganisationPerBodyList",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_BodySeatGenderRatio_OrganisationPerBodyList",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio_OrganisationPerBodyList",
                column: "BodyId")
                .Annotation("SqlServer:Clustered", false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_BodySeatGenderRatio_OrganisationPerBodyList",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio_OrganisationPerBodyList");

            migrationBuilder.DropColumn(
                name: "BodyOrganisationId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio_OrganisationPerBodyList");

            migrationBuilder.DropColumn(
                name: "OrganisationActive",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio_OrganisationPerBodyList");

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
    }
}
