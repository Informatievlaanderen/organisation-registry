import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { OrganisationInfoServiceModule } from 'services';

import { OrganisationInfoModule } from 'organisation/info';
import { OrganisationBodiesModule } from 'organisation/bodies';
import { OrganisationBuildingsModule } from 'organisation/buildings';
import { OrganisationCapacitiesModule } from 'organisation/capacities';
import { OrganisationContactsModule } from 'organisation/contacts';
import { OrganisationFunctionsModule } from 'organisation/functions';
import { OrganisationClassificationsModule } from 'organisation/classifications';
import { OrganisationKeysModule } from 'organisation/keys';
import { OrganisationRegulationsModule } from 'organisation/regulations';
import { OrganisationLabelsModule } from 'organisation/labels';
import { OrganisationLocationsModule } from 'organisation/locations';
import { OrganisationParentsModule } from 'organisation/parents';
import { OrganisationFormalFrameworksModule } from 'organisation/formalframeworks';
import { OrganisationBankAccountsModule } from 'organisation/bankaccounts';
import { OrganisationRelationsModule } from 'organisation/relations';
import { OrganisationOpeningHoursModule } from 'organisation/openinghours';
import { OrganisationManagementModule } from "organisation/management";

import { OrganisationDetailRoutingModule } from './detail-routing.module';
import { OrganisationDetailComponent } from './detail.component';

@NgModule({
  imports: [
    SharedModule,
    OrganisationDetailRoutingModule,

    OrganisationInfoModule,
    OrganisationInfoServiceModule,

    OrganisationBodiesModule,
    OrganisationBuildingsModule,
    OrganisationCapacitiesModule,
    OrganisationContactsModule,
    OrganisationFunctionsModule,
    OrganisationClassificationsModule,
    OrganisationKeysModule,
    OrganisationRegulationsModule,
    OrganisationLabelsModule,
    OrganisationLabelsModule,
    OrganisationLocationsModule,
    OrganisationParentsModule,
    OrganisationFormalFrameworksModule,
    OrganisationBankAccountsModule,
    OrganisationRelationsModule,
    OrganisationOpeningHoursModule,
    OrganisationManagementModule
  ],
  declarations: [
    OrganisationDetailComponent
  ],
  exports: [
    OrganisationDetailRoutingModule
  ]
})
export class OrganisationDetailModule { }
