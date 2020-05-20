using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddIsEffectiveColumnToReportProjections : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BodySeatTypeIsEffective",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioPostsPerTypeList",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "BodySeatTypeIsEffective",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioBodyMandateList",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodySeatTypeIsEffective",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioPostsPerTypeList");

            migrationBuilder.DropColumn(
                name: "BodySeatTypeIsEffective",
                schema: "OrganisationRegistry",
                table: "BodySeatGenderRatioBodyMandateList");
        }
    }
}
