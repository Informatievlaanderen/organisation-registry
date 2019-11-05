using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddBodyBodyClassificationProjections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BodyBodyClassificationList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    BodyBodyClassificationId = table.Column<Guid>(nullable: false),
                    BodyId = table.Column<Guid>(nullable: false),
                    BodyClassificationTypeId = table.Column<Guid>(nullable: false),
                    BodyClassificationTypeName = table.Column<string>(maxLength: 500, nullable: false),
                    BodyClassificationId = table.Column<Guid>(nullable: false),
                    BodyClassificationName = table.Column<string>(maxLength: 500, nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BodyBodyClassificationList", x => x.BodyBodyClassificationId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BodyBodyClassificationList_BodyClassificationName",
                schema: "OrganisationRegistry",
                table: "BodyBodyClassificationList",
                column: "BodyClassificationName");

            migrationBuilder.CreateIndex(
                name: "IX_BodyBodyClassificationList_BodyClassificationTypeName",
                schema: "OrganisationRegistry",
                table: "BodyBodyClassificationList",
                column: "BodyClassificationTypeName")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_BodyBodyClassificationList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "BodyBodyClassificationList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_BodyBodyClassificationList_ValidTo",
                schema: "OrganisationRegistry",
                table: "BodyBodyClassificationList",
                column: "ValidTo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BodyBodyClassificationList",
                schema: "OrganisationRegistry");
        }
    }
}
