import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { PeopleServiceModule } from 'services/people';

import { PeopleRoutingModule } from './people-routing.module';

import { PersonOverviewComponent, PersonListComponent, PersonFilterComponent } from './overview';
import { PeopleDetailModule } from './detail';

@NgModule({
  imports: [
    SharedModule,
    PeopleRoutingModule,
    PeopleServiceModule,
    PeopleDetailModule
  ],
  declarations: [
    PersonOverviewComponent,
    PersonListComponent,
    PersonFilterComponent
  ],
  exports: [
    PeopleRoutingModule
  ]
})
export class PeopleModule { }
