import { NgModule } from '@angular/core';
import { SharedModule } from '../shared';

import { SearchServiceModule } from '../services/search';

import { SearchRoutingModule } from './search-routing.module';

import { BodySearchComponent, BodySearchListComponent } from './bodies';
import { OrganisationSearchComponent, OrganisationSearchListComponent } from './organisations';
import { PersonSearchComponent, PersonSearchListComponent } from './people';
import { BlankSearchComponent } from './blank';

@NgModule({
  imports: [
    SharedModule,
    SearchRoutingModule,
    SearchServiceModule
  ],
  declarations: [
    BlankSearchComponent,
    BodySearchComponent,
    BodySearchListComponent,
    OrganisationSearchComponent,
    OrganisationSearchListComponent,
    PersonSearchComponent,
    PersonSearchListComponent
  ],
  exports: [
    SearchRoutingModule
  ]
})
export class SearchModule { }
