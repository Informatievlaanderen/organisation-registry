import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { LocationTypeService } from './location-type.service';
import { LocationTypeResolver } from './location-type.resolver';

@NgModule({
  declarations: [
  ],
  providers: [
    LocationTypeService,
    LocationTypeResolver
  ],
  exports: [
  ]
})
export class LocationTypesServiceModule { }
