<template>
  <component :is="type" :id="id" :class="classes" :disabled="disabled" :title="label" :aria-label="label">
    <svg role="presentation" class="icon-button__icon" v-svg :symbol="icon" aria-hidden="true" :size="iconSize"></svg>
  </component>
</template>

<script>
export default {
  name: 'icon-button',
  props: {
    type: {
      default: 'button',
      type: String
    },
    label: {
      default: '',
      type: String
    },
    icon: {
      default: '',
      type: String
    },
    iconSize: {
      default: '0 0 20 20',
      type: String
    },
    id: {
      default: '',
      type: String
    },
    disabled: {
      default: false,
      type: Boolean
    },
    modIsSecondary: {
      default: false,
      type: Boolean
    },
    modIsWarning: {
      default: false,
      type: Boolean
    },
    modIsLarge: {
      default: false,
      type: Boolean
    },
    modIsNarrow: {
      default: false,
      type: Boolean
    }
  },
  data () {
    return {
      classes: {
        'button': true,
        'icon-button': true,
        'button--secondary': this.modIsSecondary,
        'button--secondary button--warning': this.modIsWarning,
        'button--disabled': this.disabled,
        'button--large': this.modIsLarge,
        'button--narrow': this.modIsNarrow
      }
    }
  }
}
</script>
