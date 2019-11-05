using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class DummyEventsMigration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AlterColumn<DateTime>(
            //    name: "Timestamp",
            //    schema: "OrganisationRegistry",
            //    table: "Events",
            //    nullable: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.AlterColumn<DateTimeOffset>(
            //    name: "Timestamp",
            //    schema: "OrganisationRegistry",
            //    table: "Events",
            //    nullable: false);
        }
    }
}
