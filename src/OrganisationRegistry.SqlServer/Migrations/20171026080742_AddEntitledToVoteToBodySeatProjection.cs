using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddEntitledToVoteToBodySeatProjection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EntitledToVote",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioList",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EntitledToVote",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioList");
        }
    }
}
