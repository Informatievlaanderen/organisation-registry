import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { OrganisationParentService } from './organisation-parent.service';

@NgModule({
  declarations: [
  ],
  providers: [
    OrganisationParentService
  ],
  exports: [
  ]
})
export class OrganisationParentsServiceModule { }
