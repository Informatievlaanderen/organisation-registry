import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { CapacityPersonReportService } from './capacity-persons/capacity-person-report.service';

import { BodyParticipationReportService } from './body-participation/body-participation-report.service';

import { FormalFrameworkParticipationReportService } from './formalframework-participation/formalframework-participation-report.service';
import { FormalFrameworkBodyReportService } from './formalframework-bodies/formalframework-body-report.service';
import { FormalFrameworkOrganisationReportService } from './formalframework-organisations/formalframework-organisation-report.service';
import { FormalFrameworkOrganisationVademecumReportService } from './formalframework-organisations-vademecum/formalframework-organisation-vademecum-report.service';

import { OrganisationClassificationReportService } from './organisationclassifications/organisation-classification-report.service';
import { OrganisationClassificationParticipationReportService } from './organisationclassification-participation/organisationclassification-participation-report.service';

import { BuildingOrganisationReportService } from './building-organisations/building-organisation-report.service';

@NgModule({
  declarations: [
  ],
  providers: [
    CapacityPersonReportService,
    BodyParticipationReportService,
    FormalFrameworkBodyReportService,
    FormalFrameworkOrganisationReportService,
    FormalFrameworkOrganisationVademecumReportService,
    FormalFrameworkParticipationReportService,
    OrganisationClassificationReportService,
    OrganisationClassificationParticipationReportService,
    BuildingOrganisationReportService
  ],
  exports: [
  ]
})
export class ReportsServiceModule { }
