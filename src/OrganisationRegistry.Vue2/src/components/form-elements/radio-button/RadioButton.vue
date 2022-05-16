<template>
  <label :class="classes">
    <input type="radio" :name="name" :id="id" class="radio__toggle" :disabled="disabled" :checked="checked" :value="value"><span></span> {{ label }}
  </label>
</template>

<script>
export default {
  name: 'radio-button',
  props: {
    name: {
      default: '',
      type: String
    },
    value: {
      default: '',
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
    label: {
      default: '',
      type: String
    },
    checked: {
      default: false,
      type: Boolean
    },
    modIsBlock: {
      default: false,
      type: Boolean
    },
    modIsError: {
      default: false,
      type: Boolean
    }
  },
  data () {
    return {
      classes: {
        'radio': true,
        'radio--block': this.modIsBlock,
        'radio--error': this.modIsError,
        'radio--disabled': this.disabled
      }
    }
  }
}
</script>
