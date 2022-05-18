<template>
  <div class="cta-title">
    <h1 class="h2 cta-title__title">Even geduld aub...</h1>
  </div>
</template>

<script>
import { useUserStore } from "@/stores/user";
import { mapStores } from "pinia/dist/pinia";

export default {
  props: ["code"],
  computed: {
    ...mapStores(useUserStore),
  },
  async mounted() {
    await this.userStore.exchangeCode(this.code);
  },
};
</script>
