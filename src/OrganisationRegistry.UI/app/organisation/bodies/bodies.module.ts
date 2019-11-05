import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { OrganisationBodiesServiceModule } from 'services/organisationbodies';

import { OrganisationBodiesRoutingModule } from './bodies-routing.module';

import { OrganisationBodiesComponent } from './bodies.component';
import { OrganisationBodiesOverviewComponent } from './overview';
import { OrganisationBodiesListComponent } from './list';
import { OrganisationBodiesFilterComponent } from './filter';

@NgModule({
  imports: [
    SharedModule,
    OrganisationBodiesRoutingModule,
    OrganisationBodiesServiceModule
  ],
  declarations: [
    OrganisationBodiesComponent,
    OrganisationBodiesOverviewComponent,
    OrganisationBodiesListComponent,
    OrganisationBodiesFilterComponent
  ],
  exports: [
    OrganisationBodiesRoutingModule
  ]
})
export class OrganisationBodiesModule { }
