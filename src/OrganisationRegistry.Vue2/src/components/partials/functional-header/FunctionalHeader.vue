<template>
  <header :class="classes">
    <dv-layout :mod-is-wide="true">
      <slot></slot>
    </dv-layout>
  </header>
</template>

<script>
import DvLayout from "../../frame/layout/Layout";

export default {
  name: "functional-header",
  components: {
    DvLayout,
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
