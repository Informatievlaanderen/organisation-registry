using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.VlaanderenBeNotifier.Schema.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "VlaanderenBeNotifier");

            migrationBuilder.CreateTable(
                name: "OrganisationCache",
                schema: "VlaanderenBeNotifier",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OvoNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationCache", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganisationCache",
                schema: "VlaanderenBeNotifier");
        }
    }
}
