import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { RegulationSubThemesServiceModule } from 'services/regulation-sub-themes';
import { RegulationSubThemesRoutingModule } from './regulation-sub-themes-routing.module';

import { RegulationSubThemeDetailComponent } from './detail';
import { RegulationSubThemeOverviewComponent } from './overview';

import {
  RegulationSubThemeListComponent,
  RegulationSubThemeFilterComponent
} from './components/';

@NgModule({
  imports: [
    SharedModule,
    RegulationSubThemesRoutingModule,
    RegulationSubThemesServiceModule
  ],
  declarations: [
    RegulationSubThemeDetailComponent,
    RegulationSubThemeListComponent,
    RegulationSubThemeOverviewComponent,
    RegulationSubThemeFilterComponent
  ],
  exports: [
    RegulationSubThemesRoutingModule
  ]
})
export class AdministrationRegulationSubThemesModule { }
