import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { AdministrationBodyClassificationsModule } from '../bodyclassifications'
import { AdministrationBodyClassificationTypesModule } from '../bodyclassificationtypes'
import { AdministrationBuildingsModule } from '../buildings';
import { AdministrationCapacitiesModule } from '../capacities';
import { AdministrationContactTypesModule } from '../contacttypes';
import { AdministrationFormalFrameworkCategoriesModule } from '../formalframeworkcategories';
import { AdministrationFormalFrameworksModule } from '../formalframeworks';
import { AdministrationFunctionsModule } from '../functions';
import { AdministrationKeyTypesModule } from '../keytypes';
import { AdministrationLabelTypesModule } from '../labeltypes';
import { AdministrationLifecyclePhaseTypesModule } from '../lifecyclephasetypes';
import { AdministrationLocationsModule } from '../locations';
import { AdministrationLocationTypesModule } from '../locationtypes';
import { AdministrationMandateRoleTypesModule } from '../mandateroletypes';
import { AdministrationOrganisationClassificationsModule } from '../organisationclassifications';
import { AdministrationOrganisationClassificationTypesModule } from '../organisationclassificationtypes';
import { AdministrationOrganisationRelationTypesModule } from '../organisationrelationtypes';
import { AdministrationPeopleModule } from '../people';
import { AdministrationPurposesModule } from '../purposes';
import { AdministrationSeatTypesModule } from '../seattypes';
import {AdministrationRegulationTypesModule} from "../regulationtypes";

import { DashboardRoutingModule } from './dashboard-routing.module';

import { DashboardOverviewComponent } from './overview';

@NgModule({
  imports: [
    SharedModule,
    DashboardRoutingModule,

    AdministrationBodyClassificationsModule,
    AdministrationBodyClassificationTypesModule,
    AdministrationBuildingsModule,
    AdministrationCapacitiesModule,
    AdministrationContactTypesModule,
    AdministrationFormalFrameworkCategoriesModule,
    AdministrationFormalFrameworksModule,
    AdministrationFunctionsModule,
    AdministrationKeyTypesModule,
    AdministrationLabelTypesModule,
    AdministrationLifecyclePhaseTypesModule,
    AdministrationLocationsModule,
    AdministrationLocationTypesModule,
    AdministrationMandateRoleTypesModule,
    AdministrationOrganisationClassificationsModule,
    AdministrationOrganisationClassificationTypesModule,
    AdministrationOrganisationRelationTypesModule,
    AdministrationPeopleModule,
    AdministrationPurposesModule,
    AdministrationSeatTypesModule,
    AdministrationRegulationTypesModule
  ],
  declarations: [
    DashboardOverviewComponent
  ],
  exports: [
    DashboardRoutingModule
  ]
})
export class AdministrationDashboardModule { }
