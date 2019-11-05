import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { BodyInfoRoutingModule } from './info-routing.module';

import { BodyInfoComponent } from './info.component';
import { BodyInfoOverviewComponent } from './overview';
import { BodyInfoValidityComponent } from './validity';
import { BodyInfoGeneralComponent } from './general';

@NgModule({
  imports: [
    SharedModule,
    BodyInfoRoutingModule,
  ],
  declarations: [
    BodyInfoComponent,
    BodyInfoOverviewComponent,
    BodyInfoValidityComponent,
    BodyInfoGeneralComponent
  ],
  exports: [
    BodyInfoRoutingModule
  ]
})
export class BodyInfoModule { }
