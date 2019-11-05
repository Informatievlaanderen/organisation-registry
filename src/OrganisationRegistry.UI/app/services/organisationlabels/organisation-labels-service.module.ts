import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { OrganisationLabelService } from './organisation-label.service';

@NgModule({
  declarations: [
  ],
  providers: [
    OrganisationLabelService
  ],
  exports: [
  ]
})
export class OrganisationLabelsServiceModule { }
