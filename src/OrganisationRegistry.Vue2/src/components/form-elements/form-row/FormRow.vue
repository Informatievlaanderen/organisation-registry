<template>
  <component :is="type || 'div'" :class="classes">
    <slot></slot>
  </component>
</template>

<script>
export default {
  inject: ['$validator'],
  name: 'form-row',
  props: {
    type: {
      default: 'div',
      type: String,
    },
    modIsInline: {
      default: false,
      type: Boolean,
    },
  },
  data() {
    return {
      classes: {
        form__row: true,
        'form__row--inline': this.modIsInline,
      },
    };
  },
};
</script>
