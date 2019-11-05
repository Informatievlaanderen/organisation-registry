import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { OrganisationCapacityService } from './organisation-capacity.service';

@NgModule({
  declarations: [
  ],
  providers: [
    OrganisationCapacityService
  ],
  exports: [
  ]
})
export class OrganisationCapacitiesServiceModule { }
