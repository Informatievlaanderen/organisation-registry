import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { SeatTypeService } from './seat-type.service';
import { SeatTypeResolver } from './seat-type.resolver';

@NgModule({
  declarations: [
  ],
  providers: [
    SeatTypeService,
    SeatTypeResolver
  ],
  exports: [
  ]
})
export class SeatTypesServiceModule { }
