using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class RenameUserToUserClaims : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "User",
                schema: "Magda",
                table: "CallReferences",
                newName: "UserClaims");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UserClaims",
                schema: "Magda",
                table: "CallReferences",
                newName: "User");
        }
    }
}
