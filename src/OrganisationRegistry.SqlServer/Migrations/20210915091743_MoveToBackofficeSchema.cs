using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class MoveToBackofficeSchema : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Backoffice");

            migrationBuilder.RenameTable(
                name: "SeatTypeList",
                schema: "OrganisationRegistry",
                newName: "SeatTypeList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "RegulationTypeList",
                schema: "OrganisationRegistry",
                newName: "RegulationTypeList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "PurposeList",
                schema: "OrganisationRegistry",
                newName: "PurposeList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "PersonList",
                schema: "OrganisationRegistry",
                newName: "PersonList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "PersonFunctionList",
                schema: "OrganisationRegistry",
                newName: "PersonFunctionList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "PersonCapacityList",
                schema: "OrganisationRegistry",
                newName: "PersonCapacityList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "OrganisationTreeList",
                schema: "OrganisationRegistry",
                newName: "OrganisationTreeList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "OrganisationRelationTypeList",
                schema: "OrganisationRegistry",
                newName: "OrganisationRelationTypeList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "OrganisationRelationList",
                schema: "OrganisationRegistry",
                newName: "OrganisationRelationList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "OrganisationRegulationList",
                schema: "OrganisationRegistry",
                newName: "OrganisationRegulationList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "OrganisationParentList",
                schema: "OrganisationRegistry",
                newName: "OrganisationParentList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "OrganisationOrganisationClassificationList",
                schema: "OrganisationRegistry",
                newName: "OrganisationOrganisationClassificationList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "OrganisationOpeningHourList",
                schema: "OrganisationRegistry",
                newName: "OrganisationOpeningHourList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "OrganisationLocationList",
                schema: "OrganisationRegistry",
                newName: "OrganisationLocationList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "OrganisationList",
                schema: "OrganisationRegistry",
                newName: "OrganisationList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "OrganisationLabelList",
                schema: "OrganisationRegistry",
                newName: "OrganisationLabelList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "OrganisationKeyList",
                schema: "OrganisationRegistry",
                newName: "OrganisationKeyList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "OrganisationFunctionList",
                schema: "OrganisationRegistry",
                newName: "OrganisationFunctionList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "OrganisationFormalFrameworkValidity",
                schema: "OrganisationRegistry",
                newName: "OrganisationFormalFrameworkValidity",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "OrganisationFormalFrameworkList",
                schema: "OrganisationRegistry",
                newName: "OrganisationFormalFrameworkList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "OrganisationDetail",
                schema: "OrganisationRegistry",
                newName: "OrganisationDetail",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "OrganisationContactList",
                schema: "OrganisationRegistry",
                newName: "OrganisationContactList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "OrganisationClassificationValidity",
                schema: "OrganisationRegistry",
                newName: "OrganisationClassificationValidity",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "OrganisationClassificationTypeList",
                schema: "OrganisationRegistry",
                newName: "OrganisationClassificationTypeList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "OrganisationClassificationList",
                schema: "OrganisationRegistry",
                newName: "OrganisationClassificationList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "OrganisationChildList",
                schema: "OrganisationRegistry",
                newName: "OrganisationChildList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "OrganisationCapacityList",
                schema: "OrganisationRegistry",
                newName: "OrganisationCapacityList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "OrganisationBuildingList",
                schema: "OrganisationRegistry",
                newName: "OrganisationBuildingList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "OrganisationBodyList",
                schema: "OrganisationRegistry",
                newName: "OrganisationBodyList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "OrganisationBankAccountList",
                schema: "OrganisationRegistry",
                newName: "OrganisationBankAccountList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "MandateRoleTypeList",
                schema: "OrganisationRegistry",
                newName: "MandateRoleTypeList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "LocationTypeList",
                schema: "OrganisationRegistry",
                newName: "LocationTypeList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "LocationList",
                schema: "OrganisationRegistry",
                newName: "LocationList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "LifecyclePhaseTypeList",
                schema: "OrganisationRegistry",
                newName: "LifecyclePhaseTypeList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "LabelTypeList",
                schema: "OrganisationRegistry",
                newName: "LabelTypeList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "KeyTypeList",
                schema: "OrganisationRegistry",
                newName: "KeyTypeList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "FuturePeopleAssignedToBodyMandatesList",
                schema: "OrganisationRegistry",
                newName: "FuturePeopleAssignedToBodyMandatesList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "FutureActiveOrganisationParentList",
                schema: "OrganisationRegistry",
                newName: "FutureActiveOrganisationParentList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "FutureActiveOrganisationFormalFrameworkList",
                schema: "OrganisationRegistry",
                newName: "FutureActiveOrganisationFormalFrameworkList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "FutureActiveBodyOrganisationList",
                schema: "OrganisationRegistry",
                newName: "FutureActiveBodyOrganisationList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "FunctionList",
                schema: "OrganisationRegistry",
                newName: "FunctionList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "FormalFrameworkList",
                schema: "OrganisationRegistry",
                newName: "FormalFrameworkList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "FormalFrameworkCategoryList",
                schema: "OrganisationRegistry",
                newName: "FormalFrameworkCategoryList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "ContactTypeList",
                schema: "OrganisationRegistry",
                newName: "ContactTypeList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "CapacityList",
                schema: "OrganisationRegistry",
                newName: "CapacityList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "BuildingList",
                schema: "OrganisationRegistry",
                newName: "BuildingList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "BodySeatList",
                schema: "OrganisationRegistry",
                newName: "BodySeatList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "BodyOrganisationList",
                schema: "OrganisationRegistry",
                newName: "BodyOrganisationList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "BodyMandateList",
                schema: "OrganisationRegistry",
                newName: "BodyMandateList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "BodyList",
                schema: "OrganisationRegistry",
                newName: "BodyList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "BodyLifecyclePhaseValidity",
                schema: "OrganisationRegistry",
                newName: "BodyLifecyclePhaseValidity",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "BodyLifecyclePhaseList",
                schema: "OrganisationRegistry",
                newName: "BodyLifecyclePhaseList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "BodyFormalFrameworkList",
                schema: "OrganisationRegistry",
                newName: "BodyFormalFrameworkList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "BodyDetail",
                schema: "OrganisationRegistry",
                newName: "BodyDetail",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "BodyContactList",
                schema: "OrganisationRegistry",
                newName: "BodyContactList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "BodyClassificationTypeList",
                schema: "OrganisationRegistry",
                newName: "BodyClassificationTypeList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "BodyClassificationList",
                schema: "OrganisationRegistry",
                newName: "BodyClassificationList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "BodyBodyClassificationList",
                schema: "OrganisationRegistry",
                newName: "BodyBodyClassificationList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "ActivePeopleAssignedToBodyMandatesList",
                schema: "OrganisationRegistry",
                newName: "ActivePeopleAssignedToBodyMandatesList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "ActiveOrganisationParentList",
                schema: "OrganisationRegistry",
                newName: "ActiveOrganisationParentList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "ActiveOrganisationFormalFrameworkList",
                schema: "OrganisationRegistry",
                newName: "ActiveOrganisationFormalFrameworkList",
                newSchema: "Backoffice");

            migrationBuilder.RenameTable(
                name: "ActiveBodyOrganisationList",
                schema: "OrganisationRegistry",
                newName: "ActiveBodyOrganisationList",
                newSchema: "Backoffice");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "SeatTypeList",
                schema: "Backoffice",
                newName: "SeatTypeList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "RegulationTypeList",
                schema: "Backoffice",
                newName: "RegulationTypeList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "PurposeList",
                schema: "Backoffice",
                newName: "PurposeList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "PersonList",
                schema: "Backoffice",
                newName: "PersonList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "PersonFunctionList",
                schema: "Backoffice",
                newName: "PersonFunctionList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "PersonCapacityList",
                schema: "Backoffice",
                newName: "PersonCapacityList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "OrganisationTreeList",
                schema: "Backoffice",
                newName: "OrganisationTreeList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "OrganisationRelationTypeList",
                schema: "Backoffice",
                newName: "OrganisationRelationTypeList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "OrganisationRelationList",
                schema: "Backoffice",
                newName: "OrganisationRelationList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "OrganisationRegulationList",
                schema: "Backoffice",
                newName: "OrganisationRegulationList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "OrganisationParentList",
                schema: "Backoffice",
                newName: "OrganisationParentList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "OrganisationOrganisationClassificationList",
                schema: "Backoffice",
                newName: "OrganisationOrganisationClassificationList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "OrganisationOpeningHourList",
                schema: "Backoffice",
                newName: "OrganisationOpeningHourList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "OrganisationLocationList",
                schema: "Backoffice",
                newName: "OrganisationLocationList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "OrganisationList",
                schema: "Backoffice",
                newName: "OrganisationList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "OrganisationLabelList",
                schema: "Backoffice",
                newName: "OrganisationLabelList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "OrganisationKeyList",
                schema: "Backoffice",
                newName: "OrganisationKeyList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "OrganisationFunctionList",
                schema: "Backoffice",
                newName: "OrganisationFunctionList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "OrganisationFormalFrameworkValidity",
                schema: "Backoffice",
                newName: "OrganisationFormalFrameworkValidity",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "OrganisationFormalFrameworkList",
                schema: "Backoffice",
                newName: "OrganisationFormalFrameworkList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "OrganisationDetail",
                schema: "Backoffice",
                newName: "OrganisationDetail",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "OrganisationContactList",
                schema: "Backoffice",
                newName: "OrganisationContactList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "OrganisationClassificationValidity",
                schema: "Backoffice",
                newName: "OrganisationClassificationValidity",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "OrganisationClassificationTypeList",
                schema: "Backoffice",
                newName: "OrganisationClassificationTypeList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "OrganisationClassificationList",
                schema: "Backoffice",
                newName: "OrganisationClassificationList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "OrganisationChildList",
                schema: "Backoffice",
                newName: "OrganisationChildList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "OrganisationCapacityList",
                schema: "Backoffice",
                newName: "OrganisationCapacityList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "OrganisationBuildingList",
                schema: "Backoffice",
                newName: "OrganisationBuildingList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "OrganisationBodyList",
                schema: "Backoffice",
                newName: "OrganisationBodyList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "OrganisationBankAccountList",
                schema: "Backoffice",
                newName: "OrganisationBankAccountList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "MandateRoleTypeList",
                schema: "Backoffice",
                newName: "MandateRoleTypeList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "LocationTypeList",
                schema: "Backoffice",
                newName: "LocationTypeList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "LocationList",
                schema: "Backoffice",
                newName: "LocationList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "LifecyclePhaseTypeList",
                schema: "Backoffice",
                newName: "LifecyclePhaseTypeList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "LabelTypeList",
                schema: "Backoffice",
                newName: "LabelTypeList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "KeyTypeList",
                schema: "Backoffice",
                newName: "KeyTypeList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "FuturePeopleAssignedToBodyMandatesList",
                schema: "Backoffice",
                newName: "FuturePeopleAssignedToBodyMandatesList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "FutureActiveOrganisationParentList",
                schema: "Backoffice",
                newName: "FutureActiveOrganisationParentList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "FutureActiveOrganisationFormalFrameworkList",
                schema: "Backoffice",
                newName: "FutureActiveOrganisationFormalFrameworkList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "FutureActiveBodyOrganisationList",
                schema: "Backoffice",
                newName: "FutureActiveBodyOrganisationList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "FunctionList",
                schema: "Backoffice",
                newName: "FunctionList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "FormalFrameworkList",
                schema: "Backoffice",
                newName: "FormalFrameworkList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "FormalFrameworkCategoryList",
                schema: "Backoffice",
                newName: "FormalFrameworkCategoryList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "ContactTypeList",
                schema: "Backoffice",
                newName: "ContactTypeList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "CapacityList",
                schema: "Backoffice",
                newName: "CapacityList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "BuildingList",
                schema: "Backoffice",
                newName: "BuildingList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "BodySeatList",
                schema: "Backoffice",
                newName: "BodySeatList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "BodyOrganisationList",
                schema: "Backoffice",
                newName: "BodyOrganisationList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "BodyMandateList",
                schema: "Backoffice",
                newName: "BodyMandateList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "BodyList",
                schema: "Backoffice",
                newName: "BodyList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "BodyLifecyclePhaseValidity",
                schema: "Backoffice",
                newName: "BodyLifecyclePhaseValidity",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "BodyLifecyclePhaseList",
                schema: "Backoffice",
                newName: "BodyLifecyclePhaseList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "BodyFormalFrameworkList",
                schema: "Backoffice",
                newName: "BodyFormalFrameworkList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "BodyDetail",
                schema: "Backoffice",
                newName: "BodyDetail",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "BodyContactList",
                schema: "Backoffice",
                newName: "BodyContactList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "BodyClassificationTypeList",
                schema: "Backoffice",
                newName: "BodyClassificationTypeList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "BodyClassificationList",
                schema: "Backoffice",
                newName: "BodyClassificationList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "BodyBodyClassificationList",
                schema: "Backoffice",
                newName: "BodyBodyClassificationList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "ActivePeopleAssignedToBodyMandatesList",
                schema: "Backoffice",
                newName: "ActivePeopleAssignedToBodyMandatesList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "ActiveOrganisationParentList",
                schema: "Backoffice",
                newName: "ActiveOrganisationParentList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "ActiveOrganisationFormalFrameworkList",
                schema: "Backoffice",
                newName: "ActiveOrganisationFormalFrameworkList",
                newSchema: "OrganisationRegistry");

            migrationBuilder.RenameTable(
                name: "ActiveBodyOrganisationList",
                schema: "Backoffice",
                newName: "ActiveBodyOrganisationList",
                newSchema: "OrganisationRegistry");
        }
    }
}
