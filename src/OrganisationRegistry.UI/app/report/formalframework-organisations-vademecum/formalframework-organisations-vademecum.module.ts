import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { ReportsServiceModule } from 'services/reports';
import { FormalFrameworkOrganisationsRoutingModule } from './formalframework-organisations-vademecum-routing.module';

import { FormalFrameworkOverviewComponent } from './overview';
import { FormalFrameworkDetailComponent } from './detail';

import {
    FormalFrameworkListComponent,
    FormalFrameworkFilterComponent,
    FormalFrameworkOrganisationVademecumListComponent
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
        FormalFrameworkOrganisationVademecumListComponent
    ],
    exports: [
        FormalFrameworkOrganisationsRoutingModule
    ]
})
export class ReportFormalFrameworkOrganisationsVademecumModule { }
