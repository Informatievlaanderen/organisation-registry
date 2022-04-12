import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { RoleGuard, Role } from 'core/auth';

import { FormalFrameworkCategoryDetailComponent } from './detail';
import { FormalFrameworkCategoryOverviewComponent } from './overview';

const routes: Routes = [
  {
    path: 'administration/formalframeworkcategories',
    component: FormalFrameworkCategoryOverviewComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder],
      title: 'Parameters - Toepassingsgebiedcategorieën'
    }
  },
  {
    path: 'administration/formalframeworkcategories/create',
    component: FormalFrameworkCategoryDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder],
      title: 'Parameters - Nieuwe toepassingsgebiedcategorie'
    }
  },
  {
    path: 'administration/formalframeworkcategories/:id',
    component: FormalFrameworkCategoryDetailComponent,
    canActivate: [RoleGuard],
    data: {
      roles: [Role.AlgemeenBeheerder],
      title: 'Parameters - Bewerken toepassingsgebiedcategorie'
    }
  },
];

@NgModule({
  imports: [
    RouterModule.forChild(routes)
  ],
  exports: [
    RouterModule
  ]
})
export class FormalFrameworkCategoriesRoutingModule { }
