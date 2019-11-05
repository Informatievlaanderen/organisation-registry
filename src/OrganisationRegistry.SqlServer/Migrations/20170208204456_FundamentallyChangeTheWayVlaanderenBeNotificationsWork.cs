using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class FundamentallyChangeTheWayVlaanderenBeNotificationsWork : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_VlaanderenBeNotifications",
                schema: "OrganisationRegistry",
                table: "VlaanderenBeNotifications");

            migrationBuilder.DropColumn(
                name: "Event",
                schema: "OrganisationRegistry",
                table: "VlaanderenBeNotifications");

            migrationBuilder.DropColumn(
                name: "OrganisationId",
                schema: "OrganisationRegistry",
                table: "VlaanderenBeNotifications");

            migrationBuilder.DropColumn(
                name: "OrganisationName",
                schema: "OrganisationRegistry",
                table: "VlaanderenBeNotifications");

            migrationBuilder.AddColumn<string>(
                name: "Body",
                schema: "OrganisationRegistry",
                table: "VlaanderenBeNotifications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Subject",
                schema: "OrganisationRegistry",
                table: "VlaanderenBeNotifications",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "To",
                schema: "OrganisationRegistry",
                table: "VlaanderenBeNotifications",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_VlaanderenBeNotificationsQueue",
                schema: "OrganisationRegistry",
                table: "VlaanderenBeNotifications",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.RenameTable(
                name: "VlaanderenBeNotifications",
                schema: "OrganisationRegistry",
                newName: "VlaanderenBeNotificationsQueue");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_VlaanderenBeNotificationsQueue",
                schema: "OrganisationRegistry",
                table: "VlaanderenBeNotificationsQueue");

            migrationBuilder.DropColumn(
                name: "Body",
                schema: "OrganisationRegistry",
                table: "VlaanderenBeNotificationsQueue");

            migrationBuilder.DropColumn(
                name: "Subject",
                schema: "OrganisationRegistry",
                table: "VlaanderenBeNotificationsQueue");

            migrationBuilder.DropColumn(
                name: "To",
                schema: "OrganisationRegistry",
                table: "VlaanderenBeNotificationsQueue");

            migrationBuilder.AddColumn<string>(
                name: "Event",
                schema: "OrganisationRegistry",
                table: "VlaanderenBeNotificationsQueue",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OrganisationId",
                schema: "OrganisationRegistry",
                table: "VlaanderenBeNotificationsQueue",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "OrganisationName",
                schema: "OrganisationRegistry",
                table: "VlaanderenBeNotificationsQueue",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_VlaanderenBeNotifications",
                schema: "OrganisationRegistry",
                table: "VlaanderenBeNotificationsQueue",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.RenameTable(
                name: "VlaanderenBeNotificationsQueue",
                schema: "OrganisationRegistry",
                newName: "VlaanderenBeNotifications");
        }
    }
}
