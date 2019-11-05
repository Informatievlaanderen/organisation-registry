namespace OrganisationRegistry.SqlServer.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AddKeyPersonBuildingProjections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BuildingList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 500, nullable: true),
                    VimId = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BuildingList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "KeyList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KeyList", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PersonList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateOfBirth = table.Column<DateTime>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 100, nullable: true),
                    Gender = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PersonList", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BuildingList",
                schema: "OrganisationRegistry");

            migrationBuilder.DropTable(
                name: "KeyList",
                schema: "OrganisationRegistry");

            migrationBuilder.DropTable(
                name: "PersonList",
                schema: "OrganisationRegistry");
        }
    }
}
