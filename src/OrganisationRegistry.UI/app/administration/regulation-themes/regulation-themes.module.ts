import { NgModule } from '@angular/core';
import { SharedModule } from 'shared';

import { RegulationThemesServiceModule } from 'services/regulation-themes';
import { RegulationThemesRoutingModule } from './regulation-themes-routing.module';

import { RegulationThemeDetailComponent } from './detail';
import { RegulationThemeOverviewComponent } from './overview';

import {
  RegulationThemeListComponent,
  RegulationThemeFilterComponent
} from './components/';

@NgModule({
  imports: [
    SharedModule,
    RegulationThemesRoutingModule,
    RegulationThemesServiceModule
  ],
  declarations: [
    RegulationThemeDetailComponent,
    RegulationThemeListComponent,
    RegulationThemeOverviewComponent,
    RegulationThemeFilterComponent
  ],
  exports: [
    RegulationThemesRoutingModule
  ]
})
export class AdministrationRegulationThemesModule { }
