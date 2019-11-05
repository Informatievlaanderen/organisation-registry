import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { KeyTypesServiceModule } from 'services/keytypes';
import { OrganisationKeysServiceModule } from 'services/organisationkeys';

import { OrganisationKeysRoutingModule } from './keys-routing.module';

import { OrganisationKeysComponent } from './keys.component';
import { OrganisationKeysOverviewComponent } from './overview';
import { OrganisationKeysListComponent } from './list';
import { OrganisationKeysFilterComponent } from './filter';
import { OrganisationKeysCreateOrganisationKeyComponent } from './create';
import { OrganisationKeysUpdateOrganisationKeyComponent } from './update';

@NgModule({
  imports: [
    SharedModule,
    OrganisationKeysRoutingModule,
    OrganisationKeysServiceModule,
    KeyTypesServiceModule
  ],
  declarations: [
    OrganisationKeysComponent,
    OrganisationKeysOverviewComponent,
    OrganisationKeysListComponent,
    OrganisationKeysFilterComponent,
    OrganisationKeysCreateOrganisationKeyComponent,
    OrganisationKeysUpdateOrganisationKeyComponent
  ],
  exports: [
    OrganisationKeysRoutingModule
  ]
})
export class OrganisationKeysModule { }
