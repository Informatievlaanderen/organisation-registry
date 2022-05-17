import { defineStore } from "pinia";

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
    setUser(user) {
      this.isLoggedIn = true;
      this.name = user.family_name;
      this.firstName = user.given_name;
    },
    clearUser() {
      this.$reset();
    },
  },
});
