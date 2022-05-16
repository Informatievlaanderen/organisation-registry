<template>
	<div class="checkbox--switch__wrapper">
		<input type="checkbox" :name="name" :id="name" class="checkbox--switch" :disabled="disabled" :checked="checked" :value="value">
		<label :for="name" class="checkbox--switch__label"></label> {{ label }}
	</div>
</template>

<script>
export default {
  name: 'checkbox-switch',
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
        'checkbox': true,
        'checkbox--block': this.modIsBlock,
        'checkbox--error': this.modIsError,
        'checkbox--disabled': this.disabled
      }
    }
  }
}
</script>
