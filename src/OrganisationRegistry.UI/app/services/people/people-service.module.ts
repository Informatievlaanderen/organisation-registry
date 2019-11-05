import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { PersonService } from './person.service';

@NgModule({
  declarations: [
  ],
  providers: [
    PersonService
  ],
  exports: [
  ]
})
export class PeopleServiceModule { }
