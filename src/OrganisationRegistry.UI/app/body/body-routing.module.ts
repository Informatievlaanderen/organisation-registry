import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";

import { RoleGuard, Role, RolesResolver } from "core/auth";

import { BodyOverviewComponent } from "./overview";
import { CreateBodyComponent } from "./create";

const routes: Routes = [
  {
    path: "bodies",
    component: BodyOverviewComponent,
    data: {
      title: "Organen",
    },
  },
  {
    path: "bodies/create",
    canActivate: [RoleGuard],
    canActivateChild: [RoleGuard],
    resolve: {
      userRoles: RolesResolver,
    },
    data: {
      roles: [
        Role.AlgemeenBeheerder,
        Role.CjmBeheerder,
        Role.DecentraalBeheerder,
        Role.OrgaanBeheerder,
      ],
      title: "Nieuw orgaan",
    },
    component: CreateBodyComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class BodyRoutingModule {}
