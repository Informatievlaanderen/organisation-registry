<template>
  <li v-if="to" class="functional-header__action">
    <router-link :to="to">{{ title }}</router-link>
  </li>
  <li v-else-if="on" class="functional-header__action">
    <a tabindex="0" v-on:click="on" v-on:keyup.enter="on">{{ title }}</a>
  </li>
  <li v-else class="functional-header__action">{{ title }}</li>
</template>

<script>
export default {
  name: "functional-header-actions",
  props: {
    title: {
      default: "",
      required: true,
      type: String,
    },
    to: {
      default: "",
      type: String,
    },
    on: {
      required: false,
    },
  },
};
// import Vue from 'vue';
// import { Location } from 'vue-router';
// import { Component, Prop } from 'vue-property-decorator';
// import { isLocation, isString } from 'core/helpers';

// @Component
// export default class FunctionalHeaderAction extends Vue {
//   @Prop({ default: '', required: true })
//   title: string

//   @Prop({
//     default: null,
//     type: [String, Object],
//     validator: to => isString(to) || isLocation(to)
//   })
//   to: string | Location

//   @Prop({ required: false })
//   on: Function
// }
</script>

<style scoped>
.functional-header__action a {
  cursor: pointer;
  text-decoration: none;
}

.functional-header__action a:hover {
  text-decoration: underline;
}
</style>
