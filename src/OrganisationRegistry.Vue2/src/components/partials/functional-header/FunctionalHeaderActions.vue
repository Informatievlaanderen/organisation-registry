<template>
  <div class="functional-header__actions">
    <ul>
      <slot></slot>
    </ul>
  </div>
</template>

<script>
export default {
  name: "functional-header-actions",
  props: {
    actions: {
      default: null,
      type: Array,
    },
  },
};
</script>

<style scoped>
.functional-header__actions {
  padding: 1rem 0.5rem 1rem;
}

@media screen and (max-width: 767px) {
  .functional-header__actions {
    margin-top: -0.9rem;
  }

  .functional-header__actions {
    display: block;
  }
}

@media screen and (max-width: 580px) {
  .functional-header__actions {
    float: left;
    width: 100%;
    margin-bottom: 10px;
    margin-top: -5px;
  }
}
</style>
