using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddBodyNumberColumnToBodyDetailView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BodyNumber",
                schema: "OrganisationRegistry",
                table: "BodyDetail",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

//            // This will be added again later
//            migrationBuilder.CreateIndex(
//                name: "IX_BodyDetail_BodyNumber",
//                schema: "OrganisationRegistry",
//                table: "BodyDetail",
//                column: "BodyNumber",
//                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
//            // This will be added again later
//            migrationBuilder.DropIndex(
//                name: "IX_BodyDetail_BodyNumber",
//                schema: "OrganisationRegistry",
//                table: "BodyDetail");

            migrationBuilder.DropColumn(
                name: "BodyNumber",
                schema: "OrganisationRegistry",
                table: "BodyDetail");
        }
    }
}
