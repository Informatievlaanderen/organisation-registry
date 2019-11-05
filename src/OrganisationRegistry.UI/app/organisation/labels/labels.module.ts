import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { LabelTypesServiceModule } from 'services/labeltypes';
import { OrganisationLabelsServiceModule } from 'services/organisationlabels';

import { OrganisationLabelsRoutingModule } from './labels-routing.module';

import { OrganisationLabelsComponent } from './labels.component';
import { OrganisationLabelsOverviewComponent } from './overview';
import { OrganisationLabelsListComponent } from './list';
import { OrganisationLabelsFilterComponent } from './filter';
import { OrganisationLabelsCreateOrganisationLabelComponent } from './create';
import { OrganisationLabelsUpdateOrganisationLabelComponent } from './update';

@NgModule({
  imports: [
    SharedModule,
    OrganisationLabelsRoutingModule,
    OrganisationLabelsServiceModule,
    LabelTypesServiceModule
  ],
  declarations: [
    OrganisationLabelsComponent,
    OrganisationLabelsOverviewComponent,
    OrganisationLabelsListComponent,
    OrganisationLabelsFilterComponent,
    OrganisationLabelsCreateOrganisationLabelComponent,
    OrganisationLabelsUpdateOrganisationLabelComponent
  ],
  exports: [
    OrganisationLabelsRoutingModule
  ]
})
export class OrganisationLabelsModule { }
