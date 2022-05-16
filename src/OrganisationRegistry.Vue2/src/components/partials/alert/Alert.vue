<template>
  <transition name="fade">
    <div
      :class="{
        alert: true,
        'alert--error': this.type === 'error',
        'alert--warning': this.type === 'warning',
        'alert--success': this.type === 'success',
        'alert--cta': this.type === 'cta',
        'alert--closable': this.isClosable,
      }"
      v-if="visible"
    >
      <a
        href=""
        class="alert__close link--icon--close u-float-right"
        v-if="isClosable"
        ><span class="u-visually-hidden">Sluiten</span></a
      >
      <div class="alert__icon" aria-hidden="true" v-if="hasIcon"></div>
      <div class="alert__content">
        <div class="alert__title">{{ title }}</div>
        <div class="alert__message">
          <div class="typography">
            <slot></slot>
          </div>
        </div>
      </div>
    </div>
  </transition>
</template>

<script>
export default {
  name: "or-alert",
  props: {
    title: {
      default: "",
      type: String,
    },
    type: {
      default: null,
      type: String,
    },
    hasIcon: {
      default: true,
      type: Boolean,
    },
    isClosable: {
      default: true,
      type: Boolean,
    },
    visible: {
      default: false,
      type: Boolean,
    },
  },
  data() {
    return {};
  },
};
</script>

<style scoped>
.fade-enter-active,
.fade-leave-active {
  transition: opacity 0.5s;
}
.fade-enter, .fade-leave-to /* .fade-leave-active below version 2.1.8 */ {
  opacity: 0;
}
</style>
