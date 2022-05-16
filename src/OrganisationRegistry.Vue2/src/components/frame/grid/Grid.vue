<template>
  <div :class="classes">
    <slot></slot>
  </div>
</template>

<script>
export default {
  // inject: ["$validator"],
  name: "or-grid",
  props: {
    modIsStacked: {
      default: false,
      type: Boolean,
    },
    modIsCenter: {
      default: false,
      type: Boolean,
    },
    modIsStretched: {
      default: false,
      type: Boolean,
    },
    modIsVCenter: {
      default: false,
      type: Boolean,
    },
  },
  data() {
    return {
      classes: {
        // Standard grid class.
        grid: true,
        // Adds spacing between vertical columns.
        "grid--is-stacked": this.modIsStacked,
        // Aligns columns horizontally.
        "grid--align-center": this.modIsCenter,
        // Stretches columns vertically to the
        // max size of the biggest one.
        // (Chrome only - use js-equal-height for backup)
        "grid--v-stretched": this.modIsStretched,
        // Align columns vertically.
        "grid--v-center": this.modIsVCenter,
      },
    };
  },
};
</script>
