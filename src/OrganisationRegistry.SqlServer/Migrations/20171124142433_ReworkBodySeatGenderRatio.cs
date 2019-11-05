using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class ReworkBodySeatGenderRatio : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BodySeatGenderRatio_FormalFrameworkPerBodyList",
                schema: "OrganisationRegistry");

            migrationBuilder.DropTable(
                name: "BodySeatGenderRatioList",
                schema: "OrganisationRegistry");

            migrationBuilder.DropTable(
                name: "BodySeatGenderRatio_OrganisationList",
                schema: "OrganisationRegistry");

            migrationBuilder.DropColumn(
                name: "PersonFullName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio_PersonList");

            migrationBuilder.DropColumn(
                name: "BodyName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioPostsPerTypeList");

            migrationBuilder.DropColumn(
                name: "BodyValidFrom",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioPostsPerTypeList");

            migrationBuilder.DropColumn(
                name: "BodyValidTo",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioPostsPerTypeList");

            migrationBuilder.DropColumn(
                name: "OrganisationValidFrom",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioPostsPerTypeList");

            migrationBuilder.DropColumn(
                name: "OrganisationValidTo",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioPostsPerTypeList");

            migrationBuilder.AddColumn<bool>(
                name: "OrganisationIsActive",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioPostsPerTypeList",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<Guid>(
                name: "BodySeatId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioBodyMandateList",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "BodySeatGenderRatioBodyList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    BodyId = table.Column<Guid>(nullable: false),
                    BodyName = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodySeatGenderRatioBodyList", x => x.BodyId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "BodySeatGenderRatioLifecyclePhaseValidityList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    LifecyclePhaseId = table.Column<Guid>(nullable: false),
                    BodyId = table.Column<Guid>(nullable: false),
                    RepresentsActivePhase = table.Column<bool>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodySeatGenderRatioLifecyclePhaseValidityList", x => x.LifecyclePhaseId)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_BodySeatGenderRatioLifecyclePhaseValidityList_BodySeatGenderRatioBodyList_BodyId",
                        column: x => x.BodyId,
                        principalSchema: "OrganisationRegistry",
                        principalTable: "BodySeatGenderRatioBodyList",
                        principalColumn: "BodyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BodySeatGenderRatioPostsPerTypeList_BodyId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioPostsPerTypeList",
                column: "BodyId");

            migrationBuilder.CreateIndex(
                name: "IX_BodySeatGenderRatioLifecyclePhaseValidityList_BodyId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioLifecyclePhaseValidityList",
                column: "BodyId");

            migrationBuilder.AddForeignKey(
                name: "FK_BodySeatGenderRatioPostsPerTypeList_BodySeatGenderRatioBodyList_BodyId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioPostsPerTypeList",
                column: "BodyId",
                principalSchema: "OrganisationRegistry",
                principalTable: "BodySeatGenderRatioBodyList",
                principalColumn: "BodyId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BodySeatGenderRatioPostsPerTypeList_BodySeatGenderRatioBodyList_BodyId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioPostsPerTypeList");

            migrationBuilder.DropTable(
                name: "BodySeatGenderRatioLifecyclePhaseValidityList",
                schema: "OrganisationRegistry");

            migrationBuilder.DropTable(
                name: "BodySeatGenderRatioBodyList",
                schema: "OrganisationRegistry");

            migrationBuilder.DropIndex(
                name: "IX_BodySeatGenderRatioPostsPerTypeList_BodyId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioPostsPerTypeList");

            migrationBuilder.DropColumn(
                name: "OrganisationIsActive",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioPostsPerTypeList");

            migrationBuilder.DropColumn(
                name: "BodySeatId",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioBodyMandateList");

            migrationBuilder.AddColumn<string>(
                name: "PersonFullName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatio_PersonList",
                maxLength: 401,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BodyName",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioPostsPerTypeList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BodyValidFrom",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioPostsPerTypeList",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "BodyValidTo",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioPostsPerTypeList",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OrganisationValidFrom",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioPostsPerTypeList",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OrganisationValidTo",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioPostsPerTypeList",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "BodySeatGenderRatio_FormalFrameworkPerBodyList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    BodyId = table.Column<Guid>(nullable: false),
                    FormalFrameworkActive = table.Column<bool>(nullable: false),
                    FormalFrameworkId = table.Column<Guid>(nullable: false),
                    FormalFrameworkName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodySeatGenderRatio_FormalFrameworkPerBodyList", x => x.BodyId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "BodySeatGenderRatioList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BodyActive = table.Column<bool>(nullable: false),
                    BodyId = table.Column<Guid>(nullable: false),
                    BodyMandateId = table.Column<Guid>(nullable: true),
                    BodyName = table.Column<string>(maxLength: 500, nullable: true),
                    BodySeatActive = table.Column<bool>(nullable: false),
                    BodySeatId = table.Column<Guid>(nullable: false),
                    BodySeatNumber = table.Column<string>(maxLength: 10, nullable: true),
                    BodySeatTypeId = table.Column<Guid>(nullable: false),
                    BodySeatTypeName = table.Column<string>(maxLength: 500, nullable: true),
                    EntitledToVote = table.Column<bool>(nullable: false, defaultValue: false),
                    IsAssigned = table.Column<bool>(nullable: false, defaultValue: false),
                    IsFunctionAssigned = table.Column<bool>(nullable: false, defaultValue: false),
                    IsOrganisationAssigned = table.Column<bool>(nullable: false, defaultValue: false),
                    IsPersonAssigned = table.Column<bool>(nullable: false, defaultValue: false),
                    OrganisationActive = table.Column<bool>(nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: true),
                    OrganisationName = table.Column<string>(maxLength: 500, nullable: true),
                    PersonFullName = table.Column<string>(maxLength: 401, nullable: true),
                    PersonId = table.Column<Guid>(nullable: true),
                    PersonSex = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodySeatGenderRatioList", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "BodySeatGenderRatio_OrganisationList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationId = table.Column<Guid>(nullable: false),
                    OrganisationActive = table.Column<bool>(nullable: false),
                    OrganisationName = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodySeatGenderRatio_OrganisationList", x => x.OrganisationId)
                        .Annotation("SqlServer:Clustered", false);
                });
        }
    }
}
