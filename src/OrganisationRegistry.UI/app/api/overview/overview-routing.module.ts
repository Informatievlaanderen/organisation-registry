import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { HelpComponent } from './help';

const routes: Routes = [
  {
    path: 'api',
    component: HelpComponent,
    data: {
      title: 'Integratie (API)',
    }
  }
];

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class ApiHelpOverviewModuleRoutingModule { }
