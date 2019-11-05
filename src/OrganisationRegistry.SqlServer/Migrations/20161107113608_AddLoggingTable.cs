using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddLoggingTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logs",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Logged = table.Column<DateTime>(nullable: false),
                    Level = table.Column<string>(maxLength: 50, nullable: true),
                    Message = table.Column<string>(nullable: true),
                    Exception = table.Column<string>(nullable: true),
                    Callsite = table.Column<string>(nullable: true),
                    Logger = table.Column<string>(maxLength: 250, nullable: true),
                    Application = table.Column<string>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs",
                schema: "OrganisationRegistry");
        }
    }
}
