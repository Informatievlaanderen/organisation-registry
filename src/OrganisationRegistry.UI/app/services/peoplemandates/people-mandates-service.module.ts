import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { PersonMandateService } from './person-mandate.service';

@NgModule({
  declarations: [
  ],
  providers: [
    PersonMandateService
  ],
  exports: [
  ]
})
export class PeopleMandatesServiceModule { }
