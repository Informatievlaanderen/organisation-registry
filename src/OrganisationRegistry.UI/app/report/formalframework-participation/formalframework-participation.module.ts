import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { ReportsServiceModule } from 'services/reports';

import { FormalFrameworkParticipationRoutingModule } from './formalframework-participation-routing.module';

import { FormalFrameworkParticipationOverviewComponent } from './overview';
import { FormalFrameworkParticipationDetailComponent } from './detail';

import {
    FormalFrameworkListComponent,
    FormalFrameworkFilterComponent,
    FormalFrameworkParticipationListComponent
} from './components/';

@NgModule({
    imports: [
        SharedModule,
        FormalFrameworkParticipationRoutingModule,
        ReportsServiceModule
    ],
    declarations: [
        FormalFrameworkParticipationOverviewComponent,
        FormalFrameworkListComponent,
        FormalFrameworkFilterComponent,
        FormalFrameworkParticipationDetailComponent,
        FormalFrameworkParticipationListComponent
    ],
    exports: [
        FormalFrameworkParticipationRoutingModule
    ]
})
export class ReportFormalFrameworkParticipationModule { }