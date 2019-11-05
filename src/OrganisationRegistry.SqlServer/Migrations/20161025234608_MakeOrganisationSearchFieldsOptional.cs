using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class MakeOrganisationSearchFieldsOptional : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ParentOrganisationId",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "OrganisationClassificationTypeId",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "OrganisationClassificationId",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "FormalFrameworkId",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "ParentOrganisationId",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                nullable: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "OrganisationClassificationTypeId",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                nullable: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "OrganisationClassificationId",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                nullable: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "FormalFrameworkId",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                nullable: false);
        }
    }
}
