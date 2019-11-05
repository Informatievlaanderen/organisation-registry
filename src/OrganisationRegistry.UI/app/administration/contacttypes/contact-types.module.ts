import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { ContactTypesServiceModule } from 'services/contacttypes';
import { ContactTypesRoutingModule } from './contact-types-routing.module';

import { ContactTypeDetailComponent } from './detail';
import { ContactTypeOverviewComponent } from './overview';

import {
  ContactTypeListComponent,
  ContactTypeFilterComponent
} from './components/';

@NgModule({
  imports: [
    SharedModule,
    ContactTypesRoutingModule,
    ContactTypesServiceModule
  ],
  declarations: [
    ContactTypeDetailComponent,
    ContactTypeListComponent,
    ContactTypeOverviewComponent,
    ContactTypeFilterComponent
  ],
  exports: [
    ContactTypesRoutingModule
  ]
})
export class AdministrationContactTypesModule { }
