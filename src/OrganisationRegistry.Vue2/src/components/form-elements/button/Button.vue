<template>
    <button :type="type" :id="id" :class="{
        'button': !this.modIsLink,
        'button--secondary': this.modIsSecondary,
        'button--secondary button--warning': this.modIsWarning,
        'button--disabled': this.disabled,
        'button--block': this.modIsBlock,
        'button--large': this.modIsLarge,
        'button--narrow': this.modIsNarrow,
        'button--wide': this.modIsWide,
        'button--loading': this.modIsLoading,
        'link': this.modIsLink
      }"
      :disabled="disabled">
    {{ label }}
    <slot></slot>
  </button>
</template>

<script>
export default {
  name: 'dv-button',
  props: {
    type: {
      default: 'button',
      type: String,
    },
    label: {
      default: '',
      type: String,
    },
    id: {
      default: '',
      type: String,
    },
    disabled: {
      default: false,
      type: Boolean,
    },
    modIsBlock: {
      default: false,
      type: Boolean,
    },
    modIsSecondary: {
      default: false,
      type: Boolean,
    },
    modIsWarning: {
      default: false,
      type: Boolean,
    },
    modIsLarge: {
      default: false,
      type: Boolean,
    },
    modIsNarrow: {
      default: false,
      type: Boolean,
    },
    modIsWide: {
      default: false,
      type: Boolean,
    },
    modIsLoading: {
      default: false,
      type: Boolean,
    },
    modIsLink: {
      default: false,
      type: Boolean,
    },
  },
  data() {
    return {
    };
  },
};
</script>
<style>
button.button {
  margin-right: 0px !important;
}
.button--secondary.button--warning {
  background-color: #db3434;
  color: #FFF;
  border-color: #db3434;
}
.button--secondary.button--warning:hover {
  background-color: #fa5c5c;
  color: #FFF;
  border-color: #fa5c5c;
}
.button--secondary.button--warning:hover {
  background-color: #fa5c5c;
  color: #FFF;
  border-color: #fa5c5c;
}
.button--secondary.button--warning.button--disabled {
  background-color:gray;
  color: #FFF;
  border-color: gray;
}
.button--secondary.button--warning.button--disabled:hover {
  background-color:gray;
  color: #FFF;
  border-color: gray;
}
</style>
