using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class UseKeyTypeInOrganisationKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn("KeyId", "OrganisationKeyList", "KeyTypeId", "OrganisationRegistry");
            migrationBuilder.RenameColumn("KeyName", "OrganisationKeyList", "KeyTypeName", "OrganisationRegistry");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn("KeyTypeId", "OrganisationKeyList", "KeyId", "OrganisationRegistry");
            migrationBuilder.RenameColumn("KeyTypeName", "OrganisationKeyList", "KeyName", "OrganisationRegistry");
        }
    }
}
