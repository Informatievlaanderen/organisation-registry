<template>
  <dv-functional-header-actions v-if="isLoggedIn">
    <dv-functional-header-action :title="userDescription" />
    <dv-functional-header-action title="Afmelden" :on="logoutClicked" />
  </dv-functional-header-actions>
  <dv-functional-header-actions v-else>
    <dv-functional-header-action title="Aanmelden" :on="loginClicked" />
  </dv-functional-header-actions>
</template>

<script>
import DvFunctionalHeaderAction from "../functional-header/FunctionalHeaderAction";
import DvFunctionalHeaderActions from "../functional-header/FunctionalHeaderActions";

import { mapStores } from "pinia";
import { useUserStore } from "@/stores/user";
export default {
  name: "user-info",
  inject: ["oidcClient"],
  components: {
    DvFunctionalHeaderAction,
    DvFunctionalHeaderActions,
  },
  computed: {
    ...mapStores(useUserStore),
    userDescription() {
      return `${this.userStore.firstName} ${this.userStore.name}`;
    },
    isLoggedIn() {
      return this.userStore.isLoggedIn;
    },
  },
  methods: {
    async loginClicked() {
      this.client.signIn();
    },
    async logoutClicked() {
      this.client.signOut();
    },
  },
  async mounted() {
    this.client = await this.oidcClient;
  },
  props: {
    modHasActions: {
      default: false,
      type: Boolean,
    },
  },
  data() {
    return {
      classes: {
        "functional-header": true,
        "functional-header--has-actions": this.modHasActions,
      },
    };
  },
};
</script>

<style scoped>
.functional-header {
  margin-bottom: 0;
}

@media screen and (max-width: 767px) {
  .functional-header--has-actions:before,
  .functional-header__actions {
    display: block;
  }
}
</style>
