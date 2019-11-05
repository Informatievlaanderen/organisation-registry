import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { TogglesServiceModule } from 'services/toggles';
import { TogglesRoutingModule } from './toggles-routing.module';

import { TogglesOverviewComponent } from './overview';

@NgModule({
  imports: [
    SharedModule,
    TogglesRoutingModule,
    TogglesServiceModule
  ],
  declarations: [
    TogglesOverviewComponent
  ],
  exports: [
    TogglesRoutingModule
  ]
})
export class SystemTogglesModule { }
