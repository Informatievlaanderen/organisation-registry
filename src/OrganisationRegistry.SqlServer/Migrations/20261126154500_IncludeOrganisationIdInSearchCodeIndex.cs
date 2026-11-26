using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrganisationRegistry.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class IncludeOrganisationIdInSearchCodeIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Enkel de index wordt herbouwd; de data in de tabel (en dus de projectie) blijft ongemoeid.
            migrationBuilder.DropIndex(
                name: "IX_OrganisationSearchCodeList_NormalizedValue_KeyTypeId",
                schema: "Backoffice",
                table: "OrganisationSearchCodeList");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationSearchCodeList_NormalizedValue_KeyTypeId",
                schema: "Backoffice",
                table: "OrganisationSearchCodeList",
                columns: new[] { "NormalizedValue", "KeyTypeId" })
                .Annotation("SqlServer:Include", new[] { "OrganisationId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_OrganisationSearchCodeList_NormalizedValue_KeyTypeId",
                schema: "Backoffice",
                table: "OrganisationSearchCodeList");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationSearchCodeList_NormalizedValue_KeyTypeId",
                schema: "Backoffice",
                table: "OrganisationSearchCodeList",
                columns: new[] { "NormalizedValue", "KeyTypeId" });
        }
    }
}
