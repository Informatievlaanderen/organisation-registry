import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { BodyOrganisationService } from './body-organisation.service';

@NgModule({
  declarations: [
  ],
  providers: [
    BodyOrganisationService
  ],
  exports: [
  ]
})
export class BodyOrganisationsServiceModule { }
