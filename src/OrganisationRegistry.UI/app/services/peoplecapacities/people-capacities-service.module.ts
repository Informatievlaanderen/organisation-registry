import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { PersonCapacityService } from './person-capacity.service';

@NgModule({
  declarations: [
  ],
  providers: [
    PersonCapacityService
  ],
  exports: [
  ]
})
export class PeopleCapacitiesServiceModule { }
