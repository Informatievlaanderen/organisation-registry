import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { LocationService } from './location.service';

@NgModule({
  declarations: [
  ],
  providers: [
    LocationService
  ],
  exports: [
  ]
})
export class LocationsServiceModule { }
