import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { ApiHelpOverviewModuleRoutingModule } from './overview-routing.module';

import { HelpComponent } from './help';

@NgModule({
  imports: [
    SharedModule,
    ApiHelpOverviewModuleRoutingModule
  ],
  declarations: [
    HelpComponent
  ],
  exports: [
    ApiHelpOverviewModuleRoutingModule
  ]
})
export class ApiHelpOverviewModule { }
