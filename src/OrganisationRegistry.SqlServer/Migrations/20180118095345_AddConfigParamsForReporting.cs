using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddConfigParamsForReporting : BaseMigration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(InsertSetting("Api:INR_KeyTypeId", "KeyType GUID voor 'INR'", "00000000-0000-0000-0000-000000000000"));
            migrationBuilder.Sql(InsertSetting("Api:KBO_KeyTypeId", "KeyType GUID voor 'KBO'", "00000000-0000-0000-0000-000000000000"));
            migrationBuilder.Sql(InsertSetting("Api:Orafin_KeyTypeId", "KeyType GUID voor 'Orafin'", "00000000-0000-0000-0000-000000000000"));
            migrationBuilder.Sql(InsertSetting("Api:Vlimpers_KeyTypeId", "KeyType GUID voor 'Vlimpers'", "00000000-0000-0000-0000-000000000000"));
            migrationBuilder.Sql(InsertSetting("Api:VlimpersKort_KeyTypeId", "KeyType GUID voor 'Vlimpers kort'", "00000000-0000-0000-0000-000000000000"));

            migrationBuilder.Sql(InsertSetting("Api:Bestuursniveau_ClassificationTypeId", "ClassificationType GUID voor 'Bestuursniveau'", "00000000-0000-0000-0000-000000000000"));
            migrationBuilder.Sql(InsertSetting("Api:Categorie_ClassificationTypeId", "ClassificationType GUID voor 'Categorie'", "00000000-0000-0000-0000-000000000000"));
            migrationBuilder.Sql(InsertSetting("Api:JuridischeVorm_ClassificationTypeId", "ClassificationType GUID voor 'Juridische vorm'", "00000000-0000-0000-0000-000000000000"));
            migrationBuilder.Sql(InsertSetting("Api:ESRKlasseToezichthoudendeOverheid_ClassificationTypeId", "ClassificationType GUID voor 'ESR Klasse toezichthoudende overheid'", "00000000-0000-0000-0000-000000000000"));
            migrationBuilder.Sql(InsertSetting("Api:ESRSector_ClassificationTypeId", "ClassificationType GUID voor 'ESR Sector'", "00000000-0000-0000-0000-000000000000"));
            migrationBuilder.Sql(InsertSetting("Api:ESRToezichthoudendeOverheid_ClassificationTypeId", "ClassificationType GUID voor 'ESR Toezichthoudende overheid'", "00000000-0000-0000-0000-000000000000"));
            migrationBuilder.Sql(InsertSetting("Api:UitvoerendNiveau_ClassificationTypeId", "ClassificationType GUID voor 'Uitvoerend niveau'", "00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
