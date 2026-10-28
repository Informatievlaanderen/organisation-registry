using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrganisationRegistry.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddFunctionTypeRemovalColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ScheduledForRemoval",
                schema: "Backoffice",
                table: "OrganisationFunctionList",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRemoved",
                schema: "Backoffice",
                table: "FunctionList",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ScheduledForRemoval",
                schema: "Backoffice",
                table: "BodyMandateList",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScheduledForRemoval",
                schema: "Backoffice",
                table: "OrganisationFunctionList");

            migrationBuilder.DropColumn(
                name: "IsRemoved",
                schema: "Backoffice",
                table: "FunctionList");

            migrationBuilder.DropColumn(
                name: "ScheduledForRemoval",
                schema: "Backoffice",
                table: "BodyMandateList");
        }
    }
}
