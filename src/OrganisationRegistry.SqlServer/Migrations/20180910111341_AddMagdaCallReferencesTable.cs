using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddMagdaCallReferencesTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Magda");

            migrationBuilder.CreateTable(
                name: "CallReferences",
                schema: "Magda",
                columns: table => new
                {
                    Reference = table.Column<Guid>(nullable: false),
                    User = table.Column<string>(nullable: true),
                    CalledAt = table.Column<DateTimeOffset>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CallReferences", x => x.Reference);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CallReferences",
                schema: "Magda");
        }
    }
}
