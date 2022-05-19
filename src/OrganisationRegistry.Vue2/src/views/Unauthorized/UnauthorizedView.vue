<template>
  <vl-alert :icon="alert.type" :title="alert.title" role="alertdialog">
    <p>
      {{ alert.content }}
    </p>
  </vl-alert>
</template>

<script>
import alerts from "@/alerts/alerts";

export default {
  name: "UnauthorizedView.vue",
  methods: {},
  mounted() {
    this.alert = alerts.unauthorized;
  },
  data() {
    return {
      alert: {},
    };
  },
};
</script>

<style scoped></style>
