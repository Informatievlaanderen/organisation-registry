import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { OrganisationRelationService } from './organisation-relation.service';

@NgModule({
  declarations: [
  ],
  providers: [
    OrganisationRelationService
  ],
  exports: [
  ]
})
export class OrganisationRelationsServiceModule { }
