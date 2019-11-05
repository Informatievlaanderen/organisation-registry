import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { BodiesServiceModule } from 'services/bodies';
import { BodySeatsServiceModule } from 'services/bodyseats';
import { BodySeatsOverviewComponent } from './overview';
import { BodySeatsListComponent } from './list';
import { BodySeatFilterComponent } from './filter';
import { BodyCompositionComponent } from './composition.component';
import { BodyCompositionRoutingModule } from './composition-routing.module';
import { BodySeatsCreateBodySeatComponent } from './create';
import { BodySeatsUpdateBodySeatComponent } from './update';

@NgModule({
  imports: [
    SharedModule,
    BodyCompositionRoutingModule,
    BodySeatsServiceModule,
    BodiesServiceModule
  ],
  declarations: [
    BodyCompositionComponent,
    BodySeatsOverviewComponent,
    BodySeatsListComponent,
    BodySeatFilterComponent,
    BodySeatsCreateBodySeatComponent,
    BodySeatsUpdateBodySeatComponent
  ],
  exports: [
    BodyCompositionRoutingModule
  ]
})
export class BodyCompositionModule { }
