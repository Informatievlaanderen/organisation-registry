import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { OrganisationBankAccountsServiceModule } from 'services/organisationbankaccounts';

import { OrganisationBankAccountsRoutingModule } from './bank-accounts-routing.module';

import { OrganisationBankAccountsComponent } from './bank-accounts.component';
import { OrganisationBankAccountsOverviewComponent } from './overview';
import { OrganisationBankAccountsListComponent } from './list';
import { OrganisationBankAccountsFilterComponent } from './filter';
import { OrganisationBankAccountsCreateOrganisationBankAccountComponent } from './create';
import { OrganisationBankAccountsUpdateOrganisationBankAccountComponent } from './update';

@NgModule({
  imports: [
    SharedModule,
    OrganisationBankAccountsRoutingModule,
    OrganisationBankAccountsServiceModule
  ],
  declarations: [
    OrganisationBankAccountsComponent,
    OrganisationBankAccountsOverviewComponent,
    OrganisationBankAccountsListComponent,
    OrganisationBankAccountsFilterComponent,
    OrganisationBankAccountsCreateOrganisationBankAccountComponent,
    OrganisationBankAccountsUpdateOrganisationBankAccountComponent
  ],
  exports: [
    OrganisationBankAccountsRoutingModule
  ]
})
export class OrganisationBankAccountsModule { }
