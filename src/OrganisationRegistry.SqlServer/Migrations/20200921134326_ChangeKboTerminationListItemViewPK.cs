using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class ChangeKboTerminationListItemViewPK : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationTerminationList",
                schema: "OrganisationRegistry",
                table: "OrganisationTerminationList");

            migrationBuilder.AlterColumn<string>(
                name: "KboNumber",
                schema: "OrganisationRegistry",
                table: "OrganisationTerminationList",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationTerminationList",
                schema: "OrganisationRegistry",
                table: "OrganisationTerminationList",
                columns: new[] { "Id", "KboNumber" })
                .Annotation("SqlServer:Clustered", false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationTerminationList",
                schema: "OrganisationRegistry",
                table: "OrganisationTerminationList");

            migrationBuilder.AlterColumn<string>(
                name: "KboNumber",
                schema: "OrganisationRegistry",
                table: "OrganisationTerminationList",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationTerminationList",
                schema: "OrganisationRegistry",
                table: "OrganisationTerminationList",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);
        }
    }
}
