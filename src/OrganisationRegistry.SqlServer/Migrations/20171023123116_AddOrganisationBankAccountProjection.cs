using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddOrganisationBankAccountProjection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganisationBankAccountList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    OrganisationBankAccountId = table.Column<Guid>(nullable: false),
                    BankAccountNumber = table.Column<string>(nullable: false),
                    IsIban = table.Column<bool>(nullable: false),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationBankAccountList", x => x.OrganisationBankAccountId)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationBankAccountList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationBankAccountList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationBankAccountList_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationBankAccountList",
                column: "ValidTo");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganisationBankAccountList",
                schema: "OrganisationRegistry");
        }
    }
}
