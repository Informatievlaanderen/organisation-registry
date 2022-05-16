<template>
  <div :class="classes">
    <slot></slot>
  </div>
</template>

<script>
export default {
  name: "or-layout",
  props: {
    modIsWide: {
      default: false,
      type: Boolean,
    },
  },
  data() {
    return {
      classes: {
        // Standard layout class.
        layout: true,
        // Wide modifier - is used in every case.
        "layout--wide": this.modIsWide,
      },
    };
  },
};
</script>
