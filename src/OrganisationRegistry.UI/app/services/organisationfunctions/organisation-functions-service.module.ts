import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { OrganisationFunctionService } from './organisation-function.service';

@NgModule({
  declarations: [
  ],
  providers: [
    OrganisationFunctionService
  ],
  exports: [
  ]
})
export class OrganisationFunctionsServiceModule { }
