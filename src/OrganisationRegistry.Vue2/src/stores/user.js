import { defineStore } from "pinia";
import {
  getToken,
  getVerifier,
  removeToken,
  setToken,
} from "@/api/localStorage";
import { exchangeCode, getSecurityInfo } from "@/api/security";
import OidcClient from "@/api/oidc";
import { useAlertStore } from "@/stores/alert";
import alerts from "@/alerts/alerts";
import { UserTokenResult } from "@/stores/userTokenResult";
import NavigationTabs from "./nav-bar";

export const useUserStore = defineStore("user", {
  state: () => {
    return {
      isLoggedIn: false,
      user: {
        name: "",
        firstName: "",
        roles: [],
      },
      navigations: [],
    };
  },
  getters: {
    userDescription: (state) =>
      `${state.user.name} ${
        state.user.firstName
      } (${state.user.translatedRoles.join(", ")})`,
    getNavigations: (state) => state.navigations,
  },
  actions: {
    async initializeOidcClient() {
      const securityInfo = await getSecurityInfo();
      if (securityInfo) {
        this.oidcClient = new OidcClient(securityInfo);
      }
    },
    loadUserFromToken() {
      try {
        const userToken = UserTokenResult.fromJwt(getToken());
        if (!userToken.succeeded) {
          this.clearUser();
          return;
        }

        if (userToken.isExpired) {
          this.clearUser(alerts.sessionExpired);
          return;
        }

        this.setUser(userToken);
        this.setNavigations(userToken.user.translatedRoles);
      } catch (e) {
        console.error("Could not decode provided jwt", e);
        this.clearUser();
      }
    },
    /**
     * @param {UserTokenResult} userToken
     */
    setUser(userToken) {
      this.isLoggedIn = true;
      this.user = { ...userToken.user };
    },
    clearUser(alert) {
      if (alert) {
        const alertStore = useAlertStore();
        alertStore.setAlert(alert);
      }
      this.$reset();
      removeToken();
    },
    signIn() {
      this.oidcClient.signIn();
    },
    signOut() {
      this.oidcClient.signOut();
    },
    async exchangeCode(code) {
      const alertStore = useAlertStore();

      const verifier = getVerifier();
      const redirectUri = this.oidcClient.client.settings.redirect_uri;
      const response = await exchangeCode(code, verifier, redirectUri);
      const token = await response.text();

      try {
        setToken(token);
        this.loadUserFromToken();
        await this.router.push({ path: "/" });

        alertStore.setAlert(alerts.loginSuccess);
        setTimeout(() => alertStore.clearAlert(), 5000);
      } catch (e) {
        alertStore.setAlert(alerts.loginFailed);
        setTimeout(() => alertStore.clearAlert(), 5000);
        console.error(e);
      }
    },
    setNavigations(translatedRoles) {
      this.navigations = NavigationTabs.filter(
        (tab) =>
          tab.roles.length === 0 ||
          tab.roles.some((r) => translatedRoles.includes(r))
      );
    },
  },
});
