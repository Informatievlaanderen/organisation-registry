using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddBodyClassificationProjections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BodyClassificationList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 500, nullable: false),
                    Order = table.Column<int>(nullable: false),
                    Active = table.Column<bool>(nullable: false),
                    BodyClassificationTypeId = table.Column<Guid>(nullable: false),
                    BodyClassificationTypeName = table.Column<string>(maxLength: 500, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyClassificationList", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BodyClassificationList_Active",
                schema: "OrganisationRegistry",
                table: "BodyClassificationList",
                column: "Active");

            migrationBuilder.CreateIndex(
                name: "IX_BodyClassificationList_BodyClassificationTypeName",
                schema: "OrganisationRegistry",
                table: "BodyClassificationList",
                column: "BodyClassificationTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_BodyClassificationList_Name",
                schema: "OrganisationRegistry",
                table: "BodyClassificationList",
                column: "Name")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_BodyClassificationList_Order",
                schema: "OrganisationRegistry",
                table: "BodyClassificationList",
                column: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_BodyClassificationList_Name_BodyClassificationTypeId",
                schema: "OrganisationRegistry",
                table: "BodyClassificationList",
                columns: new[] { "Name", "BodyClassificationTypeId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BodyClassificationList",
                schema: "OrganisationRegistry");
        }
    }
}
