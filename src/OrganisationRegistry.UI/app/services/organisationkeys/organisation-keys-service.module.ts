import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { OrganisationKeyService } from './organisation-key.service';

@NgModule({
  declarations: [
  ],
  providers: [
    OrganisationKeyService
  ],
  exports: [
  ]
})
export class OrganisationKeysServiceModule { }
