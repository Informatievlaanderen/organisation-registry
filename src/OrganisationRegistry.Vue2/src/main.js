import Vue from "vue";
import App from "./App.vue";
import "./registerServiceWorker";
import router from "./router";
import "./components";

import { createPinia, PiniaVuePlugin } from "pinia";

Vue.use(PiniaVuePlugin);
const pinia = createPinia();

Vue.config.productionTip = false;

window.organisatieRegisterApiEndpoint =
  window.organisatieRegisterApiEndpoint ||
  "https://api.organisatie.dev-vlaanderen.local:9003";

new Vue({
  router,
  render: (h) => h(App),
  pinia,
}).$mount("#app");
