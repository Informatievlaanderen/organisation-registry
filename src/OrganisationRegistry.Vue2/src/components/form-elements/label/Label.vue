<template>
    <label :for="name" :id="id" :class="classes">
        <slot></slot>
    </label>
</template>

<script>
export default {
  name: 'dv-label',
  props: {
    name: {
      default: '',
      type: String,
    },
    id: {
      default: '',
      type: String,
    },
    modIsHidden: {
      default: false,
      type: Boolean,
    },
    modIsBlock: {
      default: false,
      type: Boolean,
    },
    modIsInline: {
      default: false,
      type: Boolean,
    },
  },
  data() {
    return {
      classes: {
        form__label: true,
        'form__label--block': this.modIsBlock,
        'form__label--inline': this.modIsInline,
        'u-visually-hidden': this.modIsHidden,
      },
    };
  },
};
</script>
