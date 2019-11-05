import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { DashboardRoutingModule } from './dashboard-routing.module';

import { ReportCapacityPersonsModule } from './../capacity-persons';

import { ReportOrganisationClassificationsModule } from './../organisationclassification-translations';
import { ReportOrganisationClassificationParticipationModule } from './../organisationclassification-participation';

import { ReportFormalFrameworkOrganisationsModule } from './../formalframework-organisations';
import { ReportFormalFrameworkBodiesModule } from './../formalframework-bodies';
import { ReportFormalFrameworkOrganisationsVademecumModule } from './../formalframework-organisations-vademecum';
import { ReportFormalFrameworkParticipationModule } from './../formalframework-participation';

import { ReportBuildingOrganisationsModule } from './../building-organisations';

import { DashboardOverviewComponent } from './overview';

@NgModule({
  imports: [
    SharedModule,
    DashboardRoutingModule,
    ReportCapacityPersonsModule,
    ReportFormalFrameworkOrganisationsModule,
    ReportFormalFrameworkBodiesModule,
    ReportFormalFrameworkOrganisationsVademecumModule,
    ReportFormalFrameworkParticipationModule,
    ReportOrganisationClassificationsModule,
    ReportOrganisationClassificationParticipationModule,
    ReportBuildingOrganisationsModule
  ],
  declarations: [
    DashboardOverviewComponent
  ],
  exports: [
      DashboardRoutingModule
  ]
})
export class ReportDashboardModule { }