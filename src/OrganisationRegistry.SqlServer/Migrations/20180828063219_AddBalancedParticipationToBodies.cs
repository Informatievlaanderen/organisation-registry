using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddBalancedParticipationToBodies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BalancedParticipationExceptionMeasure",
                schema: "OrganisationRegistry",
                table: "BodyDetail",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BalancedParticipationExtraRemark",
                schema: "OrganisationRegistry",
                table: "BodyDetail",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsBalancedParticipationObligatory",
                schema: "OrganisationRegistry",
                table: "BodyDetail",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BalancedParticipationExceptionMeasure",
                schema: "OrganisationRegistry",
                table: "BodyDetail");

            migrationBuilder.DropColumn(
                name: "BalancedParticipationExtraRemark",
                schema: "OrganisationRegistry",
                table: "BodyDetail");

            migrationBuilder.DropColumn(
                name: "IsBalancedParticipationObligatory",
                schema: "OrganisationRegistry",
                table: "BodyDetail");
        }
    }
}
