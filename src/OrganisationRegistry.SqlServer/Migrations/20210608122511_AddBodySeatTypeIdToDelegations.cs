using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddBodySeatTypeIdToDelegations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "BodySeatTypeId",
                schema: "OrganisationRegistry",
                table: "DelegationList",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DelegationList_BodySeatTypeId",
                schema: "OrganisationRegistry",
                table: "DelegationList",
                column: "BodySeatTypeId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DelegationList_BodySeatTypeId",
                schema: "OrganisationRegistry",
                table: "DelegationList");

            migrationBuilder.DropColumn(
                name: "BodySeatTypeId",
                schema: "OrganisationRegistry",
                table: "DelegationList");
        }
    }
}
