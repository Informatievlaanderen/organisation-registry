import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { BodySearchService } from './body/body-search.service';
import { OrganisationSearchService } from './organisation/organisation-search.service';
import { PersonSearchService } from './person/person-search.service';

@NgModule({
  declarations: [
  ],
  providers: [
    BodySearchService,
    OrganisationSearchService,
    PersonSearchService,
  ],
  exports: [
  ]
})
export class SearchServiceModule { }
