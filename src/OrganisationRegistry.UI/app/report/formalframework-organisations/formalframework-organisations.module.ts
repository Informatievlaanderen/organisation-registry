import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { ReportsServiceModule } from 'services/reports';
import { FormalFrameworkOrganisationsRoutingModule } from './formalframework-organisations-routing.module';

import { FormalFrameworkOverviewComponent } from './overview';
import { FormalFrameworkDetailComponent } from './detail';

import {
    FormalFrameworkListComponent,
    FormalFrameworkFilterComponent,
    FormalFrameworkOrganisationListComponent
} from './components/';

@NgModule({
    imports: [
        SharedModule,
        FormalFrameworkOrganisationsRoutingModule,
        ReportsServiceModule
    ],
    declarations: [
        FormalFrameworkOverviewComponent,
        FormalFrameworkListComponent,
        FormalFrameworkFilterComponent,
        FormalFrameworkDetailComponent,
        FormalFrameworkOrganisationListComponent
    ],
    exports: [
        FormalFrameworkOrganisationsRoutingModule
    ]
})
export class ReportFormalFrameworkOrganisationsModule { }
