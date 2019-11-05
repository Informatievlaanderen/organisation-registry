import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { ReportsServiceModule } from 'services/reports';

import { OrganisationClassificationParticipationRoutingModule } from './organisationclassification-participation-routing.module';

import { OrganisationClassificationParticipationOverviewComponent } from './overview';
import { OrganisationClassificationParticipationDetailComponent } from './detail';

import {
    OrganisationClassificationListComponent,
    OrganisationClassificationFilterComponent,
    OrganisationClassificationParticipationListComponent
} from './components/';

@NgModule({
    imports: [
        SharedModule,
        OrganisationClassificationParticipationRoutingModule,
        ReportsServiceModule
    ],
    declarations: [
        OrganisationClassificationParticipationOverviewComponent,
        OrganisationClassificationListComponent,
        OrganisationClassificationFilterComponent,
        OrganisationClassificationParticipationDetailComponent,
        OrganisationClassificationParticipationListComponent
    ],
    exports: [
        OrganisationClassificationParticipationRoutingModule
    ]
})
export class ReportOrganisationClassificationParticipationModule { }