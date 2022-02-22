# Rebuilder

## How to use


### Copy events from production to local file.

```
bcp OrganisationRegistry.Events out ./events-prd.bcp -S<server> -U <user> -d organisation-registry -n -E
```

### Stop all services in staging environment.

```
UPDATE [organisation-registry].OrganisationRegistry.Configuration
SET Value = N'false'
WHERE [Key] LIKE N'Toggles:ReportingRunnerAvailable' ESCAPE '#';

UPDATE [organisation-registry].OrganisationRegistry.Configuration
SET Value = N'false'
WHERE [Key] LIKE N'Toggles:DelegationsRunnerAvailable' ESCAPE '#';

UPDATE [organisation-registry].OrganisationRegistry.Configuration
SET Value = N'false'
WHERE [Key] LIKE N'Toggles:ApplicationAvailable' ESCAPE '#';

UPDATE [organisation-registry].OrganisationRegistry.Configuration
SET Value = N'false'
WHERE [Key] LIKE N'Toggles:ElasticSearchProjectionsAvailable' ESCAPE '#';

UPDATE [organisation-registry].OrganisationRegistry.Configuration
SET Value = N'false'
WHERE [Key] LIKE N'Toggles:ApiAvailable' ESCAPE '#';
```

Stop all running services (they will restart but will not be available).

### Clear data in staging environment.

Clear events table.

```
truncate table OrganisationRegistry.Events
```

Reset all projections.

```
TRUNCATE TABLE [Backoffice].[PurposeList];
TRUNCATE TABLE [Backoffice].[OrganisationFormalFrameworkList];
TRUNCATE TABLE [Backoffice].[OrganisationTerminationList];
TRUNCATE TABLE [Backoffice].[OrganisationFormalFrameworkValidity];
TRUNCATE TABLE [Backoffice].[BodySeatCacheForBodyMandateList];
TRUNCATE TABLE [Backoffice].[OrganisationClassificationValidity];
TRUNCATE TABLE [Backoffice].[OrganisationBankAccountList];
TRUNCATE TABLE [Backoffice].[LocationTypeList];
TRUNCATE TABLE [Backoffice].[ActiveOrganisationFormalFrameworkList];
TRUNCATE TABLE [Backoffice].[ActiveOrganisationParentList];
TRUNCATE TABLE [Backoffice].[BodyOrganisationList];
TRUNCATE TABLE [Backoffice].[LocationList];
TRUNCATE TABLE [Backoffice].[FutureActiveOrganisationFormalFrameworkList];
TRUNCATE TABLE [Backoffice].[BuildingList];
TRUNCATE TABLE [Backoffice].[FutureActiveOrganisationParentList];
TRUNCATE TABLE [Backoffice].[PersonList];
TRUNCATE TABLE [Backoffice].[FormalFrameworkCategoryList];
TRUNCATE TABLE [Backoffice].[BodyFormalFrameworkList];
TRUNCATE TABLE [Backoffice].[OrganisationClassificationTypeList];
TRUNCATE TABLE [Backoffice].[BodyDetail];
TRUNCATE TABLE [Backoffice].[FormalFrameworkList];
TRUNCATE TABLE [Backoffice].[OrganisationRegulationList];
TRUNCATE TABLE [Backoffice].[BodyContactList];
TRUNCATE TABLE [Backoffice].[CapacityList];
TRUNCATE TABLE [Backoffice].[RegulationThemeList];
TRUNCATE TABLE [Backoffice].[OrganisationRelationList];
TRUNCATE TABLE [Backoffice].[FunctionList];
TRUNCATE TABLE [Backoffice].[OrganisationBodyList];
TRUNCATE TABLE [Backoffice].[ActiveBodyOrganisationList];
TRUNCATE TABLE [Backoffice].[OrganisationDetail];
TRUNCATE TABLE [Backoffice].[OrganisationOpeningHourList];
TRUNCATE TABLE [Backoffice].[FutureActiveBodyOrganisationList];
TRUNCATE TABLE [Backoffice].[BodyLifecyclePhaseList];
TRUNCATE TABLE [Backoffice].[OrganisationKeyList];
TRUNCATE TABLE [Backoffice].[LifecyclePhaseTypeList];
TRUNCATE TABLE [Backoffice].[OrganisationClassificationList];
TRUNCATE TABLE [Backoffice].[BodyClassificationTypeList];
TRUNCATE TABLE [Backoffice].[BodyClassificationList];
TRUNCATE TABLE [Backoffice].[BodyLifecyclePhaseValidity];
TRUNCATE TABLE [Backoffice].[BodyBodyClassificationList];
TRUNCATE TABLE [Backoffice].[OrganisationChildList];
TRUNCATE TABLE [Backoffice].[BodySeatList];
TRUNCATE TABLE [Backoffice].[SeatTypeList];
TRUNCATE TABLE [Backoffice].[MandateRoleTypeList];
TRUNCATE TABLE [Backoffice].[OrganisationBuildingList];
TRUNCATE TABLE [Backoffice].[ContactTypeList];
TRUNCATE TABLE [Backoffice].[BodyMandateList];
TRUNCATE TABLE [Backoffice].[LabelTypeList];
TRUNCATE TABLE [Backoffice].[OrganisationRelationTypeList];
TRUNCATE TABLE [Backoffice].[PersonMandateList];
TRUNCATE TABLE [Backoffice].[OrganisationLocationList];
TRUNCATE TABLE [Backoffice].[DelegationList];
TRUNCATE TABLE [Backoffice].[OrganisationContactList];
TRUNCATE TABLE [Backoffice].[OrganisationLabelList];
TRUNCATE TABLE [Backoffice].[KeyTypeList];
TRUNCATE TABLE [Backoffice].[OrganisationOrganisationClassificationList];
TRUNCATE TABLE [Backoffice].[OrganisationPerBodyList];
TRUNCATE TABLE [Backoffice].[OrganisationFunctionList];
TRUNCATE TABLE [Backoffice].[DelegationAssignmentList];
TRUNCATE TABLE [Backoffice].[PersonFunctionList];
TRUNCATE TABLE [Backoffice].[OrganisationCapacityList];
TRUNCATE TABLE [Backoffice].[FuturePeopleAssignedToBodyMandatesList];
TRUNCATE TABLE [Backoffice].[PersonCapacityList];
TRUNCATE TABLE [Backoffice].[ActivePeopleAssignedToBodyMandatesList];
TRUNCATE TABLE [Backoffice].[OrganisationParentList];
TRUNCATE TABLE [Backoffice].[OrganisationTreeList];
TRUNCATE TABLE [Backoffice].[RegulationSubThemeList];
delete from [Backoffice].[BodyList];
delete from [Backoffice].[OrganisationList];
```

### Copy events from local file to staging environment.

```
bcp OrganisationRegistry.Events in ./events-prd.bcp -S<server> -d organisation-registry -U <user> -n -E
```

### Clear the projection states.
```
TODO

Don't reset vlaanderenbenotifier!
```


### Run the Rebuilder

Everything is now ready to rebuild.

Make sure there is a projection state item set at -1 in the staging db.
```
INSERT INTO [organisation-registry].Backoffice.ProjectionStateList (Id, EventNumber, Name, LastUpdatedUtc)
VALUES (N'603C9B4C-BF3F-490B-959D-766816205407', -1, N'OrganisationRegistry.Rebuilder', null);
```

Now run the rebuilder. Wait for it to complete.

### Start all services in staging environment.

```
UPDATE [organisation-registry].OrganisationRegistry.Configuration
SET Value = N'true'
WHERE [Key] LIKE N'Toggles:ReportingRunnerAvailable' ESCAPE '#';

UPDATE [organisation-registry].OrganisationRegistry.Configuration
SET Value = N'true'
WHERE [Key] LIKE N'Toggles:DelegationsRunnerAvailable' ESCAPE '#';

UPDATE [organisation-registry].OrganisationRegistry.Configuration
SET Value = N'true'
WHERE [Key] LIKE N'Toggles:ApplicationAvailable' ESCAPE '#';

UPDATE [organisation-registry].OrganisationRegistry.Configuration
SET Value = N'true'
WHERE [Key] LIKE N'Toggles:ElasticSearchProjectionsAvailable' ESCAPE '#';

UPDATE [organisation-registry].OrganisationRegistry.Configuration
SET Value = N'true'
WHERE [Key] LIKE N'Toggles:ApiAvailable' ESCAPE '#';
```

Stop all running services (they will restart).