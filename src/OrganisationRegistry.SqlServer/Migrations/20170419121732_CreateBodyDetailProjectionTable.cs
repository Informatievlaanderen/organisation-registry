using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class CreateBodyDetailProjectionTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BodyDetail",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    FormalValidFrom = table.Column<DateTime>(nullable: true),
                    FormalValidTo = table.Column<DateTime>(nullable: true),
                    Name = table.Column<string>(maxLength: 500, nullable: false),
                    Organisation = table.Column<string>(maxLength: 500, nullable: true),
                    OrganisationId = table.Column<Guid>(nullable: true),
                    ShortName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyDetail", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BodyDetail_FormalValidFrom",
                schema: "OrganisationRegistry",
                table: "BodyDetail",
                column: "FormalValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_BodyDetail_FormalValidTo",
                schema: "OrganisationRegistry",
                table: "BodyDetail",
                column: "FormalValidTo");

            migrationBuilder.CreateIndex(
                name: "IX_BodyDetail_Name",
                schema: "OrganisationRegistry",
                table: "BodyDetail",
                column: "Name")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_BodyDetail_Organisation",
                schema: "OrganisationRegistry",
                table: "BodyDetail",
                column: "Organisation");

            migrationBuilder.CreateIndex(
                name: "IX_BodyDetail_ShortName",
                schema: "OrganisationRegistry",
                table: "BodyDetail",
                column: "ShortName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BodyDetail",
                schema: "OrganisationRegistry");
        }
    }
}
