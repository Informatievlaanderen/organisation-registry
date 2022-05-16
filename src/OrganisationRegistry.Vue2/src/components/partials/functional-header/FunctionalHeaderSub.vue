<template>
  <div :class="classes">
    <slot></slot>
  </div>
</template>

<script>
export default {
  name: "functional-header-sub",
  data() {
    return {
      classes: {
        "functional-header__sub": true,
      },
    };
  },
};
</script>
