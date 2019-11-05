import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { BodiesServiceModule } from 'services/bodies';
// import { BodyResponsibilitiesServiceModule } from 'services/bodyresponsibilities';

import { BodyResponsibilitiesRoutingModule } from './responsibilities-routing.module';

import { BodyResponsibilitiesComponent } from './responsibilities.component';

@NgModule({
  imports: [
    SharedModule,
    BodyResponsibilitiesRoutingModule,
    // BodyResponsibilitiesServiceModule,
    BodiesServiceModule
  ],
  declarations: [
    BodyResponsibilitiesComponent,
  ],
  exports: [
    BodyResponsibilitiesRoutingModule
  ]
})
export class BodyResponsibilitiesModule { }
