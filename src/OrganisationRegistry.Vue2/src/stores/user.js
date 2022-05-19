import { defineStore } from "pinia";
import jwtDecode from "jwt-decode";
import { getToken, getVerifier, setToken } from "@/api/localStorage";
import { exchangeCode, getSecurityInfo } from "@/api/security";
import OidcClient from "@/api/oidc";
import { useAlertStore } from "@/stores/alert";
import alerts from "@/alerts/alerts";

export const useUserStore = defineStore("user", {
  state: () => {
    return {
      isLoggedIn: false,
      name: "",
      firstName: "",
      roles: [],
    };
  },
  actions: {
    async initializeOidcClient() {
      const securityInfo = await getSecurityInfo();
      if (securityInfo) {
        this.oidcClient = new OidcClient(securityInfo);
      }
    },
    loadUserFromToken() {
      const token = getToken();
      if (token) {
        const decoded = jwtDecode(token);
        this.setUser(decoded);
      } else {
        this.clearUser();
      }
    },
    setUser(user) {
      this.isLoggedIn = true;
      this.name = user.family_name;
      this.firstName = user.given_name;
    },
    clearUser() {
      this.$reset();
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
      console.log("response", response);
      const token = await response.text();

      try {
        jwtDecode(token);
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
  },
});
