<template>
  <textarea :name="name" :id="id" :class="classes" :disabled="disabled" :cols="cols" :rows="rows" :placeholder="placeholder">{{ value }}</textarea>
</template>

<script>
export default {
  name: 'textarea',
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
    placeholder: {
      default: '',
      type: String
    },
    disabled: {
      default: false,
      type: Boolean
    },
    cols: {
      default: '40',
      type: String
    },
    rows: {
      default: '4',
      type: String
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
        'textarea': true,
        'textarea--block': this.modIsBlock,
        'textarea--error': this.modIsError,
        'textarea--disabled': this.disabled
      }
    }
  }
}
</script>
