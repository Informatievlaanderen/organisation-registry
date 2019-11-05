import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { OrganisationService } from './organisation.service';
import { UpdateOrganisationService } from './update-organisation.service';

@NgModule({
  declarations: [
  ],
  providers: [
    OrganisationService,
    UpdateOrganisationService
  ],
  exports: [
  ]
})
export class OrganisationsServiceModule { }
