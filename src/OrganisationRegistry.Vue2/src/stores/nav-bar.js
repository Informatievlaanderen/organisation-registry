import Roles from "./roles";

export default [
  {
    roles: [],
    title: "Organisaties",
    to: { path: "/../#/organisations" },
  },
  {
    roles: [],
    title: "Organen",
    to: { path: "/../#/bodies" },
  },
  {
    roles: [],
    title: "Personen",
    to: { path: "/../#/people" },
  },
  {
    roles: [
      Roles.DecentraalBeheerder,
      Roles.AlgemeenBeheerder,
      Roles.CjmBeheerder,
    ],
    title: "Delegaties",
    to: { path: "/../#/manage/delegations" },
  },
  {
    roles: [],
    title: "Rapportering",
    to: { path: "/../#/report" },
  },
  {
    roles: [Roles.AlgemeenBeheerder, Roles.CjmBeheerder],
    title: "Parameters",
    to: { path: "/../#/administration" },
  },
  {
    roles: [Roles.AlgemeenBeheerder, Roles.Developer, Roles.CjmBeheerder],
    title: "Systeem",
    to: { path: "/../#/system" },
  },
  {
    roles: [
      Roles.VlimpersBeheerder,
      Roles.AlgemeenBeheerder,
      Roles.Developer,
      Roles.CjmBeheerder,
    ],
    title: "Importeren",
    to: { name: "upload-organisations" },
  },
];
