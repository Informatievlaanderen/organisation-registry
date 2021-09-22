using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class MoveToMoreSchemas : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Reporting");

            migrationBuilder.RenameTable(
                name: "ProjectionStateList",
                schema: "OrganisationRegistry",
                newName: "ProjectionStateList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "PersonMandateList",
                schema: "OrganisationRegistry",
                newName: "PersonMandateList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "OrganisationTerminationList",
                schema: "OrganisationRegistry",
                newName: "OrganisationTerminationList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "OrganisationPerBodyList",
                schema: "OrganisationRegistry",
                newName: "OrganisationPerBodyList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "DelegationList",
                schema: "OrganisationRegistry",
                newName: "DelegationList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "DelegationAssignmentList",
                schema: "OrganisationRegistry",
                newName: "DelegationAssignmentList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "BodySeatGenderRatioPostsPerTypeList",
                schema: "OrganisationRegistry",
                newName: "BodySeatGenderRatioPostsPerTypeList",
                newSchema: "Reporting");

            migrationBuilder.RenameTable(
                name: "BodySeatGenderRatioOrganisationClassificationList",
                schema: "OrganisationRegistry",
                newName: "BodySeatGenderRatioOrganisationClassificationList",
                newSchema: "Reporting");

            migrationBuilder.RenameTable(
                name: "BodySeatGenderRatioLifecyclePhaseValidityList",
                schema: "OrganisationRegistry",
                newName: "BodySeatGenderRatioLifecyclePhaseValidityList",
                newSchema: "Reporting");

            migrationBuilder.RenameTable(
                name: "BodySeatGenderRatioBodyMandateList",
                schema: "OrganisationRegistry",
                newName: "BodySeatGenderRatioBodyMandateList",
                newSchema: "Reporting");

            migrationBuilder.RenameTable(
                name: "BodySeatGenderRatioBodyList",
                schema: "OrganisationRegistry",
                newName: "BodySeatGenderRatioBodyList",
                newSchema: "Reporting");

            migrationBuilder.RenameTable(
                name: "BodySeatGenderRatioAssignmentList",
                schema: "OrganisationRegistry",
                newName: "BodySeatGenderRatioAssignmentList",
                newSchema: "Reporting");

            migrationBuilder.RenameTable(
                name: "BodySeatGenderRatio_PersonList",
                schema: "OrganisationRegistry",
                newName: "BodySeatGenderRatio_PersonList",
                newSchema: "Reporting");

            migrationBuilder.RenameTable(
                name: "BodySeatGenderRatio_OrganisationPerBodyList",
                schema: "OrganisationRegistry",
                newName: "BodySeatGenderRatio_OrganisationPerBodyList",
                newSchema: "Reporting");

            migrationBuilder.RenameTable(
                name: "BodySeatGenderRatio_OrganisationList",
                schema: "OrganisationRegistry",
                newName: "BodySeatGenderRatio_OrganisationList",
                newSchema: "Reporting");

            migrationBuilder.RenameTable(
                name: "BodySeatCacheForBodyMandateList",
                schema: "OrganisationRegistry",
                newName: "BodySeatCacheForBodyMandateList",
                newSchema: "Backoffice");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "ProjectionStateList",
                schema: "Backoffice",
                newName: "ProjectionStateList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "PersonMandateList",
                schema: "Backoffice",
                newName: "PersonMandateList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "OrganisationTerminationList",
                schema: "Backoffice",
                newName: "OrganisationTerminationList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "OrganisationPerBodyList",
                schema: "Backoffice",
                newName: "OrganisationPerBodyList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "DelegationList",
                schema: "Backoffice",
                newName: "DelegationList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "DelegationAssignmentList",
                schema: "Backoffice",
                newName: "DelegationAssignmentList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "BodySeatGenderRatioPostsPerTypeList",
                schema: "Reporting",
                newName: "BodySeatGenderRatioPostsPerTypeList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "BodySeatGenderRatioOrganisationClassificationList",
                schema: "Reporting",
                newName: "BodySeatGenderRatioOrganisationClassificationList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "BodySeatGenderRatioLifecyclePhaseValidityList",
                schema: "Reporting",
                newName: "BodySeatGenderRatioLifecyclePhaseValidityList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "BodySeatGenderRatioBodyMandateList",
                schema: "Reporting",
                newName: "BodySeatGenderRatioBodyMandateList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "BodySeatGenderRatioBodyList",
                schema: "Reporting",
                newName: "BodySeatGenderRatioBodyList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "BodySeatGenderRatioAssignmentList",
                schema: "Reporting",
                newName: "BodySeatGenderRatioAssignmentList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "BodySeatGenderRatio_PersonList",
                schema: "Reporting",
                newName: "BodySeatGenderRatio_PersonList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "BodySeatGenderRatio_OrganisationPerBodyList",
                schema: "Reporting",
                newName: "BodySeatGenderRatio_OrganisationPerBodyList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "BodySeatGenderRatio_OrganisationList",
                schema: "Reporting",
                newName: "BodySeatGenderRatio_OrganisationList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "BodySeatCacheForBodyMandateList",
                schema: "Backoffice",
                newName: "BodySeatCacheForBodyMandateList",
                newSchema: "OrganisationRegistry");
        }
    }
}
