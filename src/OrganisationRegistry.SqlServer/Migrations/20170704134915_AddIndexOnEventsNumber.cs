using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddIndexOnEventsNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Done in OrganisationRegistry\src\OrganisationRegistry.Infrastructure\EventStore\SqlServerEventStore.cs
            //migrationBuilder.CreateIndex(
            //    name: "IX_Events_Number",
            //    schema: "OrganisationRegistry",
            //    table: "Events",
            //    column: "Number");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropIndex(
            //    name: "IX_Events_Number",
            //    schema: "OrganisationRegistry",
            //    table: "Events");
        }
    }
}
