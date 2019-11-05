using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddFormalFrameworkProjections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FormalFrameworkList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(maxLength: 50, nullable: true),
                    FormalFrameworkCategoryId = table.Column<Guid>(nullable: false),
                    FormalFrameworkCategoryName = table.Column<string>(maxLength: 500, nullable: true),
                    Name = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FormalFrameworkList", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FormalFrameworkList",
                schema: "OrganisationRegistry");
        }
    }
}
