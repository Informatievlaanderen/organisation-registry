using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddBodySeatTypeNameToDelegations : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BodySeatTypeName",
                schema: "OrganisationRegistry",
                table: "DelegationList",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DelegationList_BodySeatTypeName",
                schema: "OrganisationRegistry",
                table: "DelegationList",
                column: "BodySeatTypeName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_DelegationList_BodySeatTypeName",
                schema: "OrganisationRegistry",
                table: "DelegationList");

            migrationBuilder.DropColumn(
                name: "BodySeatTypeName",
                schema: "OrganisationRegistry",
                table: "DelegationList");
        }
    }
}
