namespace OrganisationRegistry.SqlServer.Migrations
{
    using Microsoft.EntityFrameworkCore.Migrations;

    public partial class AddBankAccountsToggle : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(InsertSetting("Toggles:EnableBankAccounts", "Zijn bankrekeningnummers beschikbaar in UI en API?", "false"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
