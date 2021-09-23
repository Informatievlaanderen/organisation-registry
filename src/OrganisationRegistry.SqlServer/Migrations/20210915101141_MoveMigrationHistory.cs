using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class MoveMigrationHistory : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("IF (EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'OrganisationRegistry'" +
                                 "AND  TABLE_NAME = '__EFMigrationsHistory'))\n" +
                                 "ALTER SCHEMA Backoffice TRANSFER OrganisationRegistry.__EFMigrationsHistory");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("ALTER SCHEMA OrganisationRegistry TRANSFER Backoffice.__EFMigrationsHistory");
        }
    }
}
