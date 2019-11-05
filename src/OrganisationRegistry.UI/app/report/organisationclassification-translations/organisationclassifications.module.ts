import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { ReportsServiceModule } from 'services/reports';
import { OrganisationClassificationsRoutingModule } from './organisationclassifications-routing.module';

import { OrganisationClassificationOverviewComponent } from './overview';
import { OrganisationClassificationDetailComponent } from './detail';

import {
    OrganisationClassificationListComponent,
    OrganisationClassificationFilterComponent,
    ClassificationOrganisationListComponent
} from './components/';

@NgModule({
    imports: [
        SharedModule,
        OrganisationClassificationsRoutingModule,
        ReportsServiceModule
    ],
    declarations: [
        OrganisationClassificationOverviewComponent,
        OrganisationClassificationListComponent,
        OrganisationClassificationFilterComponent,
        OrganisationClassificationDetailComponent,
        ClassificationOrganisationListComponent
    ],
    exports: [
        OrganisationClassificationsRoutingModule
    ]
})
export class ReportOrganisationClassificationsModule { }
