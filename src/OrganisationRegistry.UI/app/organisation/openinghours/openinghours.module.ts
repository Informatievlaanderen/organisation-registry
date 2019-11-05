import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { OrganisationOpeningHoursServiceModule } from 'services/organisationopeninghours';
import { ManagementServiceModule } from 'services/management';

import { OrganisationOpeningHoursRoutingModule } from './openinghours-routing.module';

import { OrganisationOpeningHoursComponent } from './openinghours.component';
import { OrganisationOpeningHoursOverviewComponent } from './overview';
import { OrganisationOpeningHoursListComponent } from './list';
import { OrganisationOpeningHoursFilterComponent } from './filter';
import { OrganisationOpeningHoursCreateOrganisationOpeningHourComponent } from './create';
import { OrganisationOpeningHoursUpdateOrganisationOpeningHourComponent } from './update';

@NgModule({
  imports: [
    SharedModule,
    OrganisationOpeningHoursRoutingModule,
    OrganisationOpeningHoursServiceModule,
    ManagementServiceModule
  ],
  declarations: [
    OrganisationOpeningHoursComponent,
    OrganisationOpeningHoursOverviewComponent,
    OrganisationOpeningHoursListComponent,
    OrganisationOpeningHoursFilterComponent,
    OrganisationOpeningHoursCreateOrganisationOpeningHourComponent,
    OrganisationOpeningHoursUpdateOrganisationOpeningHourComponent
  ],
  exports: [
    OrganisationOpeningHoursRoutingModule
  ]
})
export class OrganisationOpeningHoursModule { }
