import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { PersonFunctionService } from './person-function.service';

@NgModule({
  declarations: [
  ],
  providers: [
    PersonFunctionService
  ],
  exports: [
  ]
})
export class PeopleFunctionsServiceModule { }
