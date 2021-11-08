import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { RegulationSubThemesServiceModule } from 'services/regulation-sub-themes';
import { RegulationThemesServiceModule } from 'services/regulation-themes';
import { OrganisationRegulationsServiceModule } from 'services/organisationregulations';

import { OrganisationRegulationsRoutingModule } from './regulations-routing.module';

import { OrganisationRegulationsComponent } from './regulations.component';
import { OrganisationRegulationsOverviewComponent } from './overview';
import { OrganisationRegulationsListComponent } from './list';
import { OrganisationRegulationsFilterComponent } from './filter';
import { OrganisationRegulationsCreateOrganisationRegulationComponent } from './create';
import { OrganisationRegulationsUpdateOrganisationRegulationComponent } from './update';

@NgModule({
  imports: [
    SharedModule,
    OrganisationRegulationsRoutingModule,
    OrganisationRegulationsServiceModule,
    RegulationThemesServiceModule,
    RegulationSubThemesServiceModule
  ],
  declarations: [
    OrganisationRegulationsComponent,
    OrganisationRegulationsOverviewComponent,
    OrganisationRegulationsListComponent,
    OrganisationRegulationsFilterComponent,
    OrganisationRegulationsCreateOrganisationRegulationComponent,
    OrganisationRegulationsUpdateOrganisationRegulationComponent
  ],
  exports: [
    OrganisationRegulationsRoutingModule
  ]
})
export class OrganisationRegulationsModule { }
