import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { LifecyclePhaseTypesServiceModule } from 'services/lifecyclephasetypes';
import { LifecyclePhaseTypesRoutingModule } from './lifecycle-phase-types-routing.module';

import { LifecyclePhaseTypeDetailComponent } from './detail';
import { LifecyclePhaseTypeOverviewComponent } from './overview';

import {
  LifecyclePhaseTypeListComponent,
  LifecyclePhaseTypeFilterComponent
} from './components/';

@NgModule({
  imports: [
    SharedModule,
    LifecyclePhaseTypesRoutingModule,
    LifecyclePhaseTypesServiceModule
  ],
  declarations: [
    LifecyclePhaseTypeDetailComponent,
    LifecyclePhaseTypeListComponent,
    LifecyclePhaseTypeOverviewComponent,
    LifecyclePhaseTypeFilterComponent
  ],
  exports: [
    LifecyclePhaseTypesRoutingModule
  ]
})
export class AdministrationLifecyclePhaseTypesModule { }
