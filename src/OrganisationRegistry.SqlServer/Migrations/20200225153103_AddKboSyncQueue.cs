using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddKboSyncQueue : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "KboSyncQueue",
                schema: "Magda",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    SourceFileName = table.Column<string>(nullable: true),
                    SourceKboNumber = table.Column<string>(nullable: true),
                    SourceName = table.Column<string>(nullable: true),
                    SourceAddressModifiedAt = table.Column<DateTimeOffset>(nullable: false),
                    SourceModifiedAt = table.Column<DateTimeOffset>(nullable: false),
                    MutationReadAt = table.Column<DateTimeOffset>(nullable: false),
                    SyncCompletedAt = table.Column<DateTimeOffset>(nullable: true),
                    SyncStatus = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KboSyncQueue", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "KboSyncQueue",
                schema: "Magda");
        }
    }
}
