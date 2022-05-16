<template>
  <div class="functional-header__content">
    <h1 class="functional-header__title">
      <span v-if="title">{{ title }}</span>
      <slot name="title"></slot>
    </h1>
    <a href="https://wegwijs.vlaanderen.be">(Terug naar V1)</a>
  </div>
</template>

<script>
export default {
  name: "functional-header-content",
  props: {
    title: {
      default: "",
      type: String,
    },
  },
};
</script>
