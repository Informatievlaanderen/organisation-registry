import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { OrganisationContactService } from './organisation-contact.service';

@NgModule({
  declarations: [
  ],
  providers: [
    OrganisationContactService
  ],
  exports: [
  ]
})
export class OrganisationContactsServiceModule { }
