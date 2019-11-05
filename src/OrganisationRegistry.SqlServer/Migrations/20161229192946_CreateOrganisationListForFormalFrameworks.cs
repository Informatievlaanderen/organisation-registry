using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Metadata;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class CreateOrganisationListForFormalFrameworks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganisationList",
                schema: "OrganisationRegistry",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    FormalFrameworkId = table.Column<Guid>(nullable: true),
                    Name = table.Column<string>(maxLength: 500, nullable: false),
                    OrganisationClassificationId = table.Column<Guid>(nullable: true),
                    OrganisationClassificationTypeId = table.Column<Guid>(nullable: true),
                    OrganisationId = table.Column<Guid>(nullable: false),
                    OvoNumber = table.Column<string>(maxLength: 10, nullable: false),
                    ParentOrganisation = table.Column<string>(nullable: true),
                    ParentOrganisationId = table.Column<Guid>(nullable: true),
                    ShortName = table.Column<string>(nullable: true),
                    ValidFrom = table.Column<DateTime>(nullable: true),
                    ValidTo = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationList", x => x.Id)
                        .Annotation("SqlServer:Clustered", false);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationList_Name",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                column: "Name")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationList_OvoNumber",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                column: "OvoNumber");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationList_ParentOrganisation",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                column: "ParentOrganisation");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationList_OrganisationId_FormalFrameworkId",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                columns: new[] { "OrganisationId", "FormalFrameworkId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganisationList",
                schema: "OrganisationRegistry");
        }
    }
}
