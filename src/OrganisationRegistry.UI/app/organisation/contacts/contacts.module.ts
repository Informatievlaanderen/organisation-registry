import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { ContactTypesServiceModule } from 'services/contacttypes';
import { OrganisationContactsServiceModule } from 'services/organisationcontacts';

import { OrganisationContactsRoutingModule } from './contacts-routing.module';

import { OrganisationContactsComponent } from './contacts.component';
import { OrganisationContactsOverviewComponent } from './overview';
import { OrganisationContactsListComponent } from './list';
import { OrganisationContactsFilterComponent } from './filter';
import { OrganisationContactsCreateOrganisationContactComponent } from './create';
import { OrganisationContactsUpdateOrganisationContactComponent } from './update';

@NgModule({
  imports: [
    SharedModule,
    OrganisationContactsRoutingModule,
    OrganisationContactsServiceModule,
    ContactTypesServiceModule
  ],
  declarations: [
    OrganisationContactsComponent,
    OrganisationContactsOverviewComponent,
    OrganisationContactsListComponent,
    OrganisationContactsFilterComponent,
    OrganisationContactsCreateOrganisationContactComponent,
    OrganisationContactsUpdateOrganisationContactComponent
  ],
  exports: [
    OrganisationContactsRoutingModule
  ]
})
export class OrganisationContactsModule { }
