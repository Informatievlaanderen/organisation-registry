import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { ReportsServiceModule } from 'services/reports';

import { CapacityPersonsRoutingModule } from './capacity-persons-routing.module';

import { CapacityOverviewComponent } from './overview';
import { CapacityDetailComponent } from './detail';

import {
    CapacityListComponent,
    CapacityFilterComponent,
    CapacityPersonListComponent
} from './components/';

@NgModule({
    imports: [
        SharedModule,
        CapacityPersonsRoutingModule,
        ReportsServiceModule
    ],
    declarations: [
        CapacityOverviewComponent,
        CapacityListComponent,
        CapacityFilterComponent,
        CapacityDetailComponent,
        CapacityPersonListComponent
    ],
    exports: [
        CapacityPersonsRoutingModule
    ]
})
export class ReportCapacityPersonsModule { }
