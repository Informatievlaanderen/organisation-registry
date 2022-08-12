import {NgModule} from "@angular/core";
import {RouterModule, Routes} from "@angular/router";

import {FeatureGuard, Role, RoleGuard} from "core/auth";

import {OrganisationDetailComponent} from "./detail.component";

import {ContactTypeResolver} from "services/contacttypes";
import {LocationTypeResolver} from "services/locationtypes";

import {
  OrganisationCancelCouplingWithKboComponent,
  OrganisationCoupleWithKboComponent,
  OrganisationInfoAddChildOrganisationComponent,
  OrganisationInfoComponent,
  OrganisationInfoEditComponent,
  OrganisationInfoOverviewComponent,
  OrganisationTerminateComponent,
} from "organisation/info";

import {
  OrganisationBuildingsComponent,
  OrganisationBuildingsCreateOrganisationBuildingComponent,
  OrganisationBuildingsOverviewComponent,
  OrganisationBuildingsUpdateOrganisationBuildingComponent,
} from "organisation/buildings";

import {
  OrganisationCapacitiesComponent,
  OrganisationCapacitiesCreateOrganisationCapacityComponent,
  OrganisationCapacitiesOverviewComponent,
  OrganisationCapacitiesUpdateOrganisationCapacityComponent,
} from "organisation/capacities";

import {
  OrganisationOrganisationClassificationsComponent,
  OrganisationOrganisationClassificationsCreateOrganisationOrganisationClassificationComponent,
  OrganisationOrganisationClassificationsOverviewComponent,
  OrganisationOrganisationClassificationsUpdateOrganisationOrganisationClassificationComponent,
} from "organisation/classifications";

import {
  OrganisationContactsComponent,
  OrganisationContactsCreateOrganisationContactComponent,
  OrganisationContactsOverviewComponent,
  OrganisationContactsUpdateOrganisationContactComponent,
} from "organisation/contacts";

import {
  OrganisationFunctionsComponent,
  OrganisationFunctionsCreateOrganisationFunctionComponent,
  OrganisationFunctionsOverviewComponent,
  OrganisationFunctionsUpdateOrganisationFunctionComponent,
} from "organisation/functions";

import {
  OrganisationKeysComponent,
  OrganisationKeysCreateOrganisationKeyComponent,
  OrganisationKeysOverviewComponent,
  OrganisationKeysUpdateOrganisationKeyComponent,
} from "organisation/keys";

import {
  OrganisationRegulationsComponent,
  OrganisationRegulationsCreateOrganisationRegulationComponent,
  OrganisationRegulationsOverviewComponent,
  OrganisationRegulationsUpdateOrganisationRegulationComponent,
} from "organisation/regulations";

import {
  OrganisationLabelsComponent,
  OrganisationLabelsCreateOrganisationLabelComponent,
  OrganisationLabelsOverviewComponent,
  OrganisationLabelsUpdateOrganisationLabelComponent,
} from "organisation/labels";

import {
  OrganisationLocationsComponent,
  OrganisationLocationsCreateOrganisationLocationComponent,
  OrganisationLocationsOverviewComponent,
  OrganisationLocationsUpdateOrganisationLocationComponent,
} from "organisation/locations";

import {
  OrganisationParentsComponent,
  OrganisationParentsCreateOrganisationParentComponent,
  OrganisationParentsOverviewComponent,
  OrganisationParentsUpdateOrganisationParentComponent,
} from "organisation/parents";

import {
  OrganisationFormalFrameworksComponent,
  OrganisationFormalFrameworksCreateOrganisationFormalFrameworkComponent,
  OrganisationFormalFrameworksOverviewComponent,
  OrganisationFormalFrameworksUpdateOrganisationFormalFrameworkComponent,
} from "organisation/formalframeworks";

import {
  OrganisationBankAccountsComponent,
  OrganisationBankAccountsCreateOrganisationBankAccountComponent,
  OrganisationBankAccountsOverviewComponent,
  OrganisationBankAccountsUpdateOrganisationBankAccountComponent,
} from "organisation/bankaccounts";

import {OrganisationBodiesComponent, OrganisationBodiesOverviewComponent,} from "organisation/bodies";

import {
  OrganisationRelationsComponent,
  OrganisationRelationsCreateOrganisationRelationComponent,
  OrganisationRelationsOverviewComponent,
  OrganisationRelationsUpdateOrganisationRelationComponent,
} from "organisation/relations";

import {
  OrganisationOpeningHoursComponent,
  OrganisationOpeningHoursCreateOrganisationOpeningHourComponent,
  OrganisationOpeningHoursOverviewComponent,
  OrganisationOpeningHoursUpdateOrganisationOpeningHourComponent,
} from "organisation/openinghours";

import {OrganisationManagementOverviewComponent} from "organisation/management";

import {OrganisationVlimpersOverviewComponent} from "organisation/vlimpers";
import {OrganisationGuard} from "../guards/organisation.guard";
import {CanAddAndUpdateFormalFrameworksGuard} from "../guards/can-add-update-formal-frameworks.guard";
import {
  CanAddAndUpdateOrganisationClassificationTypeGuard
} from "../guards/can-add-update-organisation-classification-type.guard";
import {CanAddAndUpdateCapacityGuard} from "../guards/can-add-update-capacity.guard";
import {CanAddAndUpdateRegulationGuard} from "../guards/can-add-update-regulation.guard";

const routes: Routes = [
  {
    path: "organisations/:id",
    component: OrganisationDetailComponent,
    children: [
      { path: "", pathMatch: "prefix", redirectTo: "info" },
      {
        path: "info",
        component: OrganisationInfoComponent,
        children: [
          {
            path: "",
            component: OrganisationInfoOverviewComponent,
            data: { title: "Organisatie - Algemeen" },
          },
          {
            path: "edit",
            component: OrganisationInfoEditComponent,
            canActivate: [RoleGuard, OrganisationGuard],
            data: {
              title: "Organisatie - Algemeen - Bewerken organisatie",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
                Role.VlimpersBeheerder,
              ],
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
          {
            path: "couplewithkbo",
            component: OrganisationCoupleWithKboComponent,
            canActivate: [RoleGuard],
            data: {
              title: "Organisatie - Algemeen - Koppelen KBO",
              roles: [Role.AlgemeenBeheerder, Role.CjmBeheerder],
            },
          },
          {
            path: "cancelcouplingwithkbo",
            component: OrganisationCancelCouplingWithKboComponent,
            canActivate: [RoleGuard],
            data: {
              title: "Organisatie - Algemeen - Koppelen KBO ongedaan maken",
              roles: [Role.AlgemeenBeheerder, Role.CjmBeheerder],
            },
          },
          {
            path: "terminate",
            component: OrganisationTerminateComponent,
            canActivate: [RoleGuard],
            data: {
              title: "Organisatie - Stopzetten",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.VlimpersBeheerder,
              ],
            },
          },
          {
            path: "addchild",
            component: OrganisationInfoAddChildOrganisationComponent,
            canActivate: [RoleGuard, OrganisationGuard],
            data: {
              title: "Organisatie - Algemeen - Voeg dochterorganisatie toe",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
                Role.VlimpersBeheerder,
              ],
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
        ],
      },
      {
        path: "buildings",
        component: OrganisationBuildingsComponent,
        children: [
          {
            path: "",
            component: OrganisationBuildingsOverviewComponent,
            data: { title: "Organisatie - Gebouwen" },
          },
          {
            path: "create",
            component: OrganisationBuildingsCreateOrganisationBuildingComponent,
            canActivate: [RoleGuard, OrganisationGuard],
            data: {
              title: "Organisatie - Gebouwen - Gebouw koppelen",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
              ],
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
          {
            path: "edit/:id",
            component: OrganisationBuildingsUpdateOrganisationBuildingComponent,
            canActivate: [RoleGuard, OrganisationGuard],
            data: {
              title: "Organisatie - Gebouwen - Bewerken gebouw",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
              ],
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
        ],
      },
      {
        path: "capacities",
        component: OrganisationCapacitiesComponent,
        children: [
          {
            path: "",
            component: OrganisationCapacitiesOverviewComponent,
            data: { title: "Organisatie - Hoedanigheden" },
          },
          {
            path: "create",
            component:
              OrganisationCapacitiesCreateOrganisationCapacityComponent,
            canActivate: [CanAddAndUpdateCapacityGuard],
            resolve: {
              contactTypes: ContactTypeResolver,
            },
            data: {
              title: "Organisatie - Hoedanigheden - Nieuwe hoedanigheid",
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
          {
            path: "edit/:id",
            component:
              OrganisationCapacitiesUpdateOrganisationCapacityComponent,
            canActivate: [CanAddAndUpdateCapacityGuard],
            resolve: {
              contactTypes: ContactTypeResolver,
            },
            data: {
              title: "Organisatie - Hoedanigheden - Bewerken hoedanigheid",
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
        ],
      },
      {
        path: "contacts",
        component: OrganisationContactsComponent,
        children: [
          {
            path: "",
            component: OrganisationContactsOverviewComponent,
            data: { title: "Organisatie - Contacten" },
          },
          {
            path: "create",
            component: OrganisationContactsCreateOrganisationContactComponent,
            canActivate: [RoleGuard, OrganisationGuard],
            data: {
              title: "Organisatie - Contacten - Nieuw contact",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
              ],
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
          {
            path: "edit/:id",
            component: OrganisationContactsUpdateOrganisationContactComponent,
            canActivate: [RoleGuard, OrganisationGuard],
            data: {
              title: "Organisatie - Contacten - Bewerken contact",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
              ],
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
        ],
      },
      {
        path: "functions",
        component: OrganisationFunctionsComponent,
        children: [
          {
            path: "",
            component: OrganisationFunctionsOverviewComponent,
            data: { title: "Organisatie - Functies" },
          },
          {
            path: "create",
            component: OrganisationFunctionsCreateOrganisationFunctionComponent,
            canActivate: [RoleGuard, OrganisationGuard],
            resolve: {
              contactTypes: ContactTypeResolver,
            },
            data: {
              title: "Organisatie - Functies - Nieuwe functie",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
              ],
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
          {
            path: "edit/:id",
            component: OrganisationFunctionsUpdateOrganisationFunctionComponent,
            canActivate: [RoleGuard, OrganisationGuard],
            resolve: {
              contactTypes: ContactTypeResolver,
            },
            data: {
              title: "Organisatie - Functies - Bewerken functie",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
              ],
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
        ],
      },
      {
        path: "classifications",
        component: OrganisationOrganisationClassificationsComponent,
        children: [
          {
            path: "",
            component: OrganisationOrganisationClassificationsOverviewComponent,
            data: { title: "Organisatie - Classificaties" },
          },
          {
            path: "create",
            component:
              OrganisationOrganisationClassificationsCreateOrganisationOrganisationClassificationComponent,
            canActivate: [CanAddAndUpdateOrganisationClassificationTypeGuard],
            data: {
              title: "Organisatie - Classificaties - Nieuwe classificatie",
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
          {
            path: "edit/:id",
            component:
              OrganisationOrganisationClassificationsUpdateOrganisationOrganisationClassificationComponent,
            canActivate: [CanAddAndUpdateOrganisationClassificationTypeGuard],
            data: {
              title: "Organisatie - Classificaties - Bewerken classificatie",
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
        ],
      },
      {
        path: "keys",
        component: OrganisationKeysComponent,
        children: [
          {
            path: "",
            component: OrganisationKeysOverviewComponent,
            data: { title: "Organisatie - Sleutels" },
          },
          {
            path: "create",
            component: OrganisationKeysCreateOrganisationKeyComponent,
            canActivate: [RoleGuard, OrganisationGuard],
            data: {
              title: "Organisatie - Sleutels - Nieuwe sleutel",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
                Role.VlimpersBeheerder,
              ],
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
          {
            path: "edit/:id",
            component: OrganisationKeysUpdateOrganisationKeyComponent,
            canActivate: [RoleGuard, OrganisationGuard],
            data: {
              title: "Organisatie - Sleutels - Bewerken sleutel",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
                Role.VlimpersBeheerder,
              ],
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
        ],
      },
      {
        path: "regulations",
        component: OrganisationRegulationsComponent,
        canActivate: [FeatureGuard],
        data: {
          features: ["regulationsManagement"],
        },
        children: [
          {
            path: "",
            component: OrganisationRegulationsOverviewComponent,
            data: { title: "Organisatie - Regelgeving" },
          },
          {
            path: "create",
            component:
              OrganisationRegulationsCreateOrganisationRegulationComponent,
            canActivate: [CanAddAndUpdateRegulationGuard],
            data: {
              title: "Organisatie - Regelgeving - Nieuwe regelgeving",
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
          {
            path: "edit/:id",
            component:
              OrganisationRegulationsUpdateOrganisationRegulationComponent,
            canActivate: [CanAddAndUpdateRegulationGuard],
            data: {
              title: "Organisatie - Regelgeving - Bewerken regelgeving",
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
        ],
      },
      {
        path: "labels",
        component: OrganisationLabelsComponent,
        children: [
          {
            path: "",
            component: OrganisationLabelsOverviewComponent,
            data: { title: "Organisatie - Benamingen" },
          },
          {
            path: "create",
            component: OrganisationLabelsCreateOrganisationLabelComponent,
            canActivate: [RoleGuard, OrganisationGuard],
            data: {
              title: "Organisatie - Benamingen - Nieuwe benaming",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
                Role.VlimpersBeheerder,
              ],
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
          {
            path: "edit/:id",
            component: OrganisationLabelsUpdateOrganisationLabelComponent,
            canActivate: [RoleGuard, OrganisationGuard],
            data: {
              title: "Organisatie - Benamingen - Bewerken benaming",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
                Role.VlimpersBeheerder,
              ],
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
        ],
      },
      {
        path: "locations",
        component: OrganisationLocationsComponent,
        children: [
          {
            path: "",
            component: OrganisationLocationsOverviewComponent,
            data: { title: "Organisatie - Locaties" },
          },
          {
            path: "create",
            component: OrganisationLocationsCreateOrganisationLocationComponent,
            canActivate: [RoleGuard, OrganisationGuard],
            resolve: {
              locationTypes: LocationTypeResolver,
            },
            data: {
              title: "Organisatie - Locaties - Locatie koppelen",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
              ],
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
          {
            path: "edit/:id",
            component: OrganisationLocationsUpdateOrganisationLocationComponent,
            canActivate: [RoleGuard, OrganisationGuard],
            resolve: {
              locationTypes: LocationTypeResolver,
            },
            data: {
              title: "Organisatie - Locaties - Bewerken locatie",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
              ],
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
        ],
      },
      {
        path: "parents",
        component: OrganisationParentsComponent,
        children: [
          {
            path: "",
            component: OrganisationParentsOverviewComponent,
            data: { title: "Organisatie - Historiek" },
          },
          {
            path: "create",
            component: OrganisationParentsCreateOrganisationParentComponent,
            canActivate: [RoleGuard, OrganisationGuard],
            data: {
              title: "Organisatie - Historiek - Moeder entiteit koppelen",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
                Role.VlimpersBeheerder,
              ],
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
          {
            path: "edit/:id",
            component: OrganisationParentsUpdateOrganisationParentComponent,
            canActivate: [RoleGuard, OrganisationGuard],
            data: {
              title: "Organisatie - Historiek - Bewerken moeder entiteit",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
                Role.VlimpersBeheerder,
              ],
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
        ],
      },
      {
        path: "formalframeworks",
        component: OrganisationFormalFrameworksComponent,
        children: [
          {
            path: "",
            component: OrganisationFormalFrameworksOverviewComponent,
            data: { title: "Organisatie - Toepassingsgebieden" },
          },
          {
            path: "create",
            component:
              OrganisationFormalFrameworksCreateOrganisationFormalFrameworkComponent,
            canActivate: [CanAddAndUpdateFormalFrameworksGuard],
            data: {
              title:
                "Organisatie - Toepassingsgebieden - Toepassingsgebied koppelen",
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
          {
            path: "edit/:id",
            component:
              OrganisationFormalFrameworksUpdateOrganisationFormalFrameworkComponent,
            canActivate: [CanAddAndUpdateFormalFrameworksGuard],
            data: {
              title:
                "Organisatie - Toepassingsgebieden - Bewerken toepassingsgebied",
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
        ],
      },
      {
        path: "bankaccounts",
        component: OrganisationBankAccountsComponent,
        children: [
          {
            path: "",
            component: OrganisationBankAccountsOverviewComponent,
            data: { title: "Organisatie - Bankrekeningnummers" },
          },
          {
            path: "create",
            component:
              OrganisationBankAccountsCreateOrganisationBankAccountComponent,
            canActivate: [RoleGuard, OrganisationGuard],
            data: {
              title:
                "Organisatie - Bankrekeningnummers - Bankrekeningnummer koppelen",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
              ],
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
          {
            path: "edit/:id",
            component:
              OrganisationBankAccountsUpdateOrganisationBankAccountComponent,
            canActivate: [RoleGuard, OrganisationGuard],
            data: {
              title:
                "Organisatie - Bankrekeningnummers - Bewerken bankrekeningnummer",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
              ],
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
        ],
      },
      {
        path: "bodies",
        component: OrganisationBodiesComponent,
        children: [
          {
            path: "",
            component: OrganisationBodiesOverviewComponent,
            data: { title: "Organisatie - Organen" },
          },
        ],
      },
      {
        path: "relations",
        component: OrganisationRelationsComponent,
        children: [
          {
            path: "",
            component: OrganisationRelationsOverviewComponent,
            data: { title: "Organisatie - Relaties" },
          },
          {
            path: "create",
            component: OrganisationRelationsCreateOrganisationRelationComponent,
            canActivate: [RoleGuard, OrganisationGuard],
            resolve: {
              contactTypes: ContactTypeResolver,
            },
            data: {
              title: "Organisatie - Relaties - Nieuwe relatie",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
              ],
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
          {
            path: "edit/:id",
            component: OrganisationRelationsUpdateOrganisationRelationComponent,
            canActivate: [RoleGuard, OrganisationGuard],
            resolve: {
              contactTypes: ContactTypeResolver,
            },
            data: {
              title: "Organisatie - Relaties - Bewerken relatie",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
              ],
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
        ],
      },
      {
        path: "openinghours",
        component: OrganisationOpeningHoursComponent,
        children: [
          {
            path: "",
            component: OrganisationOpeningHoursOverviewComponent,
            data: { title: "Organisatie - Openingsuren" },
          },
          {
            path: "create",
            component:
              OrganisationOpeningHoursCreateOrganisationOpeningHourComponent,
            canActivate: [RoleGuard, OrganisationGuard],
            data: {
              title: "Organisatie - Openingsuren - Nieuw openingsuur",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
              ],
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
          {
            path: "edit/:id",
            component:
              OrganisationOpeningHoursUpdateOrganisationOpeningHourComponent,
            canActivate: [RoleGuard, OrganisationGuard],
            data: {
              title: "Organisatie - Openingsuren - Bewerken openingsuur",
              roles: [
                Role.AlgemeenBeheerder,
                Role.CjmBeheerder,
                Role.DecentraalBeheerder,
              ],
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
        ],
      },
      {
        path: "manage",
        children: [
          {
            path: "",
            component: OrganisationManagementOverviewComponent,
            canActivate: [RoleGuard, OrganisationGuard],
            data: {
              title: "Organisatie - KBO-koppeling",
              roles: [Role.AlgemeenBeheerder, Role.CjmBeheerder],
              organisationGuard: {
                params: "route.parent.parent.params",
                idPart: "id",
              },
            },
          },
        ],
      },
      {
        path: "vlimpers",
        children: [
          {
            path: "",
            component: OrganisationVlimpersOverviewComponent,
            canActivate: [RoleGuard, OrganisationGuard],
            data: {
              title: "Organisatie - Vlimpers",
              roles: [Role.AlgemeenBeheerder, Role.CjmBeheerder],
              organisationGuard: {
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
export class OrganisationDetailRoutingModule {}
