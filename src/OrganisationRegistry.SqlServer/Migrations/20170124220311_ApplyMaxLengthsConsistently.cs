using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class ApplyMaxLengthsConsistently : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PersonFunctionList_OrganisationName",
                schema: "OrganisationRegistry",
                table: "PersonFunctionList");

            migrationBuilder.AlterColumn<string>(
                name: "OrganisationName",
                schema: "OrganisationRegistry",
                table: "PersonFunctionList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_PersonFunctionList_OrganisationName",
                schema: "OrganisationRegistry",
                table: "PersonFunctionList",
                column: "OrganisationName")
                .Annotation("SqlServer:Clustered", true);


            migrationBuilder.DropIndex(
                name: "IX_PersonCapacityList_OrganisationName",
                schema: "OrganisationRegistry",
                table: "PersonCapacityList");

            migrationBuilder.AlterColumn<string>(
                name: "OrganisationName",
                schema: "OrganisationRegistry",
                table: "PersonCapacityList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_PersonCapacityList_OrganisationName",
                schema: "OrganisationRegistry",
                table: "PersonCapacityList",
                column: "OrganisationName")
                .Annotation("SqlServer:Clustered", true);


            migrationBuilder.DropIndex(
                name: "IX_OrganisationLocationList_LocationName",
                schema: "OrganisationRegistry",
                table: "OrganisationLocationList");

            migrationBuilder.AlterColumn<string>(
                name: "LocationName",
                schema: "OrganisationRegistry",
                table: "OrganisationLocationList",
                maxLength: 460,
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationLocationList_LocationName",
                schema: "OrganisationRegistry",
                table: "OrganisationLocationList",
                column: "LocationName")
                .Annotation("SqlServer:Clustered", true);


            migrationBuilder.DropIndex(
                name: "IX_OrganisationFunctionList_PersonName",
                schema: "OrganisationRegistry",
                table: "OrganisationFunctionList");

            migrationBuilder.AlterColumn<string>(
                name: "PersonName",
                schema: "OrganisationRegistry",
                table: "OrganisationFunctionList",
                maxLength: 401,
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationFunctionList_PersonName",
                schema: "OrganisationRegistry",
                table: "OrganisationFunctionList",
                column: "PersonName")
                .Annotation("SqlServer:Clustered", true);


            migrationBuilder.DropIndex(
                name: "IX_OrganisationCapacityList_PersonName",
                schema: "OrganisationRegistry",
                table: "OrganisationCapacityList");

            migrationBuilder.AlterColumn<string>(
                name: "PersonName",
                schema: "OrganisationRegistry",
                table: "OrganisationCapacityList",
                maxLength: 401,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationCapacityList_PersonName",
                schema: "OrganisationRegistry",
                table: "OrganisationCapacityList",
                column: "PersonName");


            migrationBuilder.DropIndex(
                name: "IX_LocationList_ZipCode",
                schema: "OrganisationRegistry",
                table: "LocationList");

            migrationBuilder.AlterColumn<string>(
                name: "ZipCode",
                schema: "OrganisationRegistry",
                table: "LocationList",
                maxLength: 50,
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_LocationList_ZipCode",
                schema: "OrganisationRegistry",
                table: "LocationList",
                column: "ZipCode");


            migrationBuilder.DropIndex(
                name: "IX_LocationList_Street",
                schema: "OrganisationRegistry",
                table: "LocationList");

            migrationBuilder.AlterColumn<string>(
                name: "Street",
                schema: "OrganisationRegistry",
                table: "LocationList",
                maxLength: 200,
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_LocationList_Street",
                schema: "OrganisationRegistry",
                table: "LocationList",
                column: "Street");


            migrationBuilder.DropIndex(
                name: "IX_LocationList_FormattedAddress",
                schema: "OrganisationRegistry",
                table: "LocationList");

            migrationBuilder.AlterColumn<string>(
                name: "FormattedAddress",
                schema: "OrganisationRegistry",
                table: "LocationList",
                maxLength: 460,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LocationList_FormattedAddress",
                schema: "OrganisationRegistry",
                table: "LocationList",
                column: "FormattedAddress")
                .Annotation("SqlServer:Clustered", true);


            migrationBuilder.DropIndex(
                name: "IX_LocationList_Country",
                schema: "OrganisationRegistry",
                table: "LocationList");

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                schema: "OrganisationRegistry",
                table: "LocationList",
                maxLength: 100,
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_LocationList_Country",
                schema: "OrganisationRegistry",
                table: "LocationList",
                column: "Country");


            migrationBuilder.DropIndex(
                name: "IX_LocationList_City",
                schema: "OrganisationRegistry",
                table: "LocationList");

            migrationBuilder.AlterColumn<string>(
                name: "City",
                schema: "OrganisationRegistry",
                table: "LocationList",
                maxLength: 100,
                nullable: false);

            migrationBuilder.CreateIndex(
                name: "IX_LocationList_City",
                schema: "OrganisationRegistry",
                table: "LocationList",
                column: "City");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "OrganisationName",
                schema: "OrganisationRegistry",
                table: "PersonFunctionList",
                maxLength: 2000,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "OrganisationName",
                schema: "OrganisationRegistry",
                table: "PersonCapacityList",
                maxLength: 2000,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "LocationName",
                schema: "OrganisationRegistry",
                table: "OrganisationLocationList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "PersonName",
                schema: "OrganisationRegistry",
                table: "OrganisationFunctionList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "PersonName",
                schema: "OrganisationRegistry",
                table: "OrganisationCapacityList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ZipCode",
                schema: "OrganisationRegistry",
                table: "LocationList",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Street",
                schema: "OrganisationRegistry",
                table: "LocationList",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "FormattedAddress",
                schema: "OrganisationRegistry",
                table: "LocationList",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                schema: "OrganisationRegistry",
                table: "LocationList",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                schema: "OrganisationRegistry",
                table: "LocationList",
                nullable: false);
        }
    }
}
