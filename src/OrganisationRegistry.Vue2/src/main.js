import Vue from "vue";
import App from "./App.vue";
import "./registerServiceWorker";
import router from "./router";
import "./components";

import { createPinia, PiniaVuePlugin } from "pinia";
import { markRaw } from "@vue/composition-api/dist/vue-composition-api";
import { useUserStore } from "@/stores/user";

(async () => {
  Vue.use(PiniaVuePlugin);
  const pinia = createPinia();

  function RouterPlugin() {
    return { router: markRaw(router) };
  }

  pinia.use(RouterPlugin);
  Vue.config.productionTip = false;

  window.organisatieRegisterApiEndpoint =
    window.organisatieRegisterApiEndpoint ||
    "https://api.organisatie.dev-vlaanderen.local:9003";

  const app = new Vue({
    router,
    render: (h) => h(App),
    pinia,
  });

  const userStore = useUserStore();
  await userStore.initializeOidcClient();

  app.$mount("#app");
})();
