import { defineStore } from "pinia";

export const useAlertStore = defineStore("alert", {
  state: () => {
    return {
      alert: {
        title: "",
        content: "",
        type: "",
        visible: false,
      },
    };
  },
  actions: {
    setAlert(alert) {
      this.alert = alert;
      this.alert.visible = true;
    },
    clearAlert() {
      this.$reset();
    },
  },
});
