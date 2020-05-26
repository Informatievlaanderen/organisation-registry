import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { ReportsServiceModule } from 'services/reports';

import { ParticipationSummaryRoutingModule } from './participation-summary-routing.module';

import { ParticipationSummaryListComponent } from './components/participation-summary-list';
import { ParticipationSummaryDetailComponent } from './detail';

@NgModule({
    imports: [
        SharedModule,
        ParticipationSummaryRoutingModule,
        ReportsServiceModule
    ],
    declarations: [
        ParticipationSummaryDetailComponent,
        ParticipationSummaryListComponent
    ],
    exports: [
        ParticipationSummaryRoutingModule
    ]
})
export class ReportParticipationSummaryModule { }
