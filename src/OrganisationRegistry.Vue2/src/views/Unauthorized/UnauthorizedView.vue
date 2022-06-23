<template>
  <div>
    <OrAlert
      :visible="true"
      :is-closable="false"
      :type="alert.type"
      :title="alert.title"
    >
      {{ alert.content }}
    </OrAlert>
  </div>
</template>

<script>
document.title = "Unauthorized";

import alerts from "@/alerts/alerts";
import OrAlert from "@/components/partials/alert/Alert";

export default {
  name: "UnauthorizedView.vue",
  components: { OrAlert },
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
