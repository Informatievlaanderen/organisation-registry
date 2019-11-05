import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { OrganisationLocationService } from './organisation-location.service';

@NgModule({
  declarations: [
  ],
  providers: [
    OrganisationLocationService
  ],
  exports: [
  ]
})
export class OrganisationLocationsServiceModule { }
