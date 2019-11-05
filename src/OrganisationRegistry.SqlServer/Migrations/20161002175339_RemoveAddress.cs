namespace OrganisationRegistry.SqlServer.Migrations
{
    using System;
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class RemoveAddress : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Address",
                schema: "OrganisationRegistry");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Address",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AddressLine = table.Column<string>(maxLength: 500, nullable: true),
                    CountryCode = table.Column<string>(maxLength: 3, nullable: true),
                    CrabId = table.Column<int>(nullable: true),
                    Gemeente = table.Column<string>(maxLength: 100, nullable: true),
                    PostCode = table.Column<string>(maxLength: 20, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Address", x => x.Id);
                });
        }
    }
}
