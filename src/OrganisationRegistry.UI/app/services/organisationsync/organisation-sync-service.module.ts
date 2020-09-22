import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { OrganisationSyncService } from './organisation-sync.service';

@NgModule({
  declarations: [
  ],
  providers: [
    OrganisationSyncService
  ],
  exports: [
  ]
})
export class OrganisationSyncServiceModule { }
