import Vue from "vue";
import App from "./App.vue";
import "./registerServiceWorker";
import router from "./router";
import "./components";

import { createPinia, PiniaVuePlugin } from "pinia";
import { markRaw } from "@vue/composition-api/dist/vue-composition-api";
import { useUserStore } from "@/stores/user";

import moment from "moment";

(async () => {
  Vue.prototype.moment = moment;

  Vue.use(PiniaVuePlugin);
  const pinia = createPinia();

  function RouterPlugin() {
    return { router: markRaw(router) };
  }

  pinia.use(RouterPlugin);
  Vue.config.productionTip = false;

  window.organisationRegistryApiEndpoint =
    window.organisationRegistryApiEndpoint ||
    "https://api.organisatie.dev-vlaanderen.local:9003";

  Vue.use(pinia);

  const userStore = useUserStore();

  const app = new Vue({
    router,
    render: (h) => h(App),
    pinia,
  });

  await userStore.initializeOidcClient();
  router.beforeEach(async (to, from, next) => {
    if (!userStore.isLoggedIn && to.meta.requiresAuth) {
      console.log("unauthorized", userStore.isLoggedIn, to.meta.requiresAuth);
      next({ name: "unauthorized" });
    }
    next();
  });

  app.$mount("#app");
})();
