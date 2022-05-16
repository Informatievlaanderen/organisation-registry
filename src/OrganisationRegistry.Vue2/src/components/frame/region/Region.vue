<template>
  <div :class="classes">
    <slot></slot>
  </div>
</template>

<script>
export default {
  name: "or-region",
  props: {
    modIsAlt: {
      default: false,
      type: Boolean,
    },
    modIsSmall: {
      default: false,
      type: Boolean,
    },
  },
  data() {
    return {
      classes: {
        // Standard region class.
        region: true,
        // Region with grey background.
        "region--alt": this.modIsAlt || false,
        // Region with smaller margins
        "region--small": this.modIsSmall || false,
      },
    };
  },
};
</script>
