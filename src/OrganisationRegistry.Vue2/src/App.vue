<template>
  <div>
    <dv-header />

    <dv-functional-header mod-has-actions>
      <dv-user-info />

      <dv-functional-header-content>
        <router-link
          slot="title"
          class="functional-header__title"
          to="/../#/organisations"
          >{{ title }}
        </router-link>
        >
      </dv-functional-header-content>
      <dv-functional-header-sub>
        <dv-grid>
          <dv-column
            type="nav"
            role="navigation"
            :cols="[
              { nom: 9, den: 12 },
              { nom: 8, den: 12, mod: 's' },
              { nom: 1, den: 1, mod: 's' },
            ]"
            data-tabs-responsive-label="Navigatie"
          >
            <dv-tabs mod-is-functional-header>
              <dv-tab
                v-for="tab in navigations"
                :key="tab.title"
                :title="tab.title"
                exact
                @click.native="refresh()"
                :to="tab.to"
              />
            </dv-tabs>
          </dv-column>
        </dv-grid>
      </dv-functional-header-sub>
    </dv-functional-header>

    <dv-main>
      <dv-region>
        <dv-layout mod-is-wide>
          <OrAlert
            :visible="alert.visible"
            :is-closable="true"
            :type="alert.type"
            :title="alert.title"
            @close="clearAlert"
          >
            {{ alert.content }}
          </OrAlert>
          <router-view></router-view>
        </dv-layout>
      </dv-region>
    </dv-main>

    <dv-footer />
  </div>
</template>

<script>
import DvHeader from "./components/partials/header/Or-Header";
import Roles from "./stores/roles";

import DvFunctionalHeader from "./components/partials/functional-header/FunctionalHeader";
import DvUserInfo from "./components/partials/user-info/UserInfo";

import DvFunctionalHeaderContent from "./components/partials/functional-header/FunctionalHeaderContent";

import DvFunctionalHeaderSub from "./components/partials/functional-header/FunctionalHeaderSub";
import DvGrid from "./components/frame/grid/Grid";
import DvColumn from "./components/frame/column/Column";
import DvTabs from "./components/navigations/tabs/Tabs";
import DvTab from "./components/navigations/tabs/Tab";

import DvMain from "./components/frame/main/Or-Main";
import DvRegion from "./components/frame/region/Region";
import DvLayout from "./components/frame/layout/Layout";

import DvFooter from "./components/partials/footer/Footer";

import { mapActions, mapState, mapStores } from "pinia";
import { useUserStore } from "@/stores/user";
import { useAlertStore } from "@/stores/alert";
import OrAlert from "@/components/partials/alert/Alert";

export default {
  name: "App",
  components: {
    OrAlert,
    DvHeader,
    DvFunctionalHeader,
    DvUserInfo,
    DvFunctionalHeaderContent,
    DvFunctionalHeaderSub,
    DvGrid,
    DvColumn,
    DvTabs,
    DvTab,
    DvMain,
    DvRegion,
    DvLayout,
    DvFooter,
  },
  computed: {
    ...mapStores(useUserStore),
    ...mapState(useAlertStore, ["alert"]),
    navigations() {
      return this.userStore.getNavigations;
    },
  },
  methods: {
    ...mapActions(useAlertStore, ["setAlert", "clearAlert"]),
    refresh() {
      window.location.reload();
    },
  },
  async mounted() {
    this.userStore.loadUserFromToken();
    if (this.$route.meta.requiresAuth && !this.userStore.isLoggedIn) {
      await this.$router.push({ name: "unauthorized" });
    }
  },
  data() {
    return {
      title: "WEGWIJS ORGANISATIE",
      Roles: Roles,
    };
  },
};
</script>

<style lang="scss">
@import "./scss/theme.scss";

a:before,
a:hover:before {
  text-decoration: none;
}

.properties--disabled {
  color: #cbd2da;
  font-weight: lighter;
}

.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.5s;
}

.fade-enter,
.fade-leave-active {
  opacity: 0;
}
</style>
