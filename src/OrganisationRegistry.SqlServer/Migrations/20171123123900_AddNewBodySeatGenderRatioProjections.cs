using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddNewBodySeatGenderRatioProjections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BodySeatGenderRatioBodyMandateList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    BodyMandateId = table.Column<Guid>(nullable: false),
                    BodyId = table.Column<Guid>(nullable: false),
                    BodyMandateValidFrom = table.Column<DateTime>(nullable: true),
                    BodyMandateValidTo = table.Column<DateTime>(nullable: true),
                    BodySeatTypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodySeatGenderRatioBodyMandateList", x => x.BodyMandateId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "BodySeatGenderRatioPostsPerTypeList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BodyId = table.Column<Guid>(nullable: false),
                    BodyName = table.Column<string>(maxLength: 500, nullable: true),
                    BodySeatId = table.Column<Guid>(nullable: false),
                    BodySeatTypeId = table.Column<Guid>(nullable: false),
                    BodySeatTypeName = table.Column<string>(maxLength: 500, nullable: true),
                    BodySeatValidFrom = table.Column<DateTime>(nullable: true),
                    BodySeatValidTo = table.Column<DateTime>(nullable: true),
                    BodyValidFrom = table.Column<DateTime>(nullable: true),
                    BodyValidTo = table.Column<DateTime>(nullable: true),
                    EntitledToVote = table.Column<bool>(nullable: false, defaultValue: false),
                    OrganisationId = table.Column<Guid>(nullable: true),
                    OrganisationName = table.Column<string>(maxLength: 500, nullable: true),
                    OrganisationValidFrom = table.Column<DateTime>(nullable: true),
                    OrganisationValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodySeatGenderRatioPostsPerTypeList", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "BodySeatGenderRatioAssignmentList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AssignmentValidFrom = table.Column<DateTime>(nullable: true),
                    AssignmentValidTo = table.Column<DateTime>(nullable: true),
                    BodyMandateId = table.Column<Guid>(nullable: false),
                    DelegationAssignmentId = table.Column<Guid>(nullable: true),
                    PersonId = table.Column<Guid>(nullable: false),
                    Sex = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodySeatGenderRatioAssignmentList", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_BodySeatGenderRatioAssignmentList_BodySeatGenderRatioBodyMandateList_BodyMandateId",
                        column: x => x.BodyMandateId,
                        principalSchema: "OrganisationRegistry",
                        principalTable: "BodySeatGenderRatioBodyMandateList",
                        principalColumn: "BodyMandateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BodySeatGenderRatioAssignmentList_BodyMandateId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioAssignmentList",
                column: "BodyMandateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BodySeatGenderRatioAssignmentList",
                schema: "OrganisationRegistry");

            migrationBuilder.DropTable(
                name: "BodySeatGenderRatioPostsPerTypeList",
                schema: "OrganisationRegistry");

            migrationBuilder.DropTable(
                name: "BodySeatGenderRatioBodyMandateList",
                schema: "OrganisationRegistry");
        }
    }
}
