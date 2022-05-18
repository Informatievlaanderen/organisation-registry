import Vue from "vue";
import VueRouter from "vue-router";
import Callback from "@/views/Callback/AuthCallback";

Vue.use(VueRouter);

const routes = [
  {
    path: "/",
    name: "upload-organisations",
    component: () =>
      import(
        /* webpackChunkName: "ImportOrganisations" */ "../views/ImportOrganisations/ImportOrganisationsView.vue"
      ),
  },
  {
    path: "/oic",
    name: "openidconnect",
    component: Callback,
    props: (route) => ({ code: route.query.code }),
  },
];

const router = new VueRouter({
  mode: "history",
  base: process.env.BASE_URL,
  routes,
});

export default router;
