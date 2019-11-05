import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { ReportsServiceModule } from 'services/reports';
import { FormalFrameworkBodiesRoutingModule } from './formalframework-bodies-routing.module';

import { FormalFrameworkOverviewComponent } from './overview';
import { FormalFrameworkDetailComponent } from './detail';

import {
    FormalFrameworkListComponent,
    FormalFrameworkFilterComponent,
    FormalFrameworkBodyListComponent
} from './components/';

@NgModule({
    imports: [
        SharedModule,
        FormalFrameworkBodiesRoutingModule,
        ReportsServiceModule
    ],
    declarations: [
        FormalFrameworkOverviewComponent,
        FormalFrameworkListComponent,
        FormalFrameworkFilterComponent,
        FormalFrameworkDetailComponent,
        FormalFrameworkBodyListComponent
    ],
    exports: [
        FormalFrameworkBodiesRoutingModule
    ]
})
export class ReportFormalFrameworkBodiesModule { }
