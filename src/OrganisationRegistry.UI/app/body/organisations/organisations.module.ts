import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { BodiesServiceModule } from 'services/bodies';
import { BodyOrganisationsServiceModule } from 'services/bodyorganisations';

import { BodyOrganisationsRoutingModule } from './organisations-routing.module';

import { BodyOrganisationsComponent } from './organisations.component';
import { BodyOrganisationsOverviewComponent } from './overview';
import { BodyOrganisationsListComponent } from './list';
import { BodyOrganisationsFilterComponent } from './filter';
import { BodyOrganisationsCreateBodyOrganisationComponent } from './create';
import { BodyOrganisationsUpdateBodyOrganisationComponent } from './update';

@NgModule({
  imports: [
    SharedModule,
    BodyOrganisationsRoutingModule,
    BodyOrganisationsServiceModule,
    BodiesServiceModule
  ],
  declarations: [
    BodyOrganisationsComponent,
    BodyOrganisationsOverviewComponent,
    BodyOrganisationsListComponent,
    BodyOrganisationsFilterComponent,
    BodyOrganisationsCreateBodyOrganisationComponent,
    BodyOrganisationsUpdateBodyOrganisationComponent
  ],
  exports: [
    BodyOrganisationsRoutingModule
  ]
})
export class BodyOrganisationsModule { }
