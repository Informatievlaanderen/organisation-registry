import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { FormalFrameworksServiceModule } from 'services/formalframeworks';
import { FormalFrameworksRoutingModule } from './formal-frameworks-routing.module';

import { FormalFrameworkDetailComponent } from './detail';
import { FormalFrameworkOverviewComponent } from './overview';

import {
  FormalFrameworkListComponent,
  FormalFrameworkFilterComponent
} from './components/';

@NgModule({
  imports: [
    SharedModule,
    FormalFrameworksRoutingModule,
    FormalFrameworksServiceModule
  ],
  declarations: [
    FormalFrameworkDetailComponent,
    FormalFrameworkListComponent,
    FormalFrameworkOverviewComponent,
    FormalFrameworkFilterComponent
  ],
  exports: [
    FormalFrameworksRoutingModule
  ]
})
export class AdministrationFormalFrameworksModule { }
