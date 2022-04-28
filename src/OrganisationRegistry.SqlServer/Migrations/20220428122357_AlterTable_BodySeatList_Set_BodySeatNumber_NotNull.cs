using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AlterTable_BodySeatList_Set_BodySeatNumber_NotNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BodySeatNumber",
                schema: "Backoffice",
                table: "BodySeatList",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "BodySeatNumber",
                schema: "Backoffice",
                table: "BodySeatList",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);
        }
    }
}
