using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class RemoveOIDCTokenEndPointFromDatabase : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(DeleteSetting("OIDCAuth:TokenEndPoint"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(InsertSetting("OIDCAuth:TokenEndPoint", "Token endPoint bij Authority", "/v1/token"));
        }
    }
}
