import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { ContactTypesServiceModule } from 'services/contacttypes';
import { BodyContactsServiceModule } from 'services/bodycontacts';

import { BodyContactsRoutingModule } from './contacts-routing.module';

import { BodyContactsComponent } from './contacts.component';
import { BodyContactsOverviewComponent } from './overview';
import { BodyContactsListComponent } from './list';
import { BodyContactsFilterComponent } from './filter';
import { BodyContactsCreateBodyContactComponent } from './create';
import { BodyContactsUpdateBodyContactComponent } from './update';

@NgModule({
  imports: [
    SharedModule,
    BodyContactsRoutingModule,
    BodyContactsServiceModule,
    ContactTypesServiceModule
  ],
  declarations: [
    BodyContactsComponent,
    BodyContactsOverviewComponent,
    BodyContactsListComponent,
    BodyContactsFilterComponent,
    BodyContactsCreateBodyContactComponent,
    BodyContactsUpdateBodyContactComponent
  ],
  exports: [
    BodyContactsRoutingModule
  ]
})
export class BodyContactsModule { }
