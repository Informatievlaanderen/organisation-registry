using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class CreateDelegationListProjection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DelegationList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    BodyId = table.Column<Guid>(nullable: false),
                    BodyName = table.Column<string>(maxLength: 500, nullable: true),
                    BodySeatId = table.Column<Guid>(nullable: false),
                    BodySeatName = table.Column<string>(maxLength: 500, nullable: true),
                    BodySeatNumber = table.Column<string>(maxLength: 10, nullable: true),
                    FunctionTypeId = table.Column<Guid>(nullable: true),
                    FunctionTypeName = table.Column<string>(maxLength: 500, nullable: true),
                    IsDelegated = table.Column<bool>(nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    OrganisationName = table.Column<string>(maxLength: 500, nullable: true),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DelegationList", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DelegationList_BodyName",
                schema: "OrganisationRegistry",
                table: "DelegationList",
                column: "BodyName");

            migrationBuilder.CreateIndex(
                name: "IX_DelegationList_BodySeatName",
                schema: "OrganisationRegistry",
                table: "DelegationList",
                column: "BodySeatName");

            migrationBuilder.CreateIndex(
                name: "IX_DelegationList_BodySeatNumber",
                schema: "OrganisationRegistry",
                table: "DelegationList",
                column: "BodySeatNumber");

            migrationBuilder.CreateIndex(
                name: "IX_DelegationList_IsDelegated",
                schema: "OrganisationRegistry",
                table: "DelegationList",
                column: "IsDelegated");

            migrationBuilder.CreateIndex(
                name: "IX_DelegationList_OrganisationName",
                schema: "OrganisationRegistry",
                table: "DelegationList",
                column: "OrganisationName")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_DelegationList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "DelegationList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_DelegationList_ValidTo",
                schema: "OrganisationRegistry",
                table: "DelegationList",
                column: "ValidTo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DelegationList",
                schema: "OrganisationRegistry");
        }
    }
}
