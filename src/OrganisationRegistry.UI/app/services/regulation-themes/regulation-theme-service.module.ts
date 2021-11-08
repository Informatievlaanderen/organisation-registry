import { NgModule } from '@angular/core';

import { RegulationThemeService } from './regulation-theme.service';
import { RegulationThemeResolver } from './regulation-theme-resolver.service';

@NgModule({
  declarations: [
  ],
  providers: [
    RegulationThemeService,
    RegulationThemeResolver
  ],
  exports: [
  ]
})
export class RegulationThemesServiceModule { }
