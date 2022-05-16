<template>
  <div class="tabs__wrapper">
    <ul :class="classes">
      <slot></slot>
    </ul>
  </div>
</template>

<script>
export default {
  components: {},
  name: "or-tabs",
  data() {
    return {
      classes: {
        tabs: true,
      },
    };
  },
};
</script>
