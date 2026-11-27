using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrganisationRegistry.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class SearchCodeBinaryCollationAndKeyTypeIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Enkel de collatie en de index wijzigen; de data in de tabel (en dus de projectie) blijft ongemoeid.
            // NormalizedValue en de zoekterm worden op dezelfde manier genormaliseerd (hoofdletters, enkel letters
            // en cijfers), dus hoofdletterongevoeligheid in de collatie is overbodig.
            migrationBuilder.DropIndex(
                name: "IX_OrganisationSearchCodeList_NormalizedValue_KeyTypeId",
                schema: "Backoffice",
                table: "OrganisationSearchCodeList");

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedValue",
                schema: "Backoffice",
                table: "OrganisationSearchCodeList",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                collation: "Latin1_General_100_BIN2",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationSearchCodeList_KeyTypeId_NormalizedValue",
                schema: "Backoffice",
                table: "OrganisationSearchCodeList",
                columns: new[] { "KeyTypeId", "NormalizedValue" })
                .Annotation("SqlServer:Include", new[] { "OrganisationId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganisationSearchCodeList_KeyTypeId_NormalizedValue",
                schema: "Backoffice",
                table: "OrganisationSearchCodeList");

            migrationBuilder.AlterColumn<string>(
                name: "NormalizedValue",
                schema: "Backoffice",
                table: "OrganisationSearchCodeList",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldCollation: "Latin1_General_100_BIN2");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationSearchCodeList_NormalizedValue_KeyTypeId",
                schema: "Backoffice",
                table: "OrganisationSearchCodeList",
                columns: new[] { "NormalizedValue", "KeyTypeId" })
                .Annotation("SqlServer:Include", new[] { "OrganisationId" });
        }
    }
}
