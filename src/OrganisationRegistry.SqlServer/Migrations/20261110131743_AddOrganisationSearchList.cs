using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrganisationRegistry.SqlServer.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganisationSearchList : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganisationSearchCodeList",
                schema: "Backoffice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    KeyTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    OrganisationKeyId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Value = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NormalizedValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationSearchCodeList", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationSearchList",
                schema: "Backoffice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OvoNumber = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    ParentOrganisation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ParentOrganisationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ParentOrganisationOvoNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ParentOrganisationsRelationshipId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    FormalFrameworkId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationSearchList", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationSearchClassificationValidity",
                schema: "Backoffice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationClassificationTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganisationClassificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganisationOrganisationClassificationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrganisationSearchListItemId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationSearchClassificationValidity", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                    table.ForeignKey(
                        name: "FK_OrganisationSearchClassificationValidity_OrganisationSearchList_OrganisationSearchListItemId",
                        column: x => x.OrganisationSearchListItemId,
                        principalSchema: "Backoffice",
                        principalTable: "OrganisationSearchList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationSearchFormalFrameworkValidity",
                schema: "Backoffice",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrganisationFormalFrameworkId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ValidFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ValidTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OrganisationSearchListItemId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationSearchFormalFrameworkValidity", x => x.Id)
                        .Annotation("SqlServer:Clustered", true);
                    table.ForeignKey(
                        name: "FK_OrganisationSearchFormalFrameworkValidity_OrganisationSearchList_OrganisationSearchListItemId",
                        column: x => x.OrganisationSearchListItemId,
                        principalSchema: "Backoffice",
                        principalTable: "OrganisationSearchList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationSearchClassificationValidity_OrganisationClassificationTypeId",
                schema: "Backoffice",
                table: "OrganisationSearchClassificationValidity",
                column: "OrganisationClassificationTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationSearchClassificationValidity_OrganisationSearchListItemId",
                schema: "Backoffice",
                table: "OrganisationSearchClassificationValidity",
                column: "OrganisationSearchListItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationSearchClassificationValidity_ValidFrom",
                schema: "Backoffice",
                table: "OrganisationSearchClassificationValidity",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationSearchClassificationValidity_ValidTo",
                schema: "Backoffice",
                table: "OrganisationSearchClassificationValidity",
                column: "ValidTo");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationSearchCodeList_NormalizedValue_KeyTypeId",
                schema: "Backoffice",
                table: "OrganisationSearchCodeList",
                columns: new[] { "NormalizedValue", "KeyTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationSearchCodeList_OrganisationId",
                schema: "Backoffice",
                table: "OrganisationSearchCodeList",
                column: "OrganisationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationSearchCodeList_OrganisationKeyId",
                schema: "Backoffice",
                table: "OrganisationSearchCodeList",
                column: "OrganisationKeyId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationSearchFormalFrameworkValidity_OrganisationSearchListItemId",
                schema: "Backoffice",
                table: "OrganisationSearchFormalFrameworkValidity",
                column: "OrganisationSearchListItemId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationSearchFormalFrameworkValidity_ValidFrom",
                schema: "Backoffice",
                table: "OrganisationSearchFormalFrameworkValidity",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationSearchFormalFrameworkValidity_ValidTo",
                schema: "Backoffice",
                table: "OrganisationSearchFormalFrameworkValidity",
                column: "ValidTo");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationSearchList_FormalFrameworkId",
                schema: "Backoffice",
                table: "OrganisationSearchList",
                column: "FormalFrameworkId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationSearchList_Name",
                schema: "Backoffice",
                table: "OrganisationSearchList",
                column: "Name")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationSearchList_OrganisationId_FormalFrameworkId",
                schema: "Backoffice",
                table: "OrganisationSearchList",
                columns: new[] { "OrganisationId", "FormalFrameworkId" },
                unique: true,
                filter: "[FormalFrameworkId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationSearchList_OvoNumber",
                schema: "Backoffice",
                table: "OrganisationSearchList",
                column: "OvoNumber");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationSearchList_ParentOrganisation",
                schema: "Backoffice",
                table: "OrganisationSearchList",
                column: "ParentOrganisation");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationSearchList_ShortName",
                schema: "Backoffice",
                table: "OrganisationSearchList",
                column: "ShortName");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationSearchList_ValidFrom",
                schema: "Backoffice",
                table: "OrganisationSearchList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationSearchList_ValidTo",
                schema: "Backoffice",
                table: "OrganisationSearchList",
                column: "ValidTo");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganisationSearchClassificationValidity",
                schema: "Backoffice");

            migrationBuilder.DropTable(
                name: "OrganisationSearchCodeList",
                schema: "Backoffice");

            migrationBuilder.DropTable(
                name: "OrganisationSearchFormalFrameworkValidity",
                schema: "Backoffice");

            migrationBuilder.DropTable(
                name: "OrganisationSearchList",
                schema: "Backoffice");
        }
    }
}
