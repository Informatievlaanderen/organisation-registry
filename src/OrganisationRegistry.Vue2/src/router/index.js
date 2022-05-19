import Vue from "vue";
import VueRouter from "vue-router";
import Callback from "@/views/Callback/AuthCallback";
import UnauthorizedView from "@/views/Unauthorized/UnauthorizedView";

Vue.use(VueRouter);

const routes = [
  {
    path: "/",
    name: "upload-organisations",
    meta: { requiresAuth: true },
    component: () =>
      import(
        /* webpackChunkName: "ImportOrganisations" */ "../views/ImportOrganisations/ImportOrganisationsView.vue"
      ),
  },
  {
    path: "/oic",
    name: "openidconnect",
    component: Callback,
    meta: { requiresAuth: false },
    props: (route) => ({ code: route.query.code }),
  },
  {
    path: "/unauthorized",
    name: "unauthorized",
    meta: { requiresAuth: false },
    component: UnauthorizedView,
  },
];

const router = new VueRouter({
  mode: "history",
  base: process.env.BASE_URL,
  routes,
});

export default router;
