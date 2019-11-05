import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { OrganisationsServiceModule } from 'services/organisations';
import { OrganisationFormalFrameworksServiceModule } from 'services/organisationformalframeworks';

import { OrganisationFormalFrameworksRoutingModule } from './formal-frameworks-routing.module';

import { OrganisationFormalFrameworksComponent } from './formal-frameworks.component';
import { OrganisationFormalFrameworksOverviewComponent } from './overview';
import { OrganisationFormalFrameworksListComponent } from './list';
import { OrganisationFormalFrameworksFilterComponent } from './filter';
import { OrganisationFormalFrameworksCreateOrganisationFormalFrameworkComponent } from './create';
import { OrganisationFormalFrameworksUpdateOrganisationFormalFrameworkComponent } from './update';

@NgModule({
  imports: [
    SharedModule,
    OrganisationFormalFrameworksRoutingModule,
    OrganisationFormalFrameworksServiceModule,
    OrganisationsServiceModule
  ],
  declarations: [
    OrganisationFormalFrameworksComponent,
    OrganisationFormalFrameworksOverviewComponent,
    OrganisationFormalFrameworksListComponent,
    OrganisationFormalFrameworksFilterComponent,
    OrganisationFormalFrameworksCreateOrganisationFormalFrameworkComponent,
    OrganisationFormalFrameworksUpdateOrganisationFormalFrameworkComponent
  ],
  exports: [
    OrganisationFormalFrameworksRoutingModule
  ]
})
export class OrganisationFormalFrameworksModule { }
