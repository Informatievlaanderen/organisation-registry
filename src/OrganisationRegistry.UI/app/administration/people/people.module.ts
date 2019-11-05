import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { PeopleServiceModule } from 'services/people';
import { PeopleRoutingModule } from './people-routing.module';

import { PersonDetailComponent } from './detail';
import { PersonOverviewComponent } from './overview';

import {
  PersonListComponent,
  PersonFilterComponent
} from './components/';

@NgModule({
  imports: [
    SharedModule,
    PeopleRoutingModule,
    PeopleServiceModule
  ],
  declarations: [
    PersonDetailComponent,
    PersonListComponent,
    PersonOverviewComponent,
    PersonFilterComponent
  ],
  exports: [
    PeopleRoutingModule
  ]
})
export class AdministrationPeopleModule { }
