using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class CreateKboTerminationSyncQueue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SourceOrganisationStatus",
                schema: "Magda",
                table: "KboSyncQueue",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "KboTerminationSyncQueue",
                schema: "Magda",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SourceFileName = table.Column<string>(nullable: false),
                    SourceOrganisationKboNumber = table.Column<string>(nullable: false),
                    SourceOrganisationName = table.Column<string>(nullable: false),
                    SourceOrganisationModifiedAt = table.Column<DateTimeOffset>(nullable: false),
                    SourceOrganisationTerminationCode = table.Column<string>(nullable: false),
                    SourceOrganisationTerminationReason = table.Column<string>(nullable: false),
                    SourceOrganisationTerminationDate = table.Column<DateTimeOffset>(nullable: false),
                    MutationReadAt = table.Column<DateTimeOffset>(nullable: false),
                    SyncCompletedAt = table.Column<DateTimeOffset>(nullable: true),
                    SyncStatus = table.Column<string>(nullable: true),
                    SyncInfo = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KboTerminationSyncQueue", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KboTerminationSyncQueue",
                schema: "Magda");

            migrationBuilder.DropColumn(
                name: "SourceOrganisationStatus",
                schema: "Magda",
                table: "KboSyncQueue");
        }
    }
}
