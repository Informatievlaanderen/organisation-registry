import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";

import { BodyGuard, RoleGuard, Role, RolesResolver } from "core/auth";

import { BodyDetailComponent } from "./detail.component";

import { LifecyclePhaseTypeResolver } from "services/lifecyclephasetypes";
import { SeatTypeResolver } from "services/seattypes";
import { MandateRoleTypeResolver } from "services/mandateroletypes";
import { BodySeatResolver } from "services/bodyseats";
import { FunctionResolver } from "services/functions";
import { ContactTypeResolver } from "services/contacttypes";

import {
  BodyInfoComponent,
  BodyInfoOverviewComponent,
  BodyInfoValidityComponent,
  BodyInfoGeneralComponent,
} from "body/info";

import {
  BodyOrganisationsComponent,
  BodyOrganisationsOverviewComponent,
  BodyOrganisationsCreateBodyOrganisationComponent,
  BodyOrganisationsUpdateBodyOrganisationComponent,
} from "body/organisations";

import {
  BodyMandatesComponent,
  BodyMandatesOverviewComponent,
  BodyMandatesLinkPersonComponent,
  BodyMandatesLinkFunctionComponent,
  BodyMandatesLinkOrganisationComponent,
  BodyMandatesUpdatePersonComponent,
  BodyMandatesUpdateFunctionComponent,
  BodyMandatesUpdateOrganisationComponent,
} from "body/mandates";

import {
  BodyCompositionComponent,
  BodySeatsOverviewComponent,
  BodySeatsCreateBodySeatComponent,
  BodySeatsUpdateBodySeatComponent,
} from "body/composition";

import {
  BodyFormalFrameworksComponent,
  BodyFormalFrameworksOverviewComponent,
  BodyFormalFrameworksCreateBodyFormalFrameworkComponent,
  BodyFormalFrameworksUpdateBodyFormalFrameworkComponent,
} from "body/formalframeworks";

import {
  BodyLifecyclePhasesComponent,
  BodyLifecyclePhasesOverviewComponent,
  BodyLifecyclePhasesCreateBodyLifecyclePhaseComponent,
  BodyLifecyclePhasesUpdateBodyLifecyclePhaseComponent,
} from "body/lifecyclephases";

import { BodyResponsibilitiesComponent } from "body/responsibilities";

import {
  BodyParticipationComponent,
  BodyParticipationOverviewComponent,
  BodyParticipationManageComponent,
} from "body/participation";

import {
  BodyContactsComponent,
  BodyContactsOverviewComponent,
  BodyContactsCreateBodyContactComponent,
  BodyContactsUpdateBodyContactComponent,
} from "body/contacts";

import {
  BodyBodyClassificationsComponent,
  BodyBodyClassificationsOverviewComponent,
  BodyBodyClassificationsCreateBodyBodyClassificationComponent,
  BodyBodyClassificationsUpdateBodyBodyClassificationComponent,
} from "body/classifications";

const routes: Routes = [
  {
    path: "bodies/:id",
    component: BodyDetailComponent,
    children: [
      { path: "", pathMatch: "prefix", redirectTo: "info" },
      {
        path: "info",
        component: BodyInfoComponent,
        children: [
          {
            path: "",
            component: BodyInfoOverviewComponent,
            data: { title: "Orgaan - Algemeen" },
          },
          {
            path: "general",
            component: BodyInfoGeneralComponent,
            canActivate: [RoleGuard, BodyGuard],
            data: {
              title: "Orgaan - Algemeen - Bewerken Algemene Informatie",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
                Role.OrgaanBeheerder,
              ],
              bodyGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
          {
            path: "validity",
            component: BodyInfoValidityComponent,
            canActivate: [RoleGuard, BodyGuard],
            data: {
              title: "Orgaan - Algemeen - Bewerken Duurtijd",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
                Role.OrgaanBeheerder,
              ],
              bodyGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
        ],
      },
      {
        path: "organisations",
        component: BodyOrganisationsComponent,
        children: [
          {
            path: "",
            component: BodyOrganisationsOverviewComponent,
            data: { title: "Orgaan - Organisaties" },
          },
          {
            path: "create",
            component: BodyOrganisationsCreateBodyOrganisationComponent,
            canActivate: [RoleGuard, BodyGuard],
            resolve: {
              userRoles: RolesResolver,
            },
            data: {
              title: "Orgaan - Organisaties - Organisatie koppelen",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
                Role.OrgaanBeheerder,
              ],
              bodyGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
          {
            path: "edit/:id",
            component: BodyOrganisationsUpdateBodyOrganisationComponent,
            canActivate: [RoleGuard, BodyGuard],
            resolve: {
              userRoles: RolesResolver,
            },
            data: {
              title: "Orgaan - Organisaties - Bewerken organisatie",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
                Role.OrgaanBeheerder,
              ],
              bodyGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
        ],
      },
      {
        path: "composition",
        component: BodyCompositionComponent,
        children: [
          {
            path: "",
            component: BodySeatsOverviewComponent,
            data: { title: "Orgaan - Posten" },
          },
          {
            path: "create",
            component: BodySeatsCreateBodySeatComponent,
            canActivate: [RoleGuard, BodyGuard],
            resolve: {
              seatTypes: SeatTypeResolver,
            },
            data: {
              title: "Orgaan - Posten - Nieuwe post",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
                Role.OrgaanBeheerder,
              ],
              bodyGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
          {
            path: "edit/:id",
            component: BodySeatsUpdateBodySeatComponent,
            canActivate: [RoleGuard, BodyGuard],
            resolve: {
              seatTypes: SeatTypeResolver,
            },
            data: {
              title: "Orgaan - Posten - Bewerken post",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
                Role.OrgaanBeheerder,
              ],
              bodyGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
        ],
      },
      {
        path: "mandates",
        component: BodyMandatesComponent,
        children: [
          {
            path: "",
            component: BodyMandatesOverviewComponent,
            data: { title: "Orgaan - Mandaat" },
          },
          {
            path: "linkperson",
            component: BodyMandatesLinkPersonComponent,
            canActivate: [RoleGuard, BodyGuard],
            resolve: {
              bodySeats: BodySeatResolver,
              contactTypes: ContactTypeResolver,
            },
            data: {
              title: "Orgaan - Mandaat - Persoon toewijzen",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
                Role.OrgaanBeheerder,
              ],
              bodyGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
          {
            path: "updateperson/:id",
            component: BodyMandatesUpdatePersonComponent,
            canActivate: [RoleGuard, BodyGuard],
            resolve: {
              bodySeats: BodySeatResolver,
              contactTypes: ContactTypeResolver,
            },
            data: {
              title: "Orgaan - Mandaat - Bewerken mandaat (persoon)",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
                Role.OrgaanBeheerder,
              ],
              bodyGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
          {
            path: "linkfunction",
            component: BodyMandatesLinkFunctionComponent,
            canActivate: [RoleGuard, BodyGuard],
            resolve: {
              bodySeats: BodySeatResolver,
              functionTypes: FunctionResolver,
            },
            data: {
              title: "Orgaan - Mandaat - Functie toewijzen",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
                Role.OrgaanBeheerder,
              ],
              bodyGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
          {
            path: "updatefunction/:id",
            component: BodyMandatesUpdateFunctionComponent,
            canActivate: [RoleGuard, BodyGuard],
            resolve: {
              bodySeats: BodySeatResolver,
              functionTypes: FunctionResolver,
            },
            data: {
              title: "Orgaan - Mandaat - Bewerken mandaat (functie)",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
                Role.OrgaanBeheerder,
              ],
              bodyGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
          {
            path: "linkorganisation",
            component: BodyMandatesLinkOrganisationComponent,
            canActivate: [RoleGuard, BodyGuard],
            resolve: {
              bodySeats: BodySeatResolver,
            },
            data: {
              title: "Orgaan - Mandaat - Organisatie toewijzen",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
                Role.OrgaanBeheerder,
              ],
              bodyGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
          {
            path: "updateorganisation/:id",
            component: BodyMandatesUpdateOrganisationComponent,
            canActivate: [RoleGuard, BodyGuard],
            resolve: {
              bodySeats: BodySeatResolver,
            },
            data: {
              title: "Orgaan - Mandaat - Bewerken mandaat (organisatie)",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
                Role.OrgaanBeheerder,
              ],
              bodyGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
        ],
      },
      {
        path: "lifecycle",
        component: BodyLifecyclePhasesComponent,
        children: [
          {
            path: "",
            component: BodyLifecyclePhasesOverviewComponent,
            data: { title: "Orgaan - Levensloop" },
          },
          {
            path: "create",
            component: BodyLifecyclePhasesCreateBodyLifecyclePhaseComponent,
            canActivate: [RoleGuard, BodyGuard],
            resolve: {
              lifecyclePhaseTypes: LifecyclePhaseTypeResolver,
            },
            data: {
              title: "Orgaan - Levensloop - Levensloopfase koppelen",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
                Role.OrgaanBeheerder,
              ],
              bodyGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
          {
            path: "edit/:id",
            component: BodyLifecyclePhasesUpdateBodyLifecyclePhaseComponent,
            canActivate: [RoleGuard, BodyGuard],
            resolve: {
              lifecyclePhaseTypes: LifecyclePhaseTypeResolver,
            },
            data: {
              title: "Orgaan - Levensloop - Bewerken levensloopfase",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
                Role.OrgaanBeheerder,
              ],
              bodyGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
        ],
      },
      {
        path: "formalframeworks",
        component: BodyFormalFrameworksComponent,
        children: [
          {
            path: "",
            component: BodyFormalFrameworksOverviewComponent,
            resolve: {
              userRoles: RolesResolver,
            },
            data: { title: "Orgaan - Toepassingsgebieden" },
          },
          {
            path: "create",
            component: BodyFormalFrameworksCreateBodyFormalFrameworkComponent,
            canActivate: [RoleGuard, BodyGuard],
            data: {
              title:
                "Orgaan - Toepassingsgebieden - Toepassingsgebied koppelen",
              roles: [Role.AlgemeenBeheerder,
                Role.CjmBeheerder, Role.OrgaanBeheerder],
              bodyGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
          {
            path: "edit/:id",
            component: BodyFormalFrameworksUpdateBodyFormalFrameworkComponent,
            canActivate: [RoleGuard, BodyGuard],
            data: {
              title:
                "Orgaan - Toepassingsgebieden - Bewerken toepassingsgebied",
              roles: [Role.AlgemeenBeheerder,
                Role.CjmBeheerder, Role.OrgaanBeheerder],
              bodyGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
        ],
      },
      {
        path: "contacts",
        component: BodyContactsComponent,
        children: [
          {
            path: "",
            component: BodyContactsOverviewComponent,
            resolve: {
              userRoles: RolesResolver,
            },
            data: { title: "Orgaan - Contacten" },
          },
          {
            path: "create",
            component: BodyContactsCreateBodyContactComponent,
            canActivate: [RoleGuard, BodyGuard],
            data: {
              title: "Orgaan - Contacten - Nieuw contact",
              roles: [Role.AlgemeenBeheerder,
                Role.CjmBeheerder, Role.OrgaanBeheerder],
              bodyGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
          {
            path: "edit/:id",
            component: BodyContactsUpdateBodyContactComponent,
            canActivate: [RoleGuard, BodyGuard],
            data: {
              title: "Orgaan - Contacten - Bewerken contact",
              roles: [Role.AlgemeenBeheerder,
                Role.CjmBeheerder, Role.OrgaanBeheerder],
              bodyGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
        ],
      },
      {
        path: "responsibilities",
        component: BodyResponsibilitiesComponent,
        data: { title: "Orgaan - Bevoegdheden" },
      },
      {
        path: "participation",
        component: BodyParticipationComponent,
        children: [
          {
            path: "",
            component: BodyParticipationOverviewComponent,
            resolve: {
              userRoles: RolesResolver,
            },
            data: { title: "Orgaan - Participatie" },
          },
          {
            path: "manage",
            component: BodyParticipationManageComponent,
            canActivate: [RoleGuard, BodyGuard],
            data: {
              title: "Orgaan - Participatie - Beheer MEP",
              roles: [Role.AlgemeenBeheerder,
                Role.CjmBeheerder, Role.OrgaanBeheerder],
              bodyGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
        ],
      },
      {
        path: "classifications",
        component: BodyBodyClassificationsComponent,
        children: [
          {
            path: "",
            component: BodyBodyClassificationsOverviewComponent,
            resolve: {
              userRoles: RolesResolver,
            },
            data: { title: "Orgaan - Classificaties" },
          },
          {
            path: "create",
            component:
              BodyBodyClassificationsCreateBodyBodyClassificationComponent,
            canActivate: [RoleGuard, BodyGuard],
            data: {
              title: "Orgaan - Classificaties - Nieuwe classificatie",
              roles: [Role.AlgemeenBeheerder,
                Role.CjmBeheerder, Role.OrgaanBeheerder],
              bodyGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
          {
            path: "edit/:id",
            component:
              BodyBodyClassificationsUpdateBodyBodyClassificationComponent,
            canActivate: [RoleGuard, BodyGuard],
            data: {
              title: "Orgaan - Classificaties - Bewerken classificatie",
              roles: [Role.AlgemeenBeheerder,
                Role.CjmBeheerder, Role.OrgaanBeheerder],
              bodyGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
        ],
      },
    ],
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class BodyDetailRoutingModule {}
