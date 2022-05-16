<template>
  <router-link class="button" :exact="exact" :to="to">
    {{text}}
  </router-link>
</template>

<script>
// import { isLocation, isString } from 'core/helpers';

export default {
  name: 'route-button',
  props: {
    text: {
      required: true,
      type: String,
    },
    to: {
      required: true,
      type: [String, Object],
      // validator: to => isString(to) || isLocation(to)
    },
    exact: {
      default: false,
      type: Boolean,
    },
  },
};
</script>

<style scoped>
</style>
