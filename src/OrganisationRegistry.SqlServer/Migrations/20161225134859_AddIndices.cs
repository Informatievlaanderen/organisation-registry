using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace OrganisationRegistry.SqlServer.Migrations
{
    public partial class AddIndices : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PurposeList",
                schema: "OrganisationRegistry",
                table: "PurposeList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PersonList",
                schema: "OrganisationRegistry",
                table: "PersonList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PersonFunctionList",
                schema: "OrganisationRegistry",
                table: "PersonFunctionList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PersonCapacityList",
                schema: "OrganisationRegistry",
                table: "PersonCapacityList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationRelationTypeList",
                schema: "OrganisationRegistry",
                table: "OrganisationRelationTypeList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationClassificationTypeList",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationTypeList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationClassificationList",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationParentList",
                schema: "OrganisationRegistry",
                table: "OrganisationParentList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationOrganisationClassificationList",
                schema: "OrganisationRegistry",
                table: "OrganisationOrganisationClassificationList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationLocationList",
                schema: "OrganisationRegistry",
                table: "OrganisationLocationList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationList",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationLabelList",
                schema: "OrganisationRegistry",
                table: "OrganisationLabelList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationKeyList",
                schema: "OrganisationRegistry",
                table: "OrganisationKeyList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationFunctionList",
                schema: "OrganisationRegistry",
                table: "OrganisationFunctionList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationFormalFrameworkList",
                schema: "OrganisationRegistry",
                table: "OrganisationFormalFrameworkList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationContactList",
                schema: "OrganisationRegistry",
                table: "OrganisationContactList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationChildList",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationCapacityList",
                schema: "OrganisationRegistry",
                table: "OrganisationCapacityList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationBuildingList",
                schema: "OrganisationRegistry",
                table: "OrganisationBuildingList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LocationList",
                schema: "OrganisationRegistry",
                table: "LocationList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LabelTypeList",
                schema: "OrganisationRegistry",
                table: "LabelTypeList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KeyTypeList",
                schema: "OrganisationRegistry",
                table: "KeyTypeList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FunctionList",
                schema: "OrganisationRegistry",
                table: "FunctionList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FormalFrameworkCategoryList",
                schema: "OrganisationRegistry",
                table: "FormalFrameworkCategoryList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FormalFrameworkList",
                schema: "OrganisationRegistry",
                table: "FormalFrameworkList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContactTypeList",
                schema: "OrganisationRegistry",
                table: "ContactTypeList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CapacityList",
                schema: "OrganisationRegistry",
                table: "CapacityList");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "PurposeList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PurposeList",
                schema: "OrganisationRegistry",
                table: "PurposeList",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_PurposeList_Name",
                schema: "OrganisationRegistry",
                table: "PurposeList",
                column: "Name",
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "PersonList",
                maxLength: 200,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                schema: "OrganisationRegistry",
                table: "PersonList",
                maxLength: 200,
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PersonList",
                schema: "OrganisationRegistry",
                table: "PersonList",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_PersonList_FirstName",
                schema: "OrganisationRegistry",
                table: "PersonList",
                column: "FirstName");

            migrationBuilder.CreateIndex(
                name: "IX_PersonList_Name",
                schema: "OrganisationRegistry",
                table: "PersonList",
                column: "Name")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.AlterColumn<string>(
                name: "OrganisationName",
                schema: "OrganisationRegistry",
                table: "PersonFunctionList",
                maxLength: 2000,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "FunctionName",
                schema: "OrganisationRegistry",
                table: "PersonFunctionList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PersonFunctionList",
                schema: "OrganisationRegistry",
                table: "PersonFunctionList",
                column: "OrganisationFunctionId")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_PersonFunctionList_FunctionName",
                schema: "OrganisationRegistry",
                table: "PersonFunctionList",
                column: "FunctionName");

            migrationBuilder.CreateIndex(
                name: "IX_PersonFunctionList_OrganisationName",
                schema: "OrganisationRegistry",
                table: "PersonFunctionList",
                column: "OrganisationName")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonFunctionList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "PersonFunctionList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_PersonFunctionList_ValidTo",
                schema: "OrganisationRegistry",
                table: "PersonFunctionList",
                column: "ValidTo");

            migrationBuilder.AlterColumn<Guid>(
                name: "PersonId",
                schema: "OrganisationRegistry",
                table: "PersonCapacityList",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "OrganisationName",
                schema: "OrganisationRegistry",
                table: "PersonCapacityList",
                maxLength: 2000,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "CapacityName",
                schema: "OrganisationRegistry",
                table: "PersonCapacityList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PersonCapacityList",
                schema: "OrganisationRegistry",
                table: "PersonCapacityList",
                column: "OrganisationCapacityId")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_PersonCapacityList_CapacityName",
                schema: "OrganisationRegistry",
                table: "PersonCapacityList",
                column: "CapacityName");

            migrationBuilder.CreateIndex(
                name: "IX_PersonCapacityList_FunctionName",
                schema: "OrganisationRegistry",
                table: "PersonCapacityList",
                column: "FunctionName");

            migrationBuilder.CreateIndex(
                name: "IX_PersonCapacityList_OrganisationName",
                schema: "OrganisationRegistry",
                table: "PersonCapacityList",
                column: "OrganisationName")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_PersonCapacityList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "PersonCapacityList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_PersonCapacityList_ValidTo",
                schema: "OrganisationRegistry",
                table: "PersonCapacityList",
                column: "ValidTo");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "OrganisationRelationTypeList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationRelationTypeList",
                schema: "OrganisationRegistry",
                table: "OrganisationRelationTypeList",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationRelationTypeList_Name",
                schema: "OrganisationRegistry",
                table: "OrganisationRelationTypeList",
                column: "Name",
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationTypeList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationClassificationTypeList",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationTypeList",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationClassificationTypeList_Name",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationTypeList",
                column: "Name",
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.AlterColumn<string>(
                name: "OrganisationClassificationTypeName",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationClassificationList",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationList",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationClassificationList_Active",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationList",
                column: "Active");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationClassificationList_Name",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationList",
                column: "Name")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationClassificationList_Order",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationList",
                column: "Order");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationClassificationList_OrganisationClassificationTypeName",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationList",
                column: "OrganisationClassificationTypeName");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationClassificationList_Name_OrganisationClassificationTypeId",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationList",
                columns: new[] { "Name", "OrganisationClassificationTypeId" },
                unique: true);

            migrationBuilder.AlterColumn<string>(
                name: "ParentOrganisationName",
                schema: "OrganisationRegistry",
                table: "OrganisationParentList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationParentList",
                schema: "OrganisationRegistry",
                table: "OrganisationParentList",
                column: "OrganisationOrganisationParentId")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationParentList_ParentOrganisationName",
                schema: "OrganisationRegistry",
                table: "OrganisationParentList",
                column: "ParentOrganisationName")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationParentList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationParentList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationParentList_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationParentList",
                column: "ValidTo");

            migrationBuilder.AlterColumn<string>(
                name: "OrganisationClassificationTypeName",
                schema: "OrganisationRegistry",
                table: "OrganisationOrganisationClassificationList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "OrganisationClassificationName",
                schema: "OrganisationRegistry",
                table: "OrganisationOrganisationClassificationList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationOrganisationClassificationList",
                schema: "OrganisationRegistry",
                table: "OrganisationOrganisationClassificationList",
                column: "OrganisationOrganisationClassificationId")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationOrganisationClassificationList_OrganisationClassificationName",
                schema: "OrganisationRegistry",
                table: "OrganisationOrganisationClassificationList",
                column: "OrganisationClassificationName");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationOrganisationClassificationList_OrganisationClassificationTypeName",
                schema: "OrganisationRegistry",
                table: "OrganisationOrganisationClassificationList",
                column: "OrganisationClassificationTypeName")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationOrganisationClassificationList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationOrganisationClassificationList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationOrganisationClassificationList_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationOrganisationClassificationList",
                column: "ValidTo");

            migrationBuilder.AlterColumn<string>(
                name: "LocationName",
                schema: "OrganisationRegistry",
                table: "OrganisationLocationList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationLocationList",
                schema: "OrganisationRegistry",
                table: "OrganisationLocationList",
                column: "OrganisationLocationId")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationLocationList_IsMainLocation",
                schema: "OrganisationRegistry",
                table: "OrganisationLocationList",
                column: "IsMainLocation");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationLocationList_LocationName",
                schema: "OrganisationRegistry",
                table: "OrganisationLocationList",
                column: "LocationName")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationLocationList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationLocationList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationLocationList_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationLocationList",
                column: "ValidTo");

            migrationBuilder.AlterColumn<string>(
                name: "ParentOrganisation",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OvoNumber",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                maxLength: 10,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationList",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

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
                column: "OvoNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationList_ParentOrganisation",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                column: "ParentOrganisation");

            migrationBuilder.AlterColumn<string>(
                name: "LabelValue",
                schema: "OrganisationRegistry",
                table: "OrganisationLabelList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "LabelTypeName",
                schema: "OrganisationRegistry",
                table: "OrganisationLabelList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationLabelList",
                schema: "OrganisationRegistry",
                table: "OrganisationLabelList",
                column: "OrganisationLabelId")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationLabelList_LabelTypeName",
                schema: "OrganisationRegistry",
                table: "OrganisationLabelList",
                column: "LabelTypeName")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationLabelList_LabelValue",
                schema: "OrganisationRegistry",
                table: "OrganisationLabelList",
                column: "LabelValue");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationLabelList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationLabelList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationLabelList_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationLabelList",
                column: "ValidTo");

            migrationBuilder.AlterColumn<string>(
                name: "KeyValue",
                schema: "OrganisationRegistry",
                table: "OrganisationKeyList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "KeyTypeName",
                schema: "OrganisationRegistry",
                table: "OrganisationKeyList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationKeyList",
                schema: "OrganisationRegistry",
                table: "OrganisationKeyList",
                column: "OrganisationKeyId")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationKeyList_KeyTypeName",
                schema: "OrganisationRegistry",
                table: "OrganisationKeyList",
                column: "KeyTypeName")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationKeyList_KeyValue",
                schema: "OrganisationRegistry",
                table: "OrganisationKeyList",
                column: "KeyValue");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationKeyList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationKeyList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationKeyList_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationKeyList",
                column: "ValidTo");

            migrationBuilder.AlterColumn<string>(
                name: "PersonName",
                schema: "OrganisationRegistry",
                table: "OrganisationFunctionList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "FunctionName",
                schema: "OrganisationRegistry",
                table: "OrganisationFunctionList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationFunctionList",
                schema: "OrganisationRegistry",
                table: "OrganisationFunctionList",
                column: "OrganisationFunctionId")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationFunctionList_FunctionName",
                schema: "OrganisationRegistry",
                table: "OrganisationFunctionList",
                column: "FunctionName");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationFunctionList_PersonName",
                schema: "OrganisationRegistry",
                table: "OrganisationFunctionList",
                column: "PersonName")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationFunctionList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationFunctionList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationFunctionList_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationFunctionList",
                column: "ValidTo");

            migrationBuilder.AlterColumn<string>(
                name: "ParentOrganisationName",
                schema: "OrganisationRegistry",
                table: "OrganisationFormalFrameworkList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "FormalFrameworkName",
                schema: "OrganisationRegistry",
                table: "OrganisationFormalFrameworkList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationFormalFrameworkList",
                schema: "OrganisationRegistry",
                table: "OrganisationFormalFrameworkList",
                column: "OrganisationFormalFrameworkId")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationFormalFrameworkList_FormalFrameworkName",
                schema: "OrganisationRegistry",
                table: "OrganisationFormalFrameworkList",
                column: "FormalFrameworkName");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationFormalFrameworkList_ParentOrganisationName",
                schema: "OrganisationRegistry",
                table: "OrganisationFormalFrameworkList",
                column: "ParentOrganisationName")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationFormalFrameworkList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationFormalFrameworkList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationFormalFrameworkList_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationFormalFrameworkList",
                column: "ValidTo");

            migrationBuilder.AlterColumn<string>(
                name: "ContactValue",
                schema: "OrganisationRegistry",
                table: "OrganisationContactList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "ContactTypeName",
                schema: "OrganisationRegistry",
                table: "OrganisationContactList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationContactList",
                schema: "OrganisationRegistry",
                table: "OrganisationContactList",
                column: "OrganisationContactId")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationContactList_ContactTypeName",
                schema: "OrganisationRegistry",
                table: "OrganisationContactList",
                column: "ContactTypeName")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationContactList_ContactValue",
                schema: "OrganisationRegistry",
                table: "OrganisationContactList",
                column: "ContactValue");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationContactList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationContactList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationContactList_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationContactList",
                column: "ValidTo");

            migrationBuilder.AlterColumn<string>(
                name: "OvoNumber",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationChildList",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList",
                column: "OrganisationOrganisationParentId")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationChildList_Name",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList",
                column: "Name")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationChildList_OrganisationValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList",
                column: "OrganisationValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationChildList_OrganisationValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList",
                column: "OrganisationValidTo");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationChildList_OvoNumber",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList",
                column: "OvoNumber");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationChildList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationChildList_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList",
                column: "ValidTo");

            migrationBuilder.AlterColumn<string>(
                name: "CapacityName",
                schema: "OrganisationRegistry",
                table: "OrganisationCapacityList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationCapacityList",
                schema: "OrganisationRegistry",
                table: "OrganisationCapacityList",
                column: "OrganisationCapacityId")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationCapacityList_CapacityName",
                schema: "OrganisationRegistry",
                table: "OrganisationCapacityList",
                column: "CapacityName")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationCapacityList_FunctionName",
                schema: "OrganisationRegistry",
                table: "OrganisationCapacityList",
                column: "FunctionName");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationCapacityList_PersonName",
                schema: "OrganisationRegistry",
                table: "OrganisationCapacityList",
                column: "PersonName");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationCapacityList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationCapacityList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationCapacityList_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationCapacityList",
                column: "ValidTo");

            migrationBuilder.AlterColumn<string>(
                name: "BuildingName",
                schema: "OrganisationRegistry",
                table: "OrganisationBuildingList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationBuildingList",
                schema: "OrganisationRegistry",
                table: "OrganisationBuildingList",
                column: "OrganisationBuildingId")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationBuildingList_BuildingName",
                schema: "OrganisationRegistry",
                table: "OrganisationBuildingList",
                column: "BuildingName")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationBuildingList_IsMainBuilding",
                schema: "OrganisationRegistry",
                table: "OrganisationBuildingList",
                column: "IsMainBuilding");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationBuildingList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationBuildingList",
                column: "ValidFrom");

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationBuildingList_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationBuildingList",
                column: "ValidTo");

            migrationBuilder.AlterColumn<string>(
                name: "ZipCode",
                schema: "OrganisationRegistry",
                table: "LocationList",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Street",
                schema: "OrganisationRegistry",
                table: "LocationList",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "FormattedAddress",
                schema: "OrganisationRegistry",
                table: "LocationList",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                schema: "OrganisationRegistry",
                table: "LocationList",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                schema: "OrganisationRegistry",
                table: "LocationList",
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LocationList",
                schema: "OrganisationRegistry",
                table: "LocationList",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_LocationList_City",
                schema: "OrganisationRegistry",
                table: "LocationList",
                column: "City");

            migrationBuilder.CreateIndex(
                name: "IX_LocationList_Country",
                schema: "OrganisationRegistry",
                table: "LocationList",
                column: "Country");

            migrationBuilder.CreateIndex(
                name: "IX_LocationList_FormattedAddress",
                schema: "OrganisationRegistry",
                table: "LocationList",
                column: "FormattedAddress")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_LocationList_Street",
                schema: "OrganisationRegistry",
                table: "LocationList",
                column: "Street");

            migrationBuilder.CreateIndex(
                name: "IX_LocationList_ZipCode",
                schema: "OrganisationRegistry",
                table: "LocationList",
                column: "ZipCode");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "LabelTypeList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LabelTypeList",
                schema: "OrganisationRegistry",
                table: "LabelTypeList",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_LabelTypeList_Name",
                schema: "OrganisationRegistry",
                table: "LabelTypeList",
                column: "Name",
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "KeyTypeList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_KeyTypeList",
                schema: "OrganisationRegistry",
                table: "KeyTypeList",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_KeyTypeList_Name",
                schema: "OrganisationRegistry",
                table: "KeyTypeList",
                column: "Name",
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "FunctionList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FunctionList",
                schema: "OrganisationRegistry",
                table: "FunctionList",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_FunctionList_Name",
                schema: "OrganisationRegistry",
                table: "FunctionList",
                column: "Name",
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "FormalFrameworkCategoryList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FormalFrameworkCategoryList",
                schema: "OrganisationRegistry",
                table: "FormalFrameworkCategoryList",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_FormalFrameworkCategoryList_Name",
                schema: "OrganisationRegistry",
                table: "FormalFrameworkCategoryList",
                column: "Name",
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "FormalFrameworkList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "FormalFrameworkCategoryName",
                schema: "OrganisationRegistry",
                table: "FormalFrameworkList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "OrganisationRegistry",
                table: "FormalFrameworkList",
                maxLength: 50,
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FormalFrameworkList",
                schema: "OrganisationRegistry",
                table: "FormalFrameworkList",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_FormalFrameworkList_Code",
                schema: "OrganisationRegistry",
                table: "FormalFrameworkList",
                column: "Code");

            migrationBuilder.CreateIndex(
                name: "IX_FormalFrameworkList_FormalFrameworkCategoryName",
                schema: "OrganisationRegistry",
                table: "FormalFrameworkList",
                column: "FormalFrameworkCategoryName");

            migrationBuilder.CreateIndex(
                name: "IX_FormalFrameworkList_Name",
                schema: "OrganisationRegistry",
                table: "FormalFrameworkList",
                column: "Name")
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.CreateIndex(
                name: "IX_FormalFrameworkList_Name_FormalFrameworkCategoryId",
                schema: "OrganisationRegistry",
                table: "FormalFrameworkList",
                columns: new[] { "Name", "FormalFrameworkCategoryId" },
                unique: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "ContactTypeList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContactTypeList",
                schema: "OrganisationRegistry",
                table: "ContactTypeList",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_ContactTypeList_Name",
                schema: "OrganisationRegistry",
                table: "ContactTypeList",
                column: "Name",
                unique: true)
                .Annotation("SqlServer:Clustered", true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "CapacityList",
                maxLength: 500,
                nullable: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CapacityList",
                schema: "OrganisationRegistry",
                table: "CapacityList",
                column: "Id")
                .Annotation("SqlServer:Clustered", false);

            migrationBuilder.CreateIndex(
                name: "IX_CapacityList_Name",
                schema: "OrganisationRegistry",
                table: "CapacityList",
                column: "Name",
                unique: true)
                .Annotation("SqlServer:Clustered", true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_PurposeList",
                schema: "OrganisationRegistry",
                table: "PurposeList");

            migrationBuilder.DropIndex(
                name: "IX_PurposeList_Name",
                schema: "OrganisationRegistry",
                table: "PurposeList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PersonList",
                schema: "OrganisationRegistry",
                table: "PersonList");

            migrationBuilder.DropIndex(
                name: "IX_PersonList_FirstName",
                schema: "OrganisationRegistry",
                table: "PersonList");

            migrationBuilder.DropIndex(
                name: "IX_PersonList_Name",
                schema: "OrganisationRegistry",
                table: "PersonList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PersonFunctionList",
                schema: "OrganisationRegistry",
                table: "PersonFunctionList");

            migrationBuilder.DropIndex(
                name: "IX_PersonFunctionList_FunctionName",
                schema: "OrganisationRegistry",
                table: "PersonFunctionList");

            migrationBuilder.DropIndex(
                name: "IX_PersonFunctionList_OrganisationName",
                schema: "OrganisationRegistry",
                table: "PersonFunctionList");

            migrationBuilder.DropIndex(
                name: "IX_PersonFunctionList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "PersonFunctionList");

            migrationBuilder.DropIndex(
                name: "IX_PersonFunctionList_ValidTo",
                schema: "OrganisationRegistry",
                table: "PersonFunctionList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PersonCapacityList",
                schema: "OrganisationRegistry",
                table: "PersonCapacityList");

            migrationBuilder.DropIndex(
                name: "IX_PersonCapacityList_CapacityName",
                schema: "OrganisationRegistry",
                table: "PersonCapacityList");

            migrationBuilder.DropIndex(
                name: "IX_PersonCapacityList_FunctionName",
                schema: "OrganisationRegistry",
                table: "PersonCapacityList");

            migrationBuilder.DropIndex(
                name: "IX_PersonCapacityList_OrganisationName",
                schema: "OrganisationRegistry",
                table: "PersonCapacityList");

            migrationBuilder.DropIndex(
                name: "IX_PersonCapacityList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "PersonCapacityList");

            migrationBuilder.DropIndex(
                name: "IX_PersonCapacityList_ValidTo",
                schema: "OrganisationRegistry",
                table: "PersonCapacityList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationRelationTypeList",
                schema: "OrganisationRegistry",
                table: "OrganisationRelationTypeList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationRelationTypeList_Name",
                schema: "OrganisationRegistry",
                table: "OrganisationRelationTypeList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationClassificationTypeList",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationTypeList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationClassificationTypeList_Name",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationTypeList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationClassificationList",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationClassificationList_Active",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationClassificationList_Name",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationClassificationList_Order",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationClassificationList_OrganisationClassificationTypeName",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationClassificationList_Name_OrganisationClassificationTypeId",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationParentList",
                schema: "OrganisationRegistry",
                table: "OrganisationParentList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationParentList_ParentOrganisationName",
                schema: "OrganisationRegistry",
                table: "OrganisationParentList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationParentList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationParentList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationParentList_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationParentList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationOrganisationClassificationList",
                schema: "OrganisationRegistry",
                table: "OrganisationOrganisationClassificationList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationOrganisationClassificationList_OrganisationClassificationName",
                schema: "OrganisationRegistry",
                table: "OrganisationOrganisationClassificationList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationOrganisationClassificationList_OrganisationClassificationTypeName",
                schema: "OrganisationRegistry",
                table: "OrganisationOrganisationClassificationList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationOrganisationClassificationList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationOrganisationClassificationList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationOrganisationClassificationList_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationOrganisationClassificationList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationLocationList",
                schema: "OrganisationRegistry",
                table: "OrganisationLocationList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationLocationList_IsMainLocation",
                schema: "OrganisationRegistry",
                table: "OrganisationLocationList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationLocationList_LocationName",
                schema: "OrganisationRegistry",
                table: "OrganisationLocationList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationLocationList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationLocationList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationLocationList_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationLocationList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationList",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationList_Name",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationList_OvoNumber",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationList_ParentOrganisation",
                schema: "OrganisationRegistry",
                table: "OrganisationList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationLabelList",
                schema: "OrganisationRegistry",
                table: "OrganisationLabelList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationLabelList_LabelTypeName",
                schema: "OrganisationRegistry",
                table: "OrganisationLabelList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationLabelList_LabelValue",
                schema: "OrganisationRegistry",
                table: "OrganisationLabelList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationLabelList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationLabelList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationLabelList_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationLabelList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationKeyList",
                schema: "OrganisationRegistry",
                table: "OrganisationKeyList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationKeyList_KeyTypeName",
                schema: "OrganisationRegistry",
                table: "OrganisationKeyList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationKeyList_KeyValue",
                schema: "OrganisationRegistry",
                table: "OrganisationKeyList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationKeyList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationKeyList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationKeyList_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationKeyList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationFunctionList",
                schema: "OrganisationRegistry",
                table: "OrganisationFunctionList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationFunctionList_FunctionName",
                schema: "OrganisationRegistry",
                table: "OrganisationFunctionList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationFunctionList_PersonName",
                schema: "OrganisationRegistry",
                table: "OrganisationFunctionList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationFunctionList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationFunctionList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationFunctionList_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationFunctionList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationFormalFrameworkList",
                schema: "OrganisationRegistry",
                table: "OrganisationFormalFrameworkList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationFormalFrameworkList_FormalFrameworkName",
                schema: "OrganisationRegistry",
                table: "OrganisationFormalFrameworkList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationFormalFrameworkList_ParentOrganisationName",
                schema: "OrganisationRegistry",
                table: "OrganisationFormalFrameworkList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationFormalFrameworkList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationFormalFrameworkList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationFormalFrameworkList_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationFormalFrameworkList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationContactList",
                schema: "OrganisationRegistry",
                table: "OrganisationContactList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationContactList_ContactTypeName",
                schema: "OrganisationRegistry",
                table: "OrganisationContactList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationContactList_ContactValue",
                schema: "OrganisationRegistry",
                table: "OrganisationContactList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationContactList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationContactList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationContactList_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationContactList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationChildList",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationChildList_Name",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationChildList_OrganisationValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationChildList_OrganisationValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationChildList_OvoNumber",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationChildList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationChildList_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationCapacityList",
                schema: "OrganisationRegistry",
                table: "OrganisationCapacityList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationCapacityList_CapacityName",
                schema: "OrganisationRegistry",
                table: "OrganisationCapacityList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationCapacityList_FunctionName",
                schema: "OrganisationRegistry",
                table: "OrganisationCapacityList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationCapacityList_PersonName",
                schema: "OrganisationRegistry",
                table: "OrganisationCapacityList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationCapacityList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationCapacityList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationCapacityList_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationCapacityList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationBuildingList",
                schema: "OrganisationRegistry",
                table: "OrganisationBuildingList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationBuildingList_BuildingName",
                schema: "OrganisationRegistry",
                table: "OrganisationBuildingList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationBuildingList_IsMainBuilding",
                schema: "OrganisationRegistry",
                table: "OrganisationBuildingList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationBuildingList_ValidFrom",
                schema: "OrganisationRegistry",
                table: "OrganisationBuildingList");

            migrationBuilder.DropIndex(
                name: "IX_OrganisationBuildingList_ValidTo",
                schema: "OrganisationRegistry",
                table: "OrganisationBuildingList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LocationList",
                schema: "OrganisationRegistry",
                table: "LocationList");

            migrationBuilder.DropIndex(
                name: "IX_LocationList_City",
                schema: "OrganisationRegistry",
                table: "LocationList");

            migrationBuilder.DropIndex(
                name: "IX_LocationList_Country",
                schema: "OrganisationRegistry",
                table: "LocationList");

            migrationBuilder.DropIndex(
                name: "IX_LocationList_FormattedAddress",
                schema: "OrganisationRegistry",
                table: "LocationList");

            migrationBuilder.DropIndex(
                name: "IX_LocationList_Street",
                schema: "OrganisationRegistry",
                table: "LocationList");

            migrationBuilder.DropIndex(
                name: "IX_LocationList_ZipCode",
                schema: "OrganisationRegistry",
                table: "LocationList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_LabelTypeList",
                schema: "OrganisationRegistry",
                table: "LabelTypeList");

            migrationBuilder.DropIndex(
                name: "IX_LabelTypeList_Name",
                schema: "OrganisationRegistry",
                table: "LabelTypeList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_KeyTypeList",
                schema: "OrganisationRegistry",
                table: "KeyTypeList");

            migrationBuilder.DropIndex(
                name: "IX_KeyTypeList_Name",
                schema: "OrganisationRegistry",
                table: "KeyTypeList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FunctionList",
                schema: "OrganisationRegistry",
                table: "FunctionList");

            migrationBuilder.DropIndex(
                name: "IX_FunctionList_Name",
                schema: "OrganisationRegistry",
                table: "FunctionList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FormalFrameworkCategoryList",
                schema: "OrganisationRegistry",
                table: "FormalFrameworkCategoryList");

            migrationBuilder.DropIndex(
                name: "IX_FormalFrameworkCategoryList_Name",
                schema: "OrganisationRegistry",
                table: "FormalFrameworkCategoryList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FormalFrameworkList",
                schema: "OrganisationRegistry",
                table: "FormalFrameworkList");

            migrationBuilder.DropIndex(
                name: "IX_FormalFrameworkList_Code",
                schema: "OrganisationRegistry",
                table: "FormalFrameworkList");

            migrationBuilder.DropIndex(
                name: "IX_FormalFrameworkList_FormalFrameworkCategoryName",
                schema: "OrganisationRegistry",
                table: "FormalFrameworkList");

            migrationBuilder.DropIndex(
                name: "IX_FormalFrameworkList_Name",
                schema: "OrganisationRegistry",
                table: "FormalFrameworkList");

            migrationBuilder.DropIndex(
                name: "IX_FormalFrameworkList_Name_FormalFrameworkCategoryId",
                schema: "OrganisationRegistry",
                table: "FormalFrameworkList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ContactTypeList",
                schema: "OrganisationRegistry",
                table: "ContactTypeList");

            migrationBuilder.DropIndex(
                name: "IX_ContactTypeList_Name",
                schema: "OrganisationRegistry",
                table: "ContactTypeList");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CapacityList",
                schema: "OrganisationRegistry",
                table: "CapacityList");

            migrationBuilder.DropIndex(
                name: "IX_CapacityList_Name",
                schema: "OrganisationRegistry",
                table: "CapacityList");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "PurposeList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PurposeList",
                schema: "OrganisationRegistry",
                table: "PurposeList",
                column: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "PersonList",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FirstName",
                schema: "OrganisationRegistry",
                table: "PersonList",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PersonList",
                schema: "OrganisationRegistry",
                table: "PersonList",
                column: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "OrganisationName",
                schema: "OrganisationRegistry",
                table: "PersonFunctionList",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FunctionName",
                schema: "OrganisationRegistry",
                table: "PersonFunctionList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PersonFunctionList",
                schema: "OrganisationRegistry",
                table: "PersonFunctionList",
                column: "OrganisationFunctionId");

            migrationBuilder.AlterColumn<Guid>(
                name: "PersonId",
                schema: "OrganisationRegistry",
                table: "PersonCapacityList",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrganisationName",
                schema: "OrganisationRegistry",
                table: "PersonCapacityList",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CapacityName",
                schema: "OrganisationRegistry",
                table: "PersonCapacityList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_PersonCapacityList",
                schema: "OrganisationRegistry",
                table: "PersonCapacityList",
                column: "OrganisationCapacityId");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "OrganisationRelationTypeList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationRelationTypeList",
                schema: "OrganisationRegistry",
                table: "OrganisationRelationTypeList",
                column: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationTypeList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationClassificationTypeList",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationTypeList",
                column: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "OrganisationClassificationTypeName",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationClassificationList",
                schema: "OrganisationRegistry",
                table: "OrganisationClassificationList",
                column: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "ParentOrganisationName",
                schema: "OrganisationRegistry",
                table: "OrganisationParentList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationParentList",
                schema: "OrganisationRegistry",
                table: "OrganisationParentList",
                column: "OrganisationOrganisationParentId");

            migrationBuilder.AlterColumn<string>(
                name: "OrganisationClassificationTypeName",
                schema: "OrganisationRegistry",
                table: "OrganisationOrganisationClassificationList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OrganisationClassificationName",
                schema: "OrganisationRegistry",
                table: "OrganisationOrganisationClassificationList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationOrganisationClassificationList",
                schema: "OrganisationRegistry",
                table: "OrganisationOrganisationClassificationList",
                column: "OrganisationOrganisationClassificationId");

            migrationBuilder.AlterColumn<string>(
                name: "LocationName",
                schema: "OrganisationRegistry",
                table: "OrganisationLocationList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationLocationList",
                schema: "OrganisationRegistry",
                table: "OrganisationLocationList",
                column: "OrganisationLocationId");

            migrationBuilder.AlterColumn<string>(
                name: "ParentOrganisation",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "OvoNumber",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationList",
                schema: "OrganisationRegistry",
                table: "OrganisationList",
                column: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "LabelValue",
                schema: "OrganisationRegistry",
                table: "OrganisationLabelList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LabelTypeName",
                schema: "OrganisationRegistry",
                table: "OrganisationLabelList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationLabelList",
                schema: "OrganisationRegistry",
                table: "OrganisationLabelList",
                column: "OrganisationLabelId");

            migrationBuilder.AlterColumn<string>(
                name: "KeyValue",
                schema: "OrganisationRegistry",
                table: "OrganisationKeyList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "KeyTypeName",
                schema: "OrganisationRegistry",
                table: "OrganisationKeyList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationKeyList",
                schema: "OrganisationRegistry",
                table: "OrganisationKeyList",
                column: "OrganisationKeyId");

            migrationBuilder.AlterColumn<string>(
                name: "PersonName",
                schema: "OrganisationRegistry",
                table: "OrganisationFunctionList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FunctionName",
                schema: "OrganisationRegistry",
                table: "OrganisationFunctionList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationFunctionList",
                schema: "OrganisationRegistry",
                table: "OrganisationFunctionList",
                column: "OrganisationFunctionId");

            migrationBuilder.AlterColumn<string>(
                name: "ParentOrganisationName",
                schema: "OrganisationRegistry",
                table: "OrganisationFormalFrameworkList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FormalFrameworkName",
                schema: "OrganisationRegistry",
                table: "OrganisationFormalFrameworkList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationFormalFrameworkList",
                schema: "OrganisationRegistry",
                table: "OrganisationFormalFrameworkList",
                column: "OrganisationFormalFrameworkId");

            migrationBuilder.AlterColumn<string>(
                name: "ContactValue",
                schema: "OrganisationRegistry",
                table: "OrganisationContactList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ContactTypeName",
                schema: "OrganisationRegistry",
                table: "OrganisationContactList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationContactList",
                schema: "OrganisationRegistry",
                table: "OrganisationContactList",
                column: "OrganisationContactId");

            migrationBuilder.AlterColumn<string>(
                name: "OvoNumber",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList",
                nullable: false);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationChildList",
                schema: "OrganisationRegistry",
                table: "OrganisationChildList",
                column: "OrganisationOrganisationParentId");

            migrationBuilder.AlterColumn<string>(
                name: "CapacityName",
                schema: "OrganisationRegistry",
                table: "OrganisationCapacityList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationCapacityList",
                schema: "OrganisationRegistry",
                table: "OrganisationCapacityList",
                column: "OrganisationCapacityId");

            migrationBuilder.AlterColumn<string>(
                name: "BuildingName",
                schema: "OrganisationRegistry",
                table: "OrganisationBuildingList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationBuildingList",
                schema: "OrganisationRegistry",
                table: "OrganisationBuildingList",
                column: "OrganisationBuildingId");

            migrationBuilder.AlterColumn<string>(
                name: "ZipCode",
                schema: "OrganisationRegistry",
                table: "LocationList",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Street",
                schema: "OrganisationRegistry",
                table: "LocationList",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FormattedAddress",
                schema: "OrganisationRegistry",
                table: "LocationList",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Country",
                schema: "OrganisationRegistry",
                table: "LocationList",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "City",
                schema: "OrganisationRegistry",
                table: "LocationList",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LocationList",
                schema: "OrganisationRegistry",
                table: "LocationList",
                column: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "LabelTypeList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_LabelTypeList",
                schema: "OrganisationRegistry",
                table: "LabelTypeList",
                column: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "KeyTypeList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_KeyTypeList",
                schema: "OrganisationRegistry",
                table: "KeyTypeList",
                column: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "FunctionList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FunctionList",
                schema: "OrganisationRegistry",
                table: "FunctionList",
                column: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "FormalFrameworkCategoryList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FormalFrameworkCategoryList",
                schema: "OrganisationRegistry",
                table: "FormalFrameworkCategoryList",
                column: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "FormalFrameworkList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FormalFrameworkCategoryName",
                schema: "OrganisationRegistry",
                table: "FormalFrameworkList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                schema: "OrganisationRegistry",
                table: "FormalFrameworkList",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FormalFrameworkList",
                schema: "OrganisationRegistry",
                table: "FormalFrameworkList",
                column: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "ContactTypeList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ContactTypeList",
                schema: "OrganisationRegistry",
                table: "ContactTypeList",
                column: "Id");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "OrganisationRegistry",
                table: "CapacityList",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CapacityList",
                schema: "OrganisationRegistry",
                table: "CapacityList",
                column: "Id");
        }
    }
}
