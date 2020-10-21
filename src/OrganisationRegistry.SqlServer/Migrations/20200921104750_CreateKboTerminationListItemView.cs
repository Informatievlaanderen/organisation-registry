using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class CreateKboTerminationListItemView : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KboTerminationSyncQueue",
                schema: "Magda");

            migrationBuilder.Sql("UPDATE [Magda].[KboSyncQueue] SET SourceOrganisationStatus = 'Unknown' WHERE SourceOrganisationStatus IS NULL");
            migrationBuilder.Sql("UPDATE [Magda].[KboSyncQueue] SET SourceOrganisationName = 'Unknown' WHERE SourceOrganisationName IS NULL");
            migrationBuilder.Sql("UPDATE [Magda].[KboSyncQueue] SET SourceOrganisationKboNumber = 'Unknown' WHERE SourceOrganisationKboNumber IS NULL");
            migrationBuilder.Sql("UPDATE [Magda].[KboSyncQueue] SET SourceFileName = 'Unknown' WHERE SourceFileName IS NULL");

            migrationBuilder.AlterColumn<string>(
                name: "SourceOrganisationStatus",
                schema: "Magda",
                table: "KboSyncQueue",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SourceOrganisationName",
                schema: "Magda",
                table: "KboSyncQueue",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SourceOrganisationKboNumber",
                schema: "Magda",
                table: "KboSyncQueue",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "SourceFileName",
                schema: "Magda",
                table: "KboSyncQueue",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "OrganisationTerminationList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    OvoNumber = table.Column<string>(maxLength: 10, nullable: false),
                    KboNumber = table.Column<string>(nullable: true),
                    Name = table.Column<string>(maxLength: 500, nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Code = table.Column<string>(nullable: false),
                    Reason = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationTerminationList", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationTerminationList_Name",
                schema: "OrganisationRegistry",
                table: "OrganisationTerminationList",
                column: "Name")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationTerminationList_OvoNumber",
                schema: "OrganisationRegistry",
                table: "OrganisationTerminationList",
                column: "OvoNumber",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganisationTerminationList",
                schema: "OrganisationRegistry");

            migrationBuilder.AlterColumn<string>(
                name: "SourceOrganisationStatus",
                schema: "Magda",
                table: "KboSyncQueue",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "SourceOrganisationName",
                schema: "Magda",
                table: "KboSyncQueue",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "SourceOrganisationKboNumber",
                schema: "Magda",
                table: "KboSyncQueue",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AlterColumn<string>(
                name: "SourceFileName",
                schema: "Magda",
                table: "KboSyncQueue",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.CreateTable(
                name: "KboTerminationSyncQueue",
                schema: "Magda",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MutationReadAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    SourceFileName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceOrganisationKboNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceOrganisationModifiedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    SourceOrganisationName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceOrganisationTerminationCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceOrganisationTerminationDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    SourceOrganisationTerminationReason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SyncCompletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SyncInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SyncStatus = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KboTerminationSyncQueue", x => x.Id);
                });
        }
    }
}
