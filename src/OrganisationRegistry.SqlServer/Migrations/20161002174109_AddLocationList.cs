namespace OrganisationRegistry.SqlServer.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AddLocationList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LocationList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AddressLine = table.Column<string>(maxLength: 500, nullable: true),
                    City = table.Column<string>(maxLength: 100, nullable: true),
                    ZipCode = table.Column<string>(maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LocationList", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LocationList",
                schema: "OrganisationRegistry");
        }
    }
}
