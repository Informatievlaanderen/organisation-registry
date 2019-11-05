import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { OrganisationBankAccountService } from './organisation-bank-account.service';

@NgModule({
  declarations: [
  ],
  providers: [
    OrganisationBankAccountService
  ],
  exports: [
  ]
})
export class OrganisationBankAccountsServiceModule { }
