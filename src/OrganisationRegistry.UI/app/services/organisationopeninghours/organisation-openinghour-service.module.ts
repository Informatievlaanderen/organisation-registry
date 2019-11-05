import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { OrganisationOpeningHourService } from './organisation-openinghour.service';

@NgModule({
  declarations: [
  ],
  providers: [
    OrganisationOpeningHourService
  ],
  exports: [
  ]
})
export class OrganisationOpeningHoursServiceModule { }
