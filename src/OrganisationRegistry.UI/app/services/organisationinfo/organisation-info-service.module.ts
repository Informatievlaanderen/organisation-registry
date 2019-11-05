import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { OrganisationInfoService } from './organisation-info.service';

@NgModule({
  declarations: [
  ],
  providers: [
    OrganisationInfoService
  ],
  exports: [
  ]
})
export class OrganisationInfoServiceModule { }
