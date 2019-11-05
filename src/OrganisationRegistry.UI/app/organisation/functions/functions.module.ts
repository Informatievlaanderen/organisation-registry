import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { FunctionsServiceModule } from 'services/functions';
import { OrganisationFunctionsServiceModule } from 'services/organisationfunctions';

import { OrganisationFunctionsRoutingModule } from './functions-routing.module';

import { OrganisationFunctionsComponent } from './functions.component';
import { OrganisationFunctionsOverviewComponent } from './overview';
import { OrganisationFunctionsListComponent } from './list';
import { OrganisationFunctionsFilterComponent } from './filter';
import { OrganisationFunctionsCreateOrganisationFunctionComponent } from './create';
import { OrganisationFunctionsUpdateOrganisationFunctionComponent } from './update';

@NgModule({
  imports: [
    SharedModule,
    OrganisationFunctionsRoutingModule,
    OrganisationFunctionsServiceModule,
    FunctionsServiceModule
  ],
  declarations: [
    OrganisationFunctionsComponent,
    OrganisationFunctionsOverviewComponent,
    OrganisationFunctionsListComponent,
    OrganisationFunctionsFilterComponent,
    OrganisationFunctionsCreateOrganisationFunctionComponent,
    OrganisationFunctionsUpdateOrganisationFunctionComponent
  ],
  exports: [
    OrganisationFunctionsRoutingModule
  ]
})
export class OrganisationFunctionsModule { }
