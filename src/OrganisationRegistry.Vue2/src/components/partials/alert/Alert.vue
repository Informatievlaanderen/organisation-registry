<template>
  <transition name="fade">
    <vl-alert
      :icon="getIcon(type)"
      :title="title"
      v-if="visible"
      :closable="isClosable"
      :mod-error="type === 'error'"
      :mod-warning="type === 'warning'"
      :mod-success="type === 'success'"
      mod-small
      @close="close"
      role="alertdialog"
    >
      <p>
        <slot></slot>
      </p>
    </vl-alert>
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
      default: true,
      type: Boolean,
    },
  },
  methods: {
    close() {
      this.$emit("close");
    },
    getIcon(type) {
      switch (type) {
        case "warning":
        case "error":
          return "warning";
        case "success":
          return "check-circle";
        default:
          break;
      }
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
