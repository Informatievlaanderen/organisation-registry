namespace OrganisationRegistry.VlaanderenBeNotifier.Schema.Migrations;

using Microsoft.EntityFrameworkCore.Migrations;

public class BaseMigration : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder) {}

    protected string InsertSetting(string key, string description, string value)
    {
        key = key.Replace("'", "''");
        description = description.Replace("'", "''");
        value = value.Replace("'", "''");

        return $@"IF NOT EXISTS (SELECT * FROM [OrganisationRegistry].[Configuration] WHERE [Key] = '{key}')
                        INSERT INTO [OrganisationRegistry].[Configuration] ([Key], [Description], [Value])
                             VALUES ('{key}', '{description}', '{value}')";
    }

    protected string DeleteSetting(string key)
    {
        key = key.Replace("'", "''");
        return $@"DELETE FROM [OrganisationRegistry].[Configuration] WHERE [Key] = '{key}'";
    }

    protected string RenameSetting(string oldKey, string newKey)
    {
        oldKey = oldKey.Replace("'", "''");
        newKey = newKey.Replace("'", "''");
        return $@"UPDATE [OrganisationRegistry].[Configuration] SET [Key] = '{newKey}' WHERE [Key] = '{oldKey}'";
    }

    protected string RenameSettingDescription(string key, string description)
    {
        key = key.Replace("'", "''");
        description = description.Replace("'", "''");
        return $@"UPDATE [OrganisationRegistry].[Configuration] SET [Description] = '{description}' WHERE [Key] = '{key}'";
    }
}
