import Vue from "vue";

// import the entire component library and specific directives
import VlUiVueComponents, {
  VlModalToggle,
} from "@govflanders/vl-ui-vue-components";

// configuration of the built-in validator
const validatorConfig = {
  inject: true,
  locale: "nl",
};

// install the component library with config
Vue.use(VlUiVueComponents, {
  validation: validatorConfig,
});

// mandatory if you're using the vl-modal component
Vue.directive("vl-modal-toggle", VlModalToggle);
