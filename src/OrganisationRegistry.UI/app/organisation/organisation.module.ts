import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { KboServiceModule } from 'services/kbo';
import { OrganisationsServiceModule } from 'services/organisations';
import { TogglesServiceModule, FeaturesServiceModule } from 'services';

import { OrganisationRoutingModule } from './organisation-routing.module';

import { OrganisationOverviewComponent, OrganisationListComponent, OrganisationFilterComponent } from './overview';
import { CreateOrganisationComponent, CreateOrganisationFormComponent } from './create';
import { OrganisationDetailModule } from './detail';
import { OrganisationGuard } from "./guards/organisation.guard";
import { CanAddAndUpdateFormalFrameworksGuard } from "./guards/can-add-update-formal-frameworks.guard";
import { CanAddAndUpdateOrganisationClassificationTypeGuard } from "./guards/can-add-update-organisation-classification-type.guard";
import { CanAddAndUpdateCapacityGuard } from "./guards/can-add-update-capacity.guard";
import { CanAddAndUpdateRegulationGuard } from "./guards/can-add-update-regulation.guard";

@NgModule({
  imports: [
    SharedModule,
    KboServiceModule,
    TogglesServiceModule,
    FeaturesServiceModule,
    OrganisationRoutingModule,
    OrganisationsServiceModule,
    OrganisationDetailModule
  ],
  declarations: [
    OrganisationOverviewComponent,
    OrganisationListComponent,
    OrganisationFilterComponent,
    CreateOrganisationComponent,
    CreateOrganisationFormComponent
  ],
  providers:[
    OrganisationGuard,
    CanAddAndUpdateFormalFrameworksGuard,
    CanAddAndUpdateOrganisationClassificationTypeGuard,
    CanAddAndUpdateCapacityGuard,
    CanAddAndUpdateRegulationGuard
  ],
  exports: [
    OrganisationRoutingModule
  ]
})
export class OrganisationModule { }
