import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { NoContent } from './no-content';

const routes: Routes = [
  { path: '', redirectTo: '/organisations', pathMatch: 'full' },
  { path: '404', component: NoContent },
  { path: '**', redirectTo: '404', pathMatch: 'full' },
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes, { useHash: true })
  ],
  exports: [
    RouterModule
  ]
})

export class AppRoutingModule { }
