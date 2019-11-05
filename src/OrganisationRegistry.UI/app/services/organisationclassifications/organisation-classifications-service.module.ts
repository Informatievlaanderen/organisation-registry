import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { OrganisationClassificationService } from './organisation-classification.service';

@NgModule({
  declarations: [
  ],
  providers: [
    OrganisationClassificationService
  ],
  exports: [
  ]
})
export class OrganisationClassificationsServiceModule { }
