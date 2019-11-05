import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { BodySeatService } from './body-seat.service';
import { BodySeatResolver } from './body-seat.resolver';

@NgModule({
  declarations: [
  ],
  providers: [
    BodySeatService,
    BodySeatResolver
  ],
  exports: [
  ]
})
export class BodySeatsServiceModule { }
