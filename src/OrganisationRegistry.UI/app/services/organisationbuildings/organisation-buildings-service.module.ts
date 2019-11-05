import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { OrganisationBuildingService } from './organisation-building.service';

@NgModule({
  declarations: [
  ],
  providers: [
    OrganisationBuildingService
  ],
  exports: [
  ]
})
export class OrganisationBuildingsServiceModule { }
